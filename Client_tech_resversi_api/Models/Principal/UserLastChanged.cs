using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client_tech_resversi_api.Models.Principal
{
    public class UserLastChanged : BaseModel
    {
        public int UserId { get; set; }
        public string DateTimeChanged { get; set; }

    }
}
