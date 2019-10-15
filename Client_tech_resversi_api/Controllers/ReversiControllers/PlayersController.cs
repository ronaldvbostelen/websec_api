using Client_tech_resversi_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Client_tech_resversi_api.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly ReversiContext _context;

        public PlayersController(ReversiContext context)
        {
            _context = context;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(Guid playerId)
        {
            if (playerId == Guid.Empty)
                return BadRequest();

            var lookUp = await _context.Player.FindAsync(playerId);

            if (lookUp == null)
                return NotFound();

            return lookUp;
        }

        // POST: api/Players
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player player)
        {
            _context.Player.Add(player);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.InnerException.Message ?? e.Message);
            }

            return CreatedAtAction("PostPlayer",player.playerId);
        }

    }
}
