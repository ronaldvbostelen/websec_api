using Client_tech_resversi_api.Models.Interfaces;

namespace Client_tech_resversi_api.Models.Principal
{
    public class UserAccount : BaseModel, IPossession
    {
        public int UserId { get; set; }
        public bool Verified { get; set; }
        public char Status { get; set; }
        public bool TwoFactorAuth { get; set; }
        public string LastTimeLoggedIn { get; set; }
        public string LastTimeLoggedInFrom { get; set; }
        public int LoginAttempts { get; set; }
        public string ActivationKey { get; set; }
        public string RecoverKey { get; set; }
        public int RecoverAttempts { get; set; }
        public string SecurityHash { get; set; }
        public string TotpSecret { get; set; }
    }
}
