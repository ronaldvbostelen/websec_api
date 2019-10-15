using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets;
using Client_tech_resversi_api.Assets.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Non_DB_models;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Client_tech_resversi_api.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ReversiContext context, ILogger<UserController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetUser(int id)
        {
            _logger.LogMsg(HttpContext, id.ToString());

            if (id != int.Parse(User.FindFirstValue("UserId")))
            {
                return Unauthorized();
            }

            var dbUser = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.UserAccount)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (dbUser == null)
            {
                return NotFound();
            }

            return new UserProfile{UserId = dbUser.Id, Email = dbUser.Email,Username = dbUser.Name, ScreenName = dbUser.ScreenName, TwoFa = dbUser.UserAccount.TwoFactorAuth};
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchUser(int id, UserProfile user)
        {
            _logger.LogMsg(HttpContext, $"{id} {user.Username}");

            if (id != int.Parse(User.FindFirstValue("UserId")))
            {
                return Unauthorized();
            }

            if (id == 0 || user.UserId == 0 || id != user.UserId)
            {
                return BadRequest(user);
            }

            var dbUser = await _context.Users.FindAsync(id);

            if (dbUser == null)
            {
                return NotFound(user);
            }

            if (dbUser.ScreenName != user.ScreenName)
            {
                dbUser.ScreenName = user.ScreenName;
            }

            if (dbUser.Email != user.Email)
            {
                dbUser.Email = user.Email;
            }

            if (!_context.ChangeTracker.HasChanges())
            {
                return StatusCode(208, user);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, user);
            }

            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> ChangePassword(int id, ChangeUserPassword changeUserPassword)
        {
            _logger.LogMsg(HttpContext, $"{id}");

            if (id != int.Parse(User.FindFirstValue("UserId")))
            {
                return Unauthorized();
            }

            if (id == 0 || id != changeUserPassword.UserId)
            {
                return BadRequest();
            }

            if (changeUserPassword.PasswordOne != changeUserPassword.PasswordTwo || !new StringValidator().StrongPassword(changeUserPassword.PasswordOne))
            {
                return BadRequest();
            }

            var dbUser = await _context.Users.FindAsync(id);

            if (dbUser == null)
            {
                return NotFound();
            }

            var pwManager = new PasswordManager();

            if (!pwManager.VerifyPassword(changeUserPassword.CurrentPassword, dbUser.Password, dbUser.Salt))
            {
                return BadRequest();
            }

            pwManager.GenerateSaltAndPasswordHash(changeUserPassword.PasswordOne, out string hash, out string salt);

            dbUser.Password = hash;
            dbUser.Salt = salt;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return Ok();
        }
    }
}
