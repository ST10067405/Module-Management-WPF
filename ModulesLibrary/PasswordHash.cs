using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ModulesLibrary
{
    public class PasswordHash
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sHA256 = SHA256.Create())
            {
                //After using SHA256, saving the bytes into a byte array
                byte[] hashBytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Putting the hashed password bytes back together.
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                //returning formed hashed password
                return builder.ToString();
            }
        }
    }
}
