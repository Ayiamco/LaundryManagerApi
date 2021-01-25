using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LaundryApi.Infrastructure
{
    static public class HelperMethods
    {
       

        public static class ErrorMessage
        {
            public const string  InvalidToken = "invalid jwt token";
            public const  string FailedDbOperation = "failed to access database";
            public const  string UserDoesNotExist = "user does not exist ";
            public const string EntityDoesNotExist = "data was not found ";
            public const string UsernameAlreadyExist = "user email already exist";
            public const string ServiceAlreadyExist = "service already exist";
            public const string LinkExpired = "link has expired";
            public const string InCorrectPassword = "password is incorrect";
            public const string InvalidModel = "reqeust body is invalid";



            

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

        public static string GenerateRandomString(int size)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[size];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
    }
}
