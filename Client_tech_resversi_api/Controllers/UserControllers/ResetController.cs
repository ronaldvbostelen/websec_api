using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Non_DB_models;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Client_tech_resversi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<ResetController> _logger;

        public ResetController(ReversiContext context, ILogger<ResetController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPut]
        public async Task<ActionResult> ResetPassword(ResetUserPassword user)
        {
            _logger.LogMsg(HttpContext, $"{user.UserId}, resetcode: {user.ResetCode}");

            if (user.UserId == 0 || string.IsNullOrEmpty(user.ResetCode) || (user.PassOne != user.PassTwo) || !new StringValidator().StrongPassword(user.PassOne))
            {
                return BadRequest();
            }

            var dbUser = await _context.Users
                .Where(x => x.Id == user.UserId)
                .Include(x => x.UserAccount)
                .FirstOrDefaultAsync();

            if (dbUser == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(dbUser.UserAccount.RecoverKey))
            {
                return BadRequest();
            }

            if (new HashGenerator().GenerateHash(user.ResetCode) != dbUser.UserAccount.RecoverKey)
            {
                try
                {
                    if (await TestBruteForce(dbUser.UserAccount))
                    {
                        return StatusCode(429);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return StatusCode(500);
                }
            }

            new PasswordManager().GenerateSaltAndPasswordHash(user.PassOne, out string hash, out string salt);

            dbUser.Password = hash;
            dbUser.Salt = salt;
            dbUser.UserAccount.LoginAttempts = 0;
            dbUser.UserAccount.RecoverKey = "";
            dbUser.UserAccount.RecoverAttempts = 0;
            dbUser.UserAccount.SecurityHash = "";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return Ok(dbUser.Name);

        }

        [HttpPost]
        public async Task<ActionResult> RequestResetPassword([FromBody] string emailOrUsername)
        {
            _logger.LogMsg(HttpContext, emailOrUsername);

            if (string.IsNullOrWhiteSpace(emailOrUsername))
            {
                return BadRequest();
            }

            User user;

            if (emailOrUsername.Contains('@'))
            {
                user = await _context.Users
                    .Where(x => x.Email == emailOrUsername.Trim())
                    .Include(x => x.UserAccount)
                    .FirstOrDefaultAsync();
            }
            else
            {
                user = await _context.Users
                    .Where(x => x.Name == emailOrUsername.Trim())
                    .Include(x => x.UserAccount)
                    .FirstOrDefaultAsync();
            }

            if (user == null)
            {
                return NotFound();
            }

            if (!user.UserAccount.Verified)
            {
                return StatusCode(403, user.Id);
            }

            var recoverycode = new Random().Next(100000,1000000).ToString();
            var hashMaker = new HashGenerator();
            var recoveryHash = hashMaker.GenerateHash(recoverycode);
            var securityHash = hashMaker.GenerateURlSaveHash(recoveryHash);

            user.UserAccount.RecoverKey = recoveryHash;
            user.UserAccount.SecurityHash = securityHash;
            user.UserAccount.LoginAttempts = 0;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            await new Emailer().SendRecoverMailAsync(new MailboxAddress(user.Email), user.ScreenName, recoverycode);
            
            return Ok(new { user.Id, securityHash });
        }

        [HttpGet]
        public async Task<ActionResult> ValidateResetCode([FromQuery] string account, [FromQuery] string code, [FromQuery] string hash)
        {
            _logger.LogMsg(HttpContext, $"{account} {code} {hash}");

            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(hash))
            {
                return BadRequest();
            }

            User user;

            if (account.Contains('@'))
            {
                user = await _context.Users
                    .Where(x => x.Email == account.Trim())
                    .Include(x => x.UserAccount)
                    .FirstOrDefaultAsync();
            }
            else
            {
                user = await _context.Users
                    .Where(x => x.Name == account.Trim())
                    .Include(x => x.UserAccount)
                    .FirstOrDefaultAsync();
            }

            if (string.IsNullOrWhiteSpace(user.UserAccount.RecoverKey))
            {
                return BadRequest();
            }

            if (user.UserAccount.SecurityHash != hash)
            {
                return BadRequest();
            }

            if (user.Id == 0)
            {
                return NotFound();
            }
            
            if (new HashGenerator().GenerateHash(code) == user.UserAccount.RecoverKey)
            {
                return Ok();
            }

            try
            {
                if (await TestBruteForce(user.UserAccount))
                {
                    return StatusCode(429);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return BadRequest();
        }

        private async Task<bool> TestBruteForce(UserAccount userAccount)
        {
            userAccount.RecoverAttempts++;

            if (userAccount.RecoverAttempts > 4)
            {
                userAccount.RecoverKey = "";
                userAccount.SecurityHash = "";
                userAccount.RecoverAttempts = 0;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                return true;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return false;
        }
    }
}
