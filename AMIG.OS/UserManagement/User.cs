using System;
using System.Text;
using AMIG.OS.Utils;

namespace AMIG.OS.UserSystemManagement
{
    public class User
    {
        public string Username { get; internal set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }

        // Weitere Felder f�r zuk�nftige Erweiterungen
        public DateTime CreatedAt { get; private set; }
        public DateTime LastLogin { get; set; }

        public User(string username, string password, string role, bool isHashed = false)
        {
            Username = username;
            PasswordHash = isHashed ? password : HashPassword(password);
            Role = role;
            CreatedAt = DateTime.Now;
        }

        // Funktion zum �berpr�fen des Passworts
        public bool VerifyPassword(string password)
        {
            Console.WriteLine($"Verifying password. Input: {password}, Hash: {PasswordHash}");
            return PasswordHash == HashPassword(password);

            //return PasswordHash == password;
        }

        // Methode zum Hashen eines Passworts mit SHA256
        private string HashPassword(string input)
        {
            byte[] hashBytes = SHA256.Hash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        // Funktion zum �ndern des Passworts
        public void ChangePassword(string newPassword)
        {
            PasswordHash = HashPassword(newPassword);
            //PasswordHash = newPassword;
        }

        // Funktion zum �ndern der Rolle
        public void ChangeRole(string newRole)
        {
            Role = newRole;
        }

        /*
        public void ChangeName(string newName)
        {
            Username = newName;
        }
        */

    }
}
