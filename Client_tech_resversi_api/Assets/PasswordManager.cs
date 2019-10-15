using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Interfaces;

namespace Client_tech_resversi_api.Assets
{
    public class PasswordManager : IPasswordManager
    {
        public bool GenerateSaltAndPasswordHash(string password, out string hash, out string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                hash = salt = null;
                return false;
            }

            var saltBytes = new byte[64];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);

            var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA512);

            salt = Convert.ToBase64String(saltBytes);
            hash = Convert.ToBase64String(rfc2898.GetBytes(256));

            return !string.IsNullOrWhiteSpace(hash) && !string.IsNullOrWhiteSpace(salt);
        }

        public bool VerifyPassword(string enteredPw, string hash, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var rfc2898 = new Rfc2898DeriveBytes(enteredPw, saltBytes, 10000, HashAlgorithmName.SHA512);
            return Convert.ToBase64String(rfc2898.GetBytes(256)) == hash;
        }
    }
}
