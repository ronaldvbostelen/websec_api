using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_tech_resversi_api.Models
{
    public partial class antiTempering
    {
        [Key]
        public Guid gameId { get; set; }
        public Guid? lastMove { get; set; }
        public int? puckCount { get; set; }
        public string test { get; set; }
        public char state { get; set; }
        [ForeignKey("lastMove")]
        [InverseProperty("antiTempering")]
        public virtual Player lastMoveNavigation { get; set; }

    }
}