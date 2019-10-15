using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Assets.Interfaces;
using Client_tech_resversi_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Client_tech_resversi_api.Controllers.Helpdesk
{
    [Authorize(Roles = "Helpdesk, Admin, SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChangePasswordController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly IPasswordManager _pwManager;
        private readonly ILogger<ChangePasswordController> _logger;

        public ChangePasswordController(ReversiContext context, IPasswordManager pwManager, ILogger<ChangePasswordController> logger)
        {
            _logger = logger;
            _pwManager = pwManager;
            _context = context;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> SetPassword(int id, [FromBody] string newPassword)
        {
            _logger.LogMsg(HttpContext, $"{id}");

            var user = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.UserAccount)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest();
            }

            _pwManager.GenerateSaltAndPasswordHash(newPassword, out string hash, out string salt);

            user.Password = hash;
            user.Salt = salt;

            user.UserAccount.RecoverKey = "";
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

            return Ok();
        }
    }
}