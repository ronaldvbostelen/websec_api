using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client_tech_resversi_api.Models.Non_DB_models
{
    public class TwoFaLoginUser : LoginUser
    {
        public string Code { get; set; }
    }
}
