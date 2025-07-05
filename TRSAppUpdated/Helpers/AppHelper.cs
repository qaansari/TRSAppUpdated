using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace TRSAppUpdated.Helpers
{
    public static class AppHelper
    {
        public static List<string> Cities = new List<string> { "Karachi","Lahore","Islamabad","Peshawar","Quetta" };
        public static string HashWithSalt(this string password)
        {
            string saltstring = "CHYzqeN4plZekNC88Umm1Q==";
            // Generate a 128-bit salt using a sequence of
            // cryptographically strong random bytes.
            byte[] salt = Encoding.ASCII.GetBytes(saltstring);

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
