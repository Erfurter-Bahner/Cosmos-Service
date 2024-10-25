using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;
using System.Text;
using Sys = Cosmos.System;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace AMIG.OS
{
    // Klasse zur Verwaltung von Benutzerinformationen
    public class UserInfo
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }

        public UserInfo(string name, string role, string password)
        {
            Name = name;
            Role = role;
            Password = password;
        }
    }
    public class UserManagement
    {
        private const string FilePath = @"0:\users.txt"; // Datei zur Speicherung
        // Dictionary zur Verwaltung von Benutzern mit Benutzername, Rolle und Passwort
        private Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();

        public UserManagement()
        {
            InitializeTestUsers();
            LoadUsers(); // Lädt Benutzer beim Start
        }

        // Benutzer hinzufügen
        public void AddUser(string username, string role, string password)
        {
            if (!users.ContainsKey(username))
            {
                users.Add(username, new UserInfo(username, role, password)); // Übergebe den Benutzernamen
                Console.WriteLine($"Benutzer {username} mit der Rolle {role} wurde hinzugefügt.");
                SaveUsers(); // Speichert nach jedem Hinzufügen
            }
            else
            {
                Console.WriteLine($"Benutzer {username} existiert bereits.");
            }
        }
        // Überprüfen, ob ein Benutzer existiert
        public bool UserExists(string username)
        {
            return users.ContainsKey(username);
        }

        // Rolle eines Benutzers abrufen
        public string GetUserRole(string username)
        {
            if (users.ContainsKey(username))
            {
                return users[username].Role;
            }
            else
            {
                return "Benutzer existiert nicht.";
            }
        }

        // Benutzername abrufen (jetzt über die UserInfo-Klasse)
        public string GetUsername(string username)
        {
            if (users.ContainsKey(username))
            {
                return users[username].Name; // Hier verwenden wir die Name-Eigenschaft
            }
            else
            {
                return "Benutzer existiert nicht.";
            }
        }

        // Passwort eines Benutzers überprüfen
        public bool VerifyPassword(string username, string password)
        {
            if (users.ContainsKey(username))
            {
                return users[username].Password == password;
            }
            else
            {
                return false;
            }
        }

        // Rolle eines Benutzers ändern
        public void ChangeUserRole(string username, string newRole)
        {
            if (users.ContainsKey(username))
            {
                users[username].Role = newRole;
                Console.WriteLine($"Die Rolle von {username} wurde auf {newRole} geändert.");
                SaveUsers();
            }
            else
            {
                Console.WriteLine("Benutzer existiert nicht.");
            }
        }

        // Benutzer entfernen
        public void RemoveUser(string username)
        {
            if (users.ContainsKey(username))
            {
                users.Remove(username);
                Console.WriteLine($"Benutzer {username} wurde entfernt.");
            }
            else
            {
                Console.WriteLine("Benutzer existiert nicht.");
            }
        }

        public void DisplayAllUsers()
        {
            foreach (var kvp in users)
            {
                string username = kvp.Key;
                UserInfo userInfo = kvp.Value;
                Console.WriteLine($"Benutzer: {username}, Rolle: {userInfo.Role}");
            }
        }

        // Zugriff auf einen Benutzer
        public void GetUserInfo(string username)
        {
            if (users.ContainsKey(username))
            {
                UserInfo userInfo = users[username];
                Console.WriteLine($"Benutzer: {username}, Rolle: {userInfo.Role}, Passwort: {userInfo.Password}");
            }
            else
            {
                Console.WriteLine("Benutzer existiert nicht.");
            }
        }

        // Benutzer Login-Anforderung
        public bool Login(string username, string password)
        {
            if (users.ContainsKey(username))
            {
                if (users[username].Password == password)
                {
                    Console.WriteLine($"Login erfolgreich. Willkommen {username}!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Falsches Passwort.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Benutzer existiert nicht.");
                return false;
            }
        }

        // Benutzername ändern(für den aktuell eingeloggten Benutzer)
        // Benutzername ändern
        public bool ChangeUsername(string oldUsername, string newUsername)
        {
            if (users.ContainsKey(oldUsername))
            {
                if (!users.ContainsKey(newUsername)) // Überprüfen, ob der neue Benutzername bereits existiert
                {
                    UserInfo userInfo = users[oldUsername];
                    users.Remove(oldUsername); // Alten Benutzernamen entfernen
                    users.Add(newUsername, userInfo); // Neuen Benutzernamen hinzufügen
                    Console.WriteLine($"Benutzername wurde von {oldUsername} auf {newUsername} geändert.");
                    SaveUsers();
                    return true;
                }
                else
                {
                    Console.WriteLine("Der neue Benutzername existiert bereits.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Benutzer existiert nicht.");
                return false;
            }
        }

        private void SaveUsers()
        {
            try
            {
                // Überprüfe, ob die Datei bereits existiert und lösche sie ggf.
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }

                // Erstellen und Schreiben in die Datei
                using (var stream = File.Create(FilePath))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var user in users)
                    {
                        var userInfo = user.Value;
                        writer.WriteLine($"{user.Key},{userInfo.Role},{userInfo.Password}");
                    }
                }
                Console.WriteLine("Benutzerdaten erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Benutzer: {ex.Message}");
            }
        }

        //fileio
        public void LoadUsers()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    using (var stream = File.OpenRead(FilePath))
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split(',');
                            if (parts.Length == 3)
                            {
                                string username = parts[0];
                                string role = parts[1];
                                string password = parts[2];
                                users[username] = new UserInfo(username, role, password);
                            }
                        }
                    }
                    Console.WriteLine("Benutzerdaten erfolgreich geladen.");
                }
                else
                {
                    Console.WriteLine("Benutzerdaten-Datei existiert nicht.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Benutzer: {ex.Message}");
            }
        }

        public void InitializeTestUsers()
        {
            if (!File.Exists(FilePath))  // Prüfen, ob die Datei existiert
            {
                Console.WriteLine("Erstelle Testbenutzer...");

                // Testbenutzer hinzufügen
                users.Add("User1", new UserInfo("User1", "Standard", "password123"));
                users.Add("User2", new UserInfo("User2", "Admin", "adminPass"));

                // Speichern der Benutzer in die Datei
                SaveUsers();
            }
            else
            {
                Console.WriteLine("Benutzerdaten-Datei existiert bereits. Überspringe Testbenutzer-Erstellung.");
            }
        }

    }
}










