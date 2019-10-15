using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client_tech_resversi_api.Controllers.SuperAdminControllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(ReversiContext context, ILogger<ClaimsController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> AddUserClaim(UserClaim claim)
        {
            _logger.LogMsg(HttpContext, $"{claim.Claim} {claim.Value} {claim.Issuer}");

            _context.UserClaims.Add(claim);

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

        [HttpDelete("{claimId}")]
        public async Task<ActionResult> RevokeUserClaim(int claimId)
        {
            _logger.LogMsg(HttpContext, claimId.ToString());

            _context.UserClaims.Remove(new UserClaim {Id = claimId});
            
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