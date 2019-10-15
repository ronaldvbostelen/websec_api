using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Timers;
using Client_tech_resversi_api.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Client_tech_resversi_api
{
    public class Reconnect : IReconnect
    {
        private readonly ReversiContext _context;

        private Guid gameId;
//        private Dictionary<Timer,Guid> Timers { get; }

        public Reconnect()
        {
            _context = new ReversiContext();
        }

//        public Reconnect(ReversiContext context)
//        {
//            _context = context;
//            Timers = new Dictionary<Timer, Guid>();
//        }

        public void SetGame(Guid id)
        {
            gameId = id;
            new Timer {Enabled = true, AutoReset = false, Interval = 6000}.Elapsed += OnTimerElapsed;
//            timer.Elapsed += OnTimerElapsed;
//            Timers.Add(timer,id);
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs eventArgs)
        {
            var game = await _context.Game.FindAsync(gameId);
            if (game == null) return;

            var playfield = JsonConvert.DeserializeObject<Playfield>(game.gameboard);
            if (playfield.Ending == 'Q')
            {
                playfield.Active = false;
                game.endTime = DateTime.UtcNow;
                game.winner = playfield.LastMove == '-' ? game.playerTwo : game.playerOne;
                game.gameboard = JsonConvert.SerializeObject(playfield);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }


        private async Task UpdateGame()
        {
            var game = await _context.Game.FindAsync(gameId);
            if (game == null) return;

            var playfield = JsonConvert.DeserializeObject<Playfield>(game.gameboard);
            if (playfield.Ending == 'Q')
            {
                playfield.Active = false;
                game.endTime = DateTime.UtcNow;
                game.winner = playfield.LastMove == '-' ? game.playerTwo : game.playerOne;
                game.gameboard = JsonConvert.SerializeObject(playfield);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
