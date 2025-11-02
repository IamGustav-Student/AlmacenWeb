using System.Security.Cryptography;
using System.Text;

namespace AlmacenWeb.Services
{
    // Servicio para encriptar y verificar contraseñas
    public class Encrypt
    {
        // Crea un hash de la contraseña
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Convertir la contraseña a bytes
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Convertir los bytes hasheados a un string
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Verifica si la contraseña ingresada coincide con el hash
        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Hashea la contraseña ingresada
            string hashedEnteredPassword = HashPassword(enteredPassword);
            // Compara los hashes
            return string.Equals(hashedEnteredPassword, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}