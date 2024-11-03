using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AMIG.OS.Utils;

namespace AMIG.OS.UserSystemManagement
{
    public class UserRepository
    {
        public Dictionary<string, User> users = new Dictionary<string, User>();
        private readonly string dataFilePath = @"0:\user.txt"; // Pfad zur Benutzerdaten-Datei

        public UserRepository()
        {
            InitializeTestUsers();
            //LoadUsers(); // Lädt Benutzer beim Erstellen der Klasse
        }

        public void SaveUsers()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    File.Delete(dataFilePath);
                }

                using (var stream = File.Create(dataFilePath))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var user in users.Values)
                    {
                        string permissionsString = string.Empty;

                        if (user.Permissions != null && user.Permissions.Count > 0)
                        {
                            foreach (var permission in user.Permissions)
                            {
                                permissionsString += $"{permission.Key}:{permission.Value};";
                            }

                            // Entferne das letzte Semikolon, falls vorhanden
                            if (permissionsString.EndsWith(";"))
                            {
                                permissionsString = permissionsString.Substring(0, permissionsString.Length - 1);
                            }
                        }

                        writer.WriteLine($"{user.Username},{user.PasswordHash},{user.Role},{user.CreatedAt},{user.LastLogin},{permissionsString}");
                    }
                }

                Console.WriteLine("Benutzerdaten erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Benutzer: {ex.Message}");
            }
        }

        public void LoadUsers()
        {
            if (!File.Exists(dataFilePath)) return;

            using (var reader = new StreamReader(dataFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 6)
                    {
                        string username = parts[0];
                        string password = parts[1];
                        string role = parts[2];
                        string createdAt = parts[3];
                        string lastLogin = parts[4];
                        var permissions = new Dictionary<string, string>(); // Verwende Dictionary<string, string>

                        // Berechtigungen laden
                        foreach (var permissionString in parts[5].Split(';'))
                        {
                            var permissionParts = permissionString.Split(':');
                            if (permissionParts.Length == 2)
                            {
                                permissions[permissionParts[0]] = permissionParts[1];
                            }
                            else
                            {
                                Console.WriteLine($"Ungültige Berechtigung: {permissionString} - überspringe diese Berechtigung.");
                            }
                        }

                        var user = new User(username, password, role, createdAt, isHashed: true)
                        {
                            LastLogin = lastLogin,
                            Permissions = permissions // Bestehende Berechtigungen aus der Datei setzen
                        };
                        users[username] = user;
                    }
                }
            }

            Console.WriteLine("Benutzerdaten erfolgreich geladen.");
        }

        public void InitializeTestUsers()
        {
            bool addTestUsers = false;

            // Prüfen, ob die Datei existiert
            if (!File.Exists(dataFilePath))
            {
                Console.WriteLine("Benutzerdaten-Datei existiert nicht. Erstelle Testbenutzer...");
                addTestUsers = true;
            }
            else
            {
                // Datei existiert, aber prüfen, ob sie Benutzer enthält
                LoadUsers(); // Benutzer aus Datei laden
                if (users.Count == 0)
                {
                    Console.WriteLine("Benutzerdaten-Datei ist leer. Erstelle Testbenutzer...");
                    addTestUsers = true;
                }
            }

            if (addTestUsers)
            {
                // Testbenutzer hinzufügen und das aktuelle Datum als CreatedAt verwenden
                users.Add("User1", new User("User1", "123", "standard", DateTime.Now.ToString()));
                users.Add("User2", new User("User2", "adminPass", "admin", DateTime.Now.ToString()));
                
                // Berechtigungen anzeigen
                foreach (var user in users.Values)
                {
                    user.DisplayPermissions();
                }

                // Speichern der Benutzer in die Datei
                SaveUsers();
            }
            else
            {
                Console.WriteLine("Benutzerdaten-Datei enthält Benutzer. Überspringe Testbenutzer-Erstellung.");
            }
        }

        public User GetUserByUsername(string username)
        {
            users.TryGetValue(username, out User user);
            return user;
        }

        public void AddUser(User user)
        {
            if (!users.ContainsKey(user.Username))
            {
                users[user.Username] = user;
                SaveUsers(); // Speichere die Liste nach dem Hinzufügen
            }
            else
            {
                Console.WriteLine("User already exists.");
            }
        }

        public void RemoveUser(string username)
        {
            if (users.Remove(username))
            {
                SaveUsers(); // Speichere die Liste nach dem Entfernen
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        public void RemoveAllUsers()
        {
            users.Clear();
        }

        public bool ChangeUsername(string oldUsername, string newUsername)
        {
            if (users.ContainsKey(oldUsername))
            {
                if (!users.ContainsKey(newUsername)) // Überprüfen, ob der neue Benutzername bereits existiert
                {
                    User user = users[oldUsername];
                    users.Remove(oldUsername); // Alten Benutzernamen entfernen
                    user.Username = newUsername; // Benutzername in der User-Instanz ändern
                    users.Add(newUsername, user); // Neuen Benutzernamen mit aktualisiertem User-Objekt hinzufügen

                    Console.WriteLine($"Benutzername wurde von {oldUsername} auf {newUsername} geändert.");
                    SaveUsers(); // Änderungen speichern
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

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (users.ContainsKey(username))
            {
                User user = users[username];

                // Überprüfen, ob das alte Passwort korrekt ist
                if (user.VerifyPassword(oldPassword))
                {
                    // Setze das neue Passwort
                    user.ChangePassword(newPassword);

                    // Speichere die Benutzer nach der Passwortänderung
                    SaveUsers();

                    Console.WriteLine("Passwort erfolgreich geändert.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Das alte Passwort ist falsch.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Benutzer nicht gefunden.");
                return false;
            }
        }

        // Zugriff auf einen Benutzer
        public void GetUserInfo(string username)
        {
            if (users.ContainsKey(username))
            {
                User userInfo = users[username];
                // Berechtigungen formatieren
                Console.WriteLine($"Benutzer: {username}, " +
                                  $"Passwort-Hash: {userInfo.PasswordHash}, " +
                                  $"Rolle: {userInfo.Role}, " +
                                  $"Erstellt am: {userInfo.CreatedAt}, " +
                                  $"Letzter Login am: {userInfo.LastLogin}, "  );
                userInfo.DisplayPermissions();
            }
            else
            {
                Console.WriteLine("Benutzer existiert nicht.");
            }
        }

        public string GetPasswordHash(string username)
        {
            if (users.ContainsKey(username))
            {
                // Passwort-Hash des Benutzers zurückgeben
                return users[username].PasswordHash;
            }
            else
            {
                Console.WriteLine("Benutzer nicht gefunden.");
                return null;
            }
        }

        public string GetUserRole(string username)
        {
            if (users.ContainsKey(username))
            {
                return users[username].Role; // Gibt die Rolle des Benutzers zurück
            }
            else
            {
                Console.WriteLine("Benutzer existiert nicht.");
                return null; // Alternativ könnte auch ein Standardwert wie "Unbekannt" zurückgegeben werden
            }
        }

        public Dictionary<string, User> GetAllUsers()
        {
            return users;
        }
    }
}
