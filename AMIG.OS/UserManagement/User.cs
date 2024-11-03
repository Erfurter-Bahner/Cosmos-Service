using System;
using System.Collections.Generic;
using System.Text;
using AMIG.OS.Utils;

namespace AMIG.OS.UserSystemManagement
{
    public class User
    {
        public string Username { get; internal set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }
       
        // Weitere Felder für zukünftige Erweiterungen
        public string CreatedAt { get; internal set; }
        public string LastLogin { get; set; }
        public string Group { get; private set; }
        public Dictionary<string, string> Permissions { get;  set; } // Neue Eigenschaft
        public User(string username, string password, string role , string created, bool isHashed = false)
        {
            Username = username;
            PasswordHash = isHashed ? password : HashPassword(password);
            Role = role;
            CreatedAt = created; // Falls `createdAt` null ist, wird `DateTime.Now` verwendet.

            // Standardberechtigungen nur setzen, wenn keine Berechtigungen vorhanden sind

            Permissions = new Dictionary<string, string>();
            SetDefaultPermissions();
            
        }

        private void SetDefaultPermissions()
        {
            if (Role == "Admin")
            {
                Permissions["CreateUser"] = "true";
                Permissions["DeleteUser"] = "true";
                Permissions["ChangeName"] = "true";
                Permissions["ChangePassword"] = "true";
            }
            else if (Role == "Standard")
            {
                Permissions["CreateUser"] = "false";
                Permissions["DeleteUser"] = "false";
                Permissions["ChangeName"] = "true";
                Permissions["ChangePassword"] = "true";
            }
        }

        public void DisplayPermissions()
        {
            Console.WriteLine($"Berechtigungen für Benutzer: {Username}");
            foreach (var permission in Permissions)
            {
                Console.WriteLine($"- {permission.Key}: {(permission.Value == "true" ? "Erlaubt" : "Nicht erlaubt")}");
            }
        }

        public bool HasPermission(string permission)
        {
            return Permissions.ContainsKey(permission) && Permissions[permission] == "true";
        }

        public void ChangePermission(string permission, string value)
        {
            Permissions[permission] = value;
        }

        // Funktion zum Überprüfen des Passworts
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

        // Funktion zum Ändern des Passworts
        public void ChangePassword(string newPassword)
        {
            PasswordHash = HashPassword(newPassword);
            //PasswordHash = newPassword;
        }

        // Funktion zum Ändern der Rolle
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
