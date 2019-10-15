using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Extensions;
using Client_tech_resversi_api.Assets.Interfaces;
using Client_tech_resversi_api.Migrations;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Non_DB_models;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OtpNet;

namespace Client_tech_resversi_api.Controllers.UserControllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class TotpController : ControllerBase
    {
        private readonly ReversiContext _context;
        private readonly ILogger<TotpController> _logger;

        public TotpController(ReversiContext context, ILogger<TotpController> logger)
        {
            _context = context;
            _logger = logger;
        }
        

        [HttpPost("{userid}")]
        public async Task<ActionResult> EnableTotp(int userid)
        {
            _logger.LogMsg( HttpContext, userid.ToString());

            if (userid != int.Parse(User.FindFirstValue("UserId")))
            {
                return Unauthorized();
            }

            if (userid == 0)
            {
                return BadRequest();
            }

            var userAcc = await _context.UserAccounts.Where(x => x.UserId == userid).FirstOrDefaultAsync();

            if (userAcc == null)
            {
                return BadRequest();
            }

            userAcc.TotpSecret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(50));

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return Ok(userAcc.TotpSecret);
        }
        
        [HttpPut]
        public async Task<ActionResult> ConfirmTotp(TwoFaUser twoFaUser)
        {
            _logger.LogMsg(HttpContext, twoFaUser.UserId.ToString());

            if (twoFaUser.UserId != int.Parse(User.FindFirstValue("UserId")))
            {
                return Unauthorized();
            }

            if (twoFaUser.UserId == 0 || string.IsNullOrWhiteSpace(twoFaUser.Code))
            {
                return BadRequest();
            }

            var userAcc = await _context.UserAccounts.Where(x => x.UserId == twoFaUser.UserId).FirstOrDefaultAsync();

            if (userAcc == null)
            {
                return BadRequest();
            }

            if (!VerifyTotp(userAcc.TotpSecret, twoFaUser.Code))
            {
                return BadRequest();
            }

            userAcc.TwoFactorAuth = true;

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
        public async Task<ActionResult> DeleteTwoFa(int id)
        {
            _logger.LogMsg(HttpContext, id.ToString());

            if (id != int.Parse(User.FindFirstValue("UserId")))
            {
                return Unauthorized();
            }

            if (id == 0)
            {
                return BadRequest();
            }

            var userAcc = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.UserAccount)
                .Select(x => x.UserAccount)
                .FirstOrDefaultAsync();

            if (userAcc == null)
            {
                return BadRequest();
            }

            userAcc.TwoFactorAuth = false;
            userAcc.TotpSecret = "";

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

        private bool VerifyTotp(string secret, string code)
        {
            var keyBytes = Base32Encoding.ToBytes(secret);

            var totp = new Totp(keyBytes, mode: OtpHashMode.Sha256);

            return totp.VerifyTotp(code, out long match);
        }
    }
}