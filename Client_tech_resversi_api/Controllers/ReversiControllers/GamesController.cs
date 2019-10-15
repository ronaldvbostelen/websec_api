using Client_tech_resversi_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Newtonsoft.Json;

namespace Client_tech_resversi_api.Controllers
{
    [Authorize (Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ReversiContext _context;

        public GamesController(ReversiContext context)
        {
            _context = context;
        }
        
        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetGame(Guid id)
        {
            var game = await _context.Game.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return game.gameboard;
        }
        
        // PUT: api/Games/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutGame(Guid id, Game game)
        {
            var temper = await _context.antiTempering.FindAsync(id);
            if (temper == null)
            {
                return BadRequest();
            }

            var playfield = JsonConvert.DeserializeObject<Playfield>(game.gameboard);
            int puckAmount = 0;

            for (int i = 0; i < playfield.Board.GetLength(0); i++)
            {
                for (int j = 0; j < playfield.Board.GetLength(1); j++)
                {
                    if (playfield.Board[i, j] == 1 || playfield.Board[i, j] == 0) puckAmount++;
                }
            }

            // this would get me fired in production :)
            if ((id != game.gameId || temper == null || (temper.lastMove == null && game.playerOne == null) ||
                 game.playerOne == temper.lastMove || (game.playerTwo == temper.lastMove && temper.lastMove != null) ||
                 puckAmount != temper.puckCount + 1) && temper.state != 'N')
            {
                return BadRequest();
            }

            var attachGame = await _context.Game.FindAsync(id);

            if (attachGame == null || attachGame.startTime == null)
            {
                return BadRequest();
            }

            temper.puckCount++;
            temper.lastMove = game.playerOne ?? game.playerTwo;

            playfield.LastMove = game.playerOne == null ? '+' : '-';

            if (temper.puckCount == 64)
            {
                attachGame = new DetermineWinner().EndGame(attachGame, playfield);
            }
            else
            {
                attachGame.gameboard = JsonConvert.SerializeObject(playfield);
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return AcceptedAtAction("PutGame");
        }
        
        [HttpPut("{id}/out")]
        public async Task<ActionResult> OutOfMoves(Guid id, Game game)
        {
            var savedGame = await _context.Game.FindAsync(id);
            var temper = await _context.antiTempering.FindAsync(id);
            if (savedGame == null || temper == null)
            {
                return BadRequest();
            }

            var savedPlayfield = JsonConvert.DeserializeObject<Playfield>(savedGame.gameboard);

            temper.state = 'N';
            savedPlayfield.Ending = temper.state;

            if (game.playerOne != null) savedPlayfield.P1 = 'X';
            if (game.playerTwo != null) savedPlayfield.P2 = 'X';


            if (savedPlayfield.P1 == 'X' && savedPlayfield.P2 == 'X' || temper.puckCount == 64)
            {
                savedGame = new DetermineWinner().EndGame(savedGame, savedPlayfield);
            }
            else
            {
                savedGame.gameboard = JsonConvert.SerializeObject(savedPlayfield);                
            }
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
            
            return AcceptedAtAction("OutOfMoves");
        }

        [HttpPut("{id}/draw")]
        public async Task<ActionResult> Draw(Guid id, Game game)
        {
            var savedGame = await _context.Game.FindAsync(id);
            if (savedGame == null)
            {
                return BadRequest();
            }

            var savedPlayfield = JsonConvert.DeserializeObject<Playfield>(savedGame.gameboard);

            savedPlayfield.Ending = 'D';
            if (savedPlayfield.P1 == null && game.playerOne != null) savedPlayfield.P1 = 'X';
            if (savedPlayfield.P2 == null && game.playerTwo != null) savedPlayfield.P2 = 'X';
            
            if (savedPlayfield.P1 == 'X' && savedPlayfield.P2 == 'X')
            {
                savedPlayfield.Active = false;
                savedGame.winner = Guid.Empty;
                savedGame.endTime = DateTime.UtcNow;
            }
            
            savedGame.gameboard = JsonConvert.SerializeObject(savedPlayfield);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }

            return AcceptedAtAction("Draw");
        }

        [HttpPut("{id}/nodraw")]
        public async Task<ActionResult> NoDraw(Guid id)
        {
            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return BadRequest();
            }

            var playfield = JsonConvert.DeserializeObject<Playfield>(game.gameboard);
            playfield.P2 = playfield.P1 = playfield.Ending = null;
            
            game.gameboard = JsonConvert.SerializeObject(playfield);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                BadRequest();
            }

            return AcceptedAtAction("NoDraw");
        }

