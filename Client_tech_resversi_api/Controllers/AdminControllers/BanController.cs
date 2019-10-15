using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Client_tech_resversi_api.Controllers.AdminControllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BanController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<BanController> _logger;

        public BanController(ReversiContext context, ILogger<BanController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> BanUser(int id)
        {
            _logger.LogMsg( HttpContext, id.ToString());

            var user = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.UserAccount)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest();
            }

            user.UserAccount.Status = 'B';

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> UnBanUser(int id)
        {
            _logger.LogMsg(HttpContext, id.ToString());

            var user = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.UserAccount)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest();
            }

            user.UserAccount.Status = 'A';

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