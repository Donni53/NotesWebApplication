using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace NotesWebApplication.Helpers
{
    public static class Cryptography
    {
        public static string GetHash(string data, int length)
        {
            // generate a 128-bit salt using a secure PRNG
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                data,
                salt,
                KeyDerivationPrf.HMACSHA1,
                1000,
                length));
            return hashed;
        }
    }
}