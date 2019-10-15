namespace Client_tech_resversi_api.Assets.Interfaces
{
    public interface IPasswordManager
    {
        bool GenerateSaltAndPasswordHash(string password, out string hash, out string salt);
        bool VerifyPassword(string enteredPw, string hash, string salt);
    }
}