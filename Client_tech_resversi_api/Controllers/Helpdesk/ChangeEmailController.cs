using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Non_DB_models;
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
    public class ChangeEmailController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<ChangeEmailController> _logger;

        public ChangeEmailController(ReversiContext context, ILogger<ChangeEmailController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> SetNewEmail(int id, [FromBody] string emailaddress)
        {
            _logger.LogMsg(HttpContext, $"{id} {emailaddress}");

            var user = await _context.Users
                .Include(x => x.UserAccount)
                .Include(x => x.UserClaims)
                .Include(x => x.UserRoles)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest();
            }

            try
            {
               new MailAddress(emailaddress);
            }
            catch
            {
                return StatusCode(406);
            }

            user.Email = emailaddress;

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