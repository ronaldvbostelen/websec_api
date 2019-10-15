using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Client_tech_resversi_api.Controllers.SuperAdminControllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<RolesController> _logger;

        public RolesController(ReversiContext context, ILogger<RolesController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> AddRole(UserRole role)
        {
            _logger.LogMsg(HttpContext, $"{role.UserId} {role.Role}");

            _context.UserRoles.Add(role);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(); // integrity validation probably failed therfore 400 and not 500
            }

            return Ok();
        }

        [HttpDelete("{roleId}")]
        public async Task<ActionResult> RevokeRole(int roleId)
        {
            _logger.LogMsg(HttpContext, roleId.ToString());

            _context.UserRoles.Remove(new UserRole {Id = roleId});

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(); // integrity validation probably failed therfore 400 and not 500
            }

            return Ok();
        }
    }
}