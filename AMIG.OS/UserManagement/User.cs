using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using AMIG.OS.Utils;

namespace AMIG.OS.UserSystemManagement
{
    public class User
    {
        // Basisinformationen des Benutzers
        public string Username { get; internal set; }
        public string PasswordHash { get; private set; }
        public string CreatedAt { get; internal set; } // Zeitpunkt der Erstellung des Benutzerkontos
        public string LastLogin { get; set; } // Zeitpunkt der letzten Anmeldung
        // Rollen und Berechtigungen des Benutzers
        public List<Role> Roles { get; set; } // Zugewiesene Rollen
        public HashSet<string> Permissions { get; private set; } // Individuelle Berechtigungen
        //public HashSet<string> CombinedPermissions { get; private set; } // Kombinierte Berechtigungen aus Rollen und individuellen Berechtigungen
        public HashSet<string> CustomPermissions { get; private set; }
        // Konstruktor: Erstellt einen neuen Benutzer und initialisiert die Felder
        public User(string username, string password, bool isHashed = false, List<Role> roles = null, HashSet<string> permissions = null, HashSet<string> custompermissions = null, string created = null)
        {
            Username = username;
            PasswordHash = isHashed ? password : HashPassword(password);
            CreatedAt = created ?? DateTime.Now.ToString(); // Aktuelle Zeit, falls `created` null ist
            Roles = roles ?? new List<Role>();
            Permissions = permissions ?? new HashSet<string>();
            CustomPermissions = custompermissions ?? new HashSet<string>();
            //CombinedPermissions = new HashSet<string>();// bei jedem start
            UpdateCombinedPermissions(); // Initialisiere die kombinierten Berechtigungen
        }


        //// Aktualisiert die kombinierten Berechtigungen aus Rollen und individuellen Berechtigungen
        //public void UpdateCombinedPermissions()
        //{
        //    CombinedPermissions.Clear(); //lösche alles heraus

        //    foreach (var role in Roles)
        //    {
        //        CombinedPermissions.UnionWith(role.Permissions); //tu für jede rolle die Permissions in cP
        //    }

        //    // Individuelle Berechtigungen des Benutzers hinzufügen
        //    CombinedPermissions.UnionWith(Permissions); 
        //}

        public void UpdateCombinedPermissions()
        {
            Permissions.Clear(); //lösche alles heraus

            foreach (var role in Roles)
            {
                Permissions.UnionWith(role.Permissions); //tu für jede rolle die Permissions in cP
            }
            Permissions.UnionWith(CustomPermissions);
        }

        // Überprüft, ob der Benutzer eine bestimmte Berechtigung hat
        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        // Fügt eine Rolle hinzu und aktualisiert die kombinierten Berechtigungen
        public void AddRole(Role role)
        {
            if (!Roles.Contains(role))
            {
                Roles.Add(role);
                UpdateCombinedPermissions();
            }

        }

        // Entfernt eine Rolle und aktualisiert die kombinierten Berechtigungen
        public void RemoveRole(Role role)
        {
            if (Roles.Remove(role))
            {
                UpdateCombinedPermissions();
            }
        }

        // Fügt eine individuelle Berechtigung hinzu und aktualisiert die kombinierten Berechtigungen
        public void AddPermission(string permission)
        {
            if (!Permissions.Contains(permission))
            {
                CustomPermissions.Add(permission);
                UpdateCombinedPermissions();
            }
        }

        // Entfernt eine individuelle Berechtigung und aktualisiert die kombinierten Berechtigungen
        public void RemovePermission(string permission)
        {
            Permissions.Remove(permission);
            UpdateCombinedPermissions();
        }

        // Überprüft, ob das übergebene Passwort mit dem gespeicherten Hash übereinstimmt
        public bool VerifyPassword(string password)
        {
            Console.WriteLine($"Verifying password. Input: {password}, Hash: {PasswordHash}");
            return PasswordHash == HashPassword(password);
        }

        // Hash-Funktion zum Erstellen eines SHA256-Hashes für das Passwort
        private string HashPassword(string input)
        {
            byte[] hashBytes = SHA256.Hash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        // Ändert das Passwort und aktualisiert den Hash-Wert
        public void ChangePassword(string newPassword)
        {
            PasswordHash = HashPassword(newPassword);
        }
    }
}
