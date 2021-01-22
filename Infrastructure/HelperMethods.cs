using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Infrastructure
{
    static public class HelperMethods
    {
       

        public static class ErrorMessage
        {
            public static string  InvalidToken = "invalid jwt token";
            public static string FailedDbOperation = "failed to access database";
            public static string UserDoesNotExist = "user does not exist ";
            public static string EntityDoesNotExist = " data was not found ";

            

        }

        

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
    }
}
