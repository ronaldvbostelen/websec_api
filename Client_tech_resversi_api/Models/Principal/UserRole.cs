using System;
using System.Collections.Generic;
using System.Linq;
using Client_tech_resversi_api.Models.Interfaces;

namespace Client_tech_resversi_api.Models.Principal
{
    public class UserRole : BaseModel, IPossession
    {
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}
