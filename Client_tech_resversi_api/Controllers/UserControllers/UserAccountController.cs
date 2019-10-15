using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets;
using Client_tech_resversi_api.Assets.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Client_tech_resversi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<UserAccountController> _logger;

        public UserAccountController(ReversiContext context, ILogger<UserAccountController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/UserAccount/5
        [HttpGet("{id}")]
        public async Task<ActionResult> NewActivationCode(int id)
        {
            _logger.LogMsg( HttpContext,id.ToString());

            var dbUser = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.UserAccount)
                .FirstOrDefaultAsync();

            if (dbUser == null || dbUser.UserAccount == null)
            {
                return NotFound();
            }


            if (dbUser.UserAccount.Verified)
            {
                return BadRequest();
            }
                
            //generating new activation key
            string activationKey = new Random().Next(100000, 1000000).ToString();
            string activationHash = new HashGenerator().GenerateHash(activationKey);

            dbUser.UserAccount.ActivationKey = activationHash;
            
            try
            {
                await _context.SaveChangesAsync();
                await new Emailer().SendActivationMailAsync(new MailboxAddress(dbUser.Email), dbUser.ScreenName, activationKey);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> ValidateActivationCode(int id,[FromBody] string code)
        {
            _logger.LogMsg(HttpContext, $"{id} {code}");

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (userAccount == null)
            {
                return NotFound();
            }


            if (new HashGenerator().GenerateHash(code) != userAccount.ActivationKey)
            {
                return BadRequest();
            }


            if (userAccount.Verified)
            {
                return StatusCode(401);
            }
                
            userAccount.Verified = true;
            userAccount.ActivationKey = "";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
            
            return Ok();
        }
    }
}
