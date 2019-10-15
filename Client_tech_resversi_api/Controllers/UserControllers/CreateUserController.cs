using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Assets.Interfaces;
using Client_tech_resversi_api.Models.Non_DB_models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Client_tech_resversi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateUserController : ControllerBase
    {
        private readonly ILogger<CreateUserController> _logger;
        private readonly ReversiContext _context;
        private readonly IPasswordManager _passwordManger;
        private readonly IStringValidator _stringValidator;
        private readonly IHashGenerator _hashGenerator;

        public CreateUserController(ReversiContext context, IPasswordManager passwordManager, IStringValidator stringValidator, 
            IHashGenerator hashGenerator, ILogger<CreateUserController> logger)
        {
            _logger = logger;
            _hashGenerator = hashGenerator;
            _stringValidator = stringValidator;
            _passwordManger = passwordManager;
            _context = context;
        }
        
        // POST: api/CreateUser
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(RegisterUser user)
        {
            _logger.LogMsg( HttpContext,$"{user.Name} {user.Email}");

            if (!_stringValidator.StrongPassword(user.Password))
            {
                return BadRequest("Weak password");
            }

            var sqlParm = new SqlParameter("username", user.Name);

            if (await _context.Users.FromSql("select * from dbo.[User] where Name = @username", sqlParm).OrderBy(x => x.Id).FirstOrDefaultAsync() != null)
            {
                return BadRequest("Username already exists.");
            }

            sqlParm = new SqlParameter("email", user.Email);

            if (await _context.Users.FromSql("select * from dbo.[User] where Email = @email", sqlParm).OrderBy(x => x.Id).FirstOrDefaultAsync() != null)
            {
                return BadRequest("Email address already exists.");
            }

            _passwordManger.GenerateSaltAndPasswordHash(user.Password, out var hash, out var salt);
            
            SqlParameter[] sqlParmList = new []
            {
                new SqlParameter("name",user.Name),
                new SqlParameter("screenname",user.ScreenName),
                new SqlParameter("email",user.Email),
                new SqlParameter("password", hash),
                new SqlParameter("salt", salt),
            };

            try
            {
                await _context.Database.ExecuteSqlCommandAsync("insert into dbo.[User](Name,ScreenName,Email,Password,Salt) values (@name, @screenname, @email, @password, @salt)", sqlParmList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "Unable to create account, please try again later");
            }


            //generating activation key
            string activationKey = new Random().Next(100000, 1000000).ToString();
            string activationHash = _hashGenerator.GenerateHash(activationKey);

            var id = await _context.Users.Where(x => x.Name == user.Name).Select(x => x.Id).FirstOrDefaultAsync();

            try
            {
                sqlParmList = new[]
                {
                    new SqlParameter("userid", id),
                    new SqlParameter("status", 'A'),
                    new SqlParameter("key", activationHash),
                    new SqlParameter("verified", false),
                    new SqlParameter("attempts", value:0), 
                };

                await _context.Database.ExecuteSqlCommandAsync("insert into dbo.UserAccount(UserId,Status,ActivationKey,Verified,LoginAttempts,RecoverAttempts) values(@userid,@status,@key,@verified,@attempts,@attempts)", sqlParmList);

                sqlParmList = new[]
                {
                    new SqlParameter("userid", id),
                    new SqlParameter("role", "User")
                };

                await _context.Database.ExecuteSqlCommandAsync("insert into dbo.UserRole(UserId,Role) values(@userid,@role)", sqlParmList);

                sqlParmList = new[]
                {
                    new SqlParameter("userid", id),
                    new SqlParameter("claim", "UserId"),
                    new SqlParameter("value", id),
                    new SqlParameter("issuer", "LOCALAUTHORITY")
                };

                await _context.Database.ExecuteSqlCommandAsync("insert into dbo.UserClaim(UserId,Claim,Value,Issuer) values (@userid,@claim,@value,@issuer)", sqlParmList);
            }
            catch (Exception exception)
            {
                var qParm = new SqlParameter("userId", id);
                await _context.Database.ExecuteSqlCommandAsync("delete from dbo.UserAccount where Id = @userId", qParm);
                await _context.Database.ExecuteSqlCommandAsync("delete from dbo.UserRole where Id = @userId", qParm);
                await _context.Database.ExecuteSqlCommandAsync("delete from dbo.UserClaim where Id = @userId", qParm);
                await _context.Database.ExecuteSqlCommandAsync("delete from dbo.[User] where Id = @userId", qParm);

                Console.WriteLine(exception);
                return StatusCode(500, "Unable to create account, please try again later");
            }

            try
            {
                await new Emailer().SendActivationMailAsync(new MailboxAddress(user.Email), user.ScreenName, activationKey);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "Account created, but unable to send confirmation email. Please contact the server admin");
            }
            
            return CreatedAtAction("CreateUser", new { id = id }, null);
        }
    }
}
