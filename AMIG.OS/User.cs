using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;
using System.Text;
using Sys = Cosmos.System;
using System.Security.Cryptography;

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
        // Dictionary zur Verwaltung von Benutzern mit Benutzername, Rolle und Passwort
        private Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();

        // Variable zum Speichern des aktuell eingeloggten Benutzers
        private string loggedInUser = null;

        // Benutzer hinzufügen
        public void AddUser(string username, string role, string password)
        {
            if (!users.ContainsKey(username))
            {
                users.Add(username, new UserInfo(username, role, password)); // Übergebe den Benutzernamen
                Console.WriteLine($"Benutzer {username} mit der Rolle {role} wurde hinzugefügt.");
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
     
        // Benutzer ausloggen
        public void Logout()
        {
            if (loggedInUser != null)
            {
                Console.WriteLine($"Benutzer {loggedInUser} wurde abgemeldet.");
                loggedInUser = null; // Setze den eingeloggten Benutzer zurück
            }
        }

        // Zugriff auf die Informationen des eingeloggten Benutzers
        public void DisplayLoggedInUserInfo(string loggedInUser)
        {
            if (users.ContainsKey(loggedInUser))
            {
                UserInfo userInfo = users[loggedInUser];
                Console.WriteLine($"Benutzer: {userInfo.Name}, Rolle: {userInfo.Role}");
            }
            else
            {
                Console.WriteLine("Benutzer existiert nicht.");
            }
        }

        // Rolle des aktuell eingeloggten Benutzers ändern
        public void ChangeLoggedInUserRole(string newRole)
        {
            if (loggedInUser != null && users.ContainsKey(loggedInUser))
            {
                users[loggedInUser].Role = newRole;
                Console.WriteLine($"Die Rolle von {loggedInUser} wurde auf {newRole} geändert.");
            }
            else
            {
                Console.WriteLine("Kein Benutzer eingeloggt.");
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
    }

}










