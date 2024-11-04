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
        public string CreatedAt { get; internal set; }
        public string LastLogin { get; set; }


        //Neue Berechtigungsimplementierung
        public HashSet<Role> Roles { get; private set; }
        public HashSet<string> Permissions { get; private set; }

        private HashSet<string> combinedPermissions = new HashSet<string>();
        
        
        public User(string username, string password, bool isHashed = false, HashSet<Role> roles = null, HashSet<string> permissions = null, string created = null)
        {
            Username = username;
            PasswordHash = isHashed ? password : HashPassword(password);
            CreatedAt = created ?? DateTime.Now.ToString(); // Falls `createdAt` null ist, wird `DateTime.Now` verwendet. Vllt noch überarbeiten
            
            Roles = roles ?? new HashSet<Role>();
            Permissions = permissions ?? new HashSet<string>();
            
            UpdateCombinedPermissions();
        }

        public void AddRole(Role role)
        {
            Roles.Add(role);
            UpdateCombinedPermissions();
        }

        public void RemoveRole(Role role)
        {
            Roles.Remove(role);
            UpdateCombinedPermissions();
        }

        public void AddPermission(string permission)
        {
            Permissions.Add(permission);
        }

        public void RemovePermission(string permission)
        {
            Permissions.Remove(permission);
        }

        private void UpdateCombinedPermissions()
        {
            combinedPermissions.Clear();

            // Berechtigungen der Rollen hinzufügen
            foreach (var role in Roles)
            {
                combinedPermissions.UnionWith(role.Permissions);
            }

            // Individuelle Berechtigungen des Users hinzufügen
            combinedPermissions.UnionWith(Permissions);
        }

        public bool HasPermission(string permission)
        {
            return combinedPermissions.Contains(permission);
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
    }
}
