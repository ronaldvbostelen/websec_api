using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Models.Principal;

namespace Client_tech_resversi_api.Models.Non_DB_models
{
    public class UserOverviewLight
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool Verified { get; set; }
        public List<UserRole> UserRoles { get; set; }

        public UserOverviewLight()
        {
            UserRoles = new List<UserRole>();
        }
    }
}