        [HttpPut("{id}/surrender")]
        public async Task<ActionResult> Surrender(Guid id, Game game)
        {
            var savedGame = await _context.Game.FindAsync(id);
            if (savedGame == null)
            {
                return BadRequest();
            }

            savedGame.winner = game.playerOne == null ? savedGame.playerOne : savedGame.playerTwo;
            savedGame.endTime = DateTime.UtcNow;

            var playfield = JsonConvert.DeserializeObject<Playfield>(savedGame.gameboard);
            playfield.Active = false;
            playfield.Ending = 'S';
            playfield.LastMove = game.playerOne == null ? '-' : '+';
            savedGame.gameboard = JsonConvert.SerializeObject(playfield);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                BadRequest();
            }

            return AcceptedAtAction("Surrender");
        }

        [HttpPut("{id}/quit")]
        public async Task<ActionResult> IQuit(Guid id, Game game)
        {
            var savedGame = await _context.Game.FindAsync(id);
            if (savedGame == null)
            {
                return BadRequest();
            }

            var playfield = JsonConvert.DeserializeObject<Playfield>(savedGame.gameboard);

            playfield.Ending = 'Q';
            playfield.Active = false;
            playfield.LastMove = game.playerOne == null ? '+' : '-';
            savedGame.gameboard = JsonConvert.SerializeObject(playfield);
            savedGame.winner = game.playerOne == null ? savedGame.playerOne : savedGame.playerTwo;
            savedGame.endTime = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }

            return AcceptedAtAction("IQuit");
        }


        //join a game (existing or a new one)
        [HttpPost("0/joinGame")]
        public async Task<ActionResult<Game>> JoinGame(Player player)
        {
            var activeGames = await _context.Game.Where(x => x.playerTwo == null && x.endTime == null).ToListAsync();

            Game game = null;

            foreach (var activeGame in activeGames)
            {
                if (activeGame.playerOne == player.playerId || activeGame.playerTwo == player.playerId)
                {
                    if (activeGame.playerOne == player.playerId)
                    {
                        activeGame.playerTwo = null;
                    }
                    else
                    {
                        activeGame.playerOne = null;
                    }

                    return activeGame;
                }

                if (activeGame.playerTwo == null)
                {
                    game = activeGame;
                    break;
                }
            }
            
            if (game != null && game.playerOne != player.playerId)
            {
                _context.Attach(game);
                game.playerTwo = player.playerId;
                game.startTime = DateTime.UtcNow;
                var playfield = JsonConvert.DeserializeObject<Playfield>(game.gameboard);
                playfield.Active = true;
                playfield.PlayerTwo = player.username;
                game.gameboard = JsonConvert.SerializeObject(playfield);

                try
                {
                    await _context.SaveChangesAsync();
                    game.playerOne = null;
                    return CreatedAtAction("JoinGame",  game);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }

            game = new Game{playerOne = player.playerId, gameboard = JsonConvert.SerializeObject(new Playfield{PlayerOne = player.username})};

            try
            {
                _context.Add(game);
                await _context.SaveChangesAsync();

                _context.Add(new antiTempering {gameId = game.gameId, puckCount = 4});
                await _context.SaveChangesAsync();
                
                return CreatedAtAction("JoinGame",  game);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return BadRequest();
        }
        
        private bool GameExists(Guid id)
        {
            return _context.Game.Any(e => e.gameId == id);
        }
    }
}
