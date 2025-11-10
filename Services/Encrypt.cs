using System.Security.Cryptography;
using System.Text;

namespace AlmacenWeb.Services
{
    
    public class Encrypt
    {
        
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        
        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            
            string hashedEnteredPassword = HashPassword(enteredPassword);
            
            return string.Equals(hashedEnteredPassword, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}