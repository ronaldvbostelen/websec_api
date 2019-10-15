using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace Client_tech_resversi_api.Assets
{
    public class HashGenerator : IHashGenerator
    {
        public string GenerateHash(string seed)
        {
            using (var hasher = SHA256.Create())
            {
                var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(seed));

                return Convert.ToBase64String(hash);
            }
        }

        public string GenerateURlSaveHash(string seed)
        {
            using (var hasher = SHA512.Create())
            {
                var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(seed));

                return WebEncoders.Base64UrlEncode(hash);
            }
        }
    }
}