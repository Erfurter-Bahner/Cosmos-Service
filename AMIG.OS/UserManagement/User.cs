using System;
using System.Security.Cryptography;
using System.Text;

namespace AMIG.OS.UserSystemManagement
{
    public enum UserRole
    {
        Standard,
        Admin
    }

    public class User
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }

        // Weitere Felder für zukünftige Erweiterungen
        public DateTime CreatedAt { get; private set; }
        public DateTime LastLogin { get; set; }

        public User(string username, string password, UserRole role)
        {
            Username = username;
            //PasswordHash = HashPassword(password);
            PasswordHash = password;
            Role = role;
            CreatedAt = DateTime.Now;
        }

        // Funktion zum Überprüfen des Passworts
        public bool VerifyPassword(string password)
        {
            //return PasswordHash == HashPassword(password);
            return PasswordHash == password;
        }


        // Private Methode zur Passwort-Hashing
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Funktion zum Ändern des Passworts
        public void ChangePassword(string newPassword)
        {
            //PasswordHash = HashPassword(newPassword);
            PasswordHash = newPassword;
        }

        // Funktion zum Ändern der Rolle
        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
        }
    }
}
