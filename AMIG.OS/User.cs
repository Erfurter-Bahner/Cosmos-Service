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
        public string Name { get; set; } // Füge die Name-Eigenschaft hinzu
        public string Role { get; set; }
        public string Password { get; set; }

        public UserInfo(string name, string role, string password)
        {
            Name = name; // Setze den Benutzernamen
            Role = role;
            Password = password;
        }
    }
    public class UserManagement
    {
        // Dictionary zur Verwaltung von Benutzern mit Benutzername, Rolle und Passwort
        private Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();

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


    }

}















/*
// Hash-Funktion, die den SHA256-Algorithmus verwendet funkionniert nicht 
private string HashPassword(string input)
{
    if (string.IsNullOrEmpty(input))
    {
        throw new ArgumentException("Das Passwort darf nicht leer sein.", nameof(input));
    }

    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }
}

// Methode zum Abrufen des gehashten Passworts
public string GetHashedPassword()
{
    return Password;
}




//verifyHash
public bool VerifyHash(string inputPassword)
{
    string hashedInput = HashPassword(inputPassword); // Hash das eingegebene Passwort
    return Password == hashedInput; // Vergleiche mit dem gespeicherten gehashten Passwort
}

//passwort vorraussetzungen

*/







