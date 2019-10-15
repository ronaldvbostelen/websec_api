using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_tech_resversi_api.Models
{
    public partial class Game
    {
        public Guid gameId { get; set; }
        public Guid? playerOne { get; set; }
        public Guid? playerTwo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? startTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? endTime { get; set; }
        [StringLength(2056)]
        public string gameboard { get; set; }
        public Guid? winner { get; set; }

        [ForeignKey("playerOne")]
        [InverseProperty("GameplayerOneNavigation")]
        public virtual Player playerOneNavigation { get; set; }
        [ForeignKey("playerTwo")]
        [InverseProperty("GameplayerTwoNavigation")]
        public virtual Player playerTwoNavigation { get; set; }
    }
}