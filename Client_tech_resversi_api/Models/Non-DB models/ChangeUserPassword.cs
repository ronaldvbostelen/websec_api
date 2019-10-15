using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client_tech_resversi_api.Models.Non_DB_models
{
    public class ChangeUserPassword
    {
        public int UserId { get; set; }
        
        [Required]
        [MinLength(8, ErrorMessage = "Password does not meet the requirements.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password does not meet the requirements.")]
        [DataType(DataType.Password)]
        public string PasswordOne { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password does not meet the requirements.")]
        [DataType(DataType.Password)]
        public string PasswordTwo { get; set; }
    }
}
