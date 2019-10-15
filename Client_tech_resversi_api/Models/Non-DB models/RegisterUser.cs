using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Client_tech_resversi_api.Models.Non_DB_models
{
    public class RegisterUser
    {
        [Required]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Username must be with a minimum length of 8.")]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "No white space or specialchars in username")]
        public string Name { get; set; }

        [StringLength(255, MinimumLength = 8, ErrorMessage = "Screenname must be with a minimum length of 8, but not greater then 255.")]
        public string ScreenName { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password does not meet the requirements.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
