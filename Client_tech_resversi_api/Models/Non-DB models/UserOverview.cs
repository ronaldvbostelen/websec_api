using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Client_tech_resversi_api.Models.Principal;

namespace Client_tech_resversi_api.Models.Non_DB_models
{
    public class UserOverview : UserOverviewLight
    {
        public string ScreenName { get; set; }
        public string Email { get; set; }
        public UserAccount UserAccount { get; set; }
        public List<UserClaim> UserClaims { get; set; }

        public UserOverview()
        {
            UserAccount = new UserAccount();
            UserClaims = new List<UserClaim>();
        }
    }
}
