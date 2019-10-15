using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Authorize(Roles = "Helpdesk, Admin, SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserOverviewController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<UserOverviewController> _logger;

        public UserOverviewController(ReversiContext context, ILogger<UserOverviewController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserOverviewLight>>> GetUsers()
        {
            _logger.LogMsg(HttpContext,"");

            return await _context.Users
                .Include(x => x.UserAccount)
                .Include(x => x.UserRoles)
                .Select(x => new UserOverviewLight
                {
                    UserId = x.Id,
                    UserName = x.Name,
                    Verified = x.UserAccount.Verified,
                    UserRoles = x.UserRoles.ToList()
                })
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserOverview>> GetUser(int id)
        {
            _logger.LogMsg(HttpContext, id.ToString());

            var user = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.UserAccount)
                .Include(x => x.UserClaims)
                .Include(x => x.UserRoles)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return new UserOverview
            {
                UserId = user.Id,
                UserName = user.Name,
                ScreenName = user.ScreenName,
                Email = user.Email,
                UserAccount = user.UserAccount,
                UserClaims = user.UserClaims.ToList(),
                UserRoles = user.UserRoles.ToList(),
            };
        }

    }
}
