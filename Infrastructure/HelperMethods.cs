using LaundryApi.Entites;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LaundryApi.Models;
using Microsoft.Extensions.Configuration;

namespace LaundryApi.Infrastructure
{
    public static class HelperMethods
    {
        public  static IConfiguration config { get; set; }
        
        public static string HashPassword(string password)
        {
            //byte[] salt = new byte[128 / 8];
            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(salt);
            //}
            //string parsedSalt = Convert.ToBase64String(salt);
            byte[] salt = {68,81,72,65,101,67,90,98,106
                    ,76,109,117,112,105,77,52,112,49,83,72,68,119,61,61};


            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public static string GenerateRandomString(int size)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[size];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);

            return finalString;
        }

        public static string GetResetLink(string username, string roleId = "")
        {
            string linkId = GenerateRandomString(10);
            linkId = roleId + linkId;
            string userClaim = HashPassword(username);
            linkId += userClaim;  
            return linkId;
        }

        
       
    }
}
