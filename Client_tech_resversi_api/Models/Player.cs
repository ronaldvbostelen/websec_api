using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_tech_resversi_api.Models
{
    public partial class Player
    {
        public Player()
        {
            GameplayerOneNavigation = new HashSet<Game>();
            GameplayerTwoNavigation = new HashSet<Game>();
            antiTempering = new HashSet<antiTempering>();
        }

        public Guid playerId { get; set; }
        [Required]
        [StringLength(255)]
        public string username { get; set; }
        [StringLength(255)]
        public string screenname { get; set; }
        [Required]
        [StringLength(2048)]
        public string password { get; set; }
        [Required]
        [StringLength(2048)]
        public string salt { get; set; }
        [Required]
        [StringLength(255)]
        public string email { get; set; }
        public bool verified { get; set; }
        public int role { get; set; }
        [Required]
        [StringLength(1)]
        public string status { get; set; }
        public bool deleted { get; set; }
        public int loginAttempt { get; set; }

        [InverseProperty("playerOneNavigation")]
        public virtual ICollection<Game> GameplayerOneNavigation { get; set; }
        [InverseProperty("playerTwoNavigation")]
        public virtual ICollection<Game> GameplayerTwoNavigation { get; set; }
        [InverseProperty("lastMoveNavigation")]
        public virtual ICollection<antiTempering> antiTempering { get; set; }
    }
}