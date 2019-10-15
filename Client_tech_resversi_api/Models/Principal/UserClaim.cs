using Client_tech_resversi_api.Models.Interfaces;

namespace Client_tech_resversi_api.Models.Principal
{
    public class UserClaim : BaseModel, IPossession
    {
        public int UserId { get; set; }
        public string Claim { get; set; }
        public string Value { get; set; }
        public string Issuer { get; set; }

    }
}
