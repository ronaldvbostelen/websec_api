using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Client_tech_resversi_api.Models.Non_DB_models;

namespace Client_tech_resversi_api.Models.Principal
{
    public class User : RegisterUser
    {
        public int Id { get; set; }

        public string Salt { get; set; }

        public User()
        {
            UserClaims = new HashSet<UserClaim>();
            UserRoles = new HashSet<UserRole>();
        }
        public virtual UserAccount UserAccount { get; set; }
        public virtual UserLastChanged UserLastChanged { get; set; }
        public virtual ICollection<UserClaim> UserClaims { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
