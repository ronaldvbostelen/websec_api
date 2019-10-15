using Client_tech_resversi_api.Assets;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Non_DB_models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Assets.Interfaces;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using OtpNet;

namespace Client_tech_resversi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<LoginController> _logger;
        private readonly IPasswordManager _passwordManager;

        public LoginController(ReversiContext context, ILogger<LoginController> logger, IPasswordManager passwordManager)
        {
            _passwordManager = passwordManager;
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginUser loginUser, bool twoFaSucces = false)
        {
            _logger.LogMsg(HttpContext, loginUser.ToString());

            if (string.IsNullOrWhiteSpace(loginUser.Name) || string.IsNullOrWhiteSpace(loginUser.Password))
            {
                return BadRequest();
            }

            var sqlParam = new SqlParameter("username", loginUser.Name);
            
            var dbUserRaw = await _context.Users.FromSql("select * from dbo.[User] where Name = @username", sqlParam).OrderBy(x => x.Id).FirstOrDefaultAsync();

            if (dbUserRaw == null)
            {
                return NotFound();
            }

            sqlParam = new SqlParameter("userid", dbUserRaw.Id);

            dbUserRaw.UserAccount = await _context.UserAccounts.FromSql("select * from dbo.UserAccount where UserID = @userid", sqlParam).OrderBy(x => x.Id).FirstOrDefaultAsync();

            if (dbUserRaw.UserAccount == null)
            {
                return StatusCode(500);
            }
            
            if (!_passwordManager.VerifyPassword(loginUser.Password, dbUserRaw.Password, dbUserRaw.Salt))
            {
                Thread.Sleep(1000 * dbUserRaw.UserAccount.LoginAttempts);

                try
                {
                    SqlParameter[] parms = new[]
                    {
                        new SqlParameter("id", dbUserRaw.UserAccount.Id),
                        new SqlParameter("attempts", ++dbUserRaw.UserAccount.LoginAttempts)
                    };

                    await _context.Database.ExecuteSqlCommandAsync("update dbo.UserAccount set LoginAttempts = @attempts where Id = @id", parms);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return StatusCode(500);
                }

                return BadRequest();
            }

            if (dbUserRaw.UserAccount.TwoFactorAuth && !twoFaSucces)
            {
                return StatusCode(426);
            }

            if (dbUserRaw.UserAccount.Status != 'A')
            {
                return StatusCode(423);
            }

            if (!dbUserRaw.UserAccount.Verified)
            {
                return StatusCode(403, dbUserRaw.Id);
            }

            try
            {
                SqlParameter[] parms = new[]
                {
                    new SqlParameter("id", dbUserRaw.UserAccount.Id),
                    new SqlParameter("attempts",  value:0),
                    new SqlParameter("loggedin", DateTime.Now.ToString()), 
                    new SqlParameter("logginfrom", HttpContext.Connection.RemoteIpAddress.ToString())
                };

                await _context.Database.ExecuteSqlCommandAsync("update dbo.UserAccount set LoginAttempts = @attempts, LastTimeLoggedIn = @loggedin, LastTimeLoggedInFrom = @logginfrom where Id = @id", parms);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            dbUserRaw.UserRoles = await _context.UserRoles.FromSql("select * from dbo.UserRole where UserId = @userid", sqlParam).ToListAsync();
            dbUserRaw.UserClaims = await _context.UserClaims.FromSql("select * from UserClaim where UserId = @userid", sqlParam).ToListAsync();
            dbUserRaw.UserLastChanged = await _context.UserLastChanges.FromSql("select * from dbo.UserLastChanged where UserId = @userid", sqlParam).OrderBy(x => x.Id).FirstOrDefaultAsync();

            var claims = new List<Claim>();
            
            foreach (var dbUserUserRole in dbUserRaw.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, dbUserUserRole.Role));
            }

            foreach (var dbUserUserClaim in dbUserRaw.UserClaims)
            {
                claims.Add(new Claim(dbUserUserClaim.Claim, dbUserUserClaim.Value));
            }

            claims.Add(new Claim("LastChanged", dbUserRaw.UserLastChanged.DateTimeChanged));

            return Ok(claims);
        }

        [HttpPatch]
        public async Task<ActionResult> LoginTwoFa(TwoFaLoginUser twoFaLoginUserUser)
        {
            var secret = await _context.Users
                .Where(x => x.Name == twoFaLoginUserUser.Name)
                .Include(x => x.UserAccount)
                .Select(x => x.UserAccount.TotpSecret)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (secret == null || string.IsNullOrWhiteSpace(secret))
            {
                return BadRequest();
            }
            
            var keyBytes = Base32Encoding.ToBytes(secret);

            var totp = new Totp(keyBytes, mode: OtpHashMode.Sha256);

            if (totp.VerifyTotp(twoFaLoginUserUser.Code, out long match))
            {
                return await Login(new LoginUser{Name = twoFaLoginUserUser.Name, Password = twoFaLoginUserUser.Password}, true);
            }

            return BadRequest();
        }
    }
}