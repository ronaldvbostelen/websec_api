using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Client_tech_resversi_api.Models.Non_DB_models
{
    public class ResetUserPassword
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string ResetCode { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password does not meet the requirements.")]
        [DataType(DataType.Password)]
        public string PassOne { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password does not meet the requirements.")]
        [DataType(DataType.Password)]
        public string PassTwo { get; set; }
    }
}
