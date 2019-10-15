using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Client_tech_resversi_api.Models
{
    public class Playfield
    {
        [JsonProperty]
        public int?[,] Board { get; set; }
        [JsonProperty]
        public bool? Active { get; set; }
        [JsonProperty]
        public char LastMove { get; set; }
        [JsonProperty]
        public char? Ending { get; set; }
        [JsonProperty]
        public char? P1 { get; set; }
        [JsonProperty]
        public char? P2 { get; set; }
        [JsonProperty]
        public string PlayerOne { get; set; }
        [JsonProperty]
        public string PlayerTwo { get; set; }

        public Playfield()
        {
            Board = new int?[8,8];

            Board[3, 3] = 1; // < start  //
            Board[3, 4] = 0; // <  posi- //
            Board[4, 3] = 0; // <    ti- //
            Board[4, 4] = 1; // <   ons  //
            
            LastMove = '+';
        }
    }
}
