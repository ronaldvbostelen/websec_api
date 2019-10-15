using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Models;
using Newtonsoft.Json;

namespace Client_tech_resversi_api
{
    public class DetermineWinner
    {
        public Game EndGame(Game game, Playfield playfield)
        {
            int amountofZero = 0;
            int amountofOnes = 0;

            for (int i = 0; i < playfield.Board.GetLength(0); i++)
            {
                for (int j = 0; j < playfield.Board.GetLength(1); j++)
                {
                    if (playfield.Board[i, j] == 0) amountofZero++;
                    if (playfield.Board[i, j] == 1) amountofOnes++;
                }
            }

            if (amountofOnes == amountofZero)
            {
                game.winner = Guid.Empty;
                playfield.LastMove = 'X';
                playfield.Ending = 'T';
            }
            else
            {
                game.winner = amountofOnes < amountofZero ? game.playerOne : game.playerTwo;
                playfield.LastMove = amountofOnes < amountofZero ? '-' : '+';
                playfield.Ending = 'F';
            }

            playfield.Active = false;
            game.endTime = DateTime.UtcNow;

            game.gameboard = JsonConvert.SerializeObject(playfield);

            return game;
        }
    }
}
