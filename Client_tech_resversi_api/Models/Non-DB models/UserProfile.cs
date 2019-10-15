using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client_tech_resversi_api.Models.Non_DB_models
{
    public class UserProfile
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        [StringLength(255, MinimumLength = 8, ErrorMessage = "Screenname should be minimal 8 charcters long but not more then 255")]
        public string ScreenName { get; set; }

        [StringLength(255, ErrorMessage = "Emailaddress is too long")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public bool TwoFa { get; set; }
    }
}
