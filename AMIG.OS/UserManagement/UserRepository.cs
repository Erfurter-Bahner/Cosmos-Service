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
            //InitializeTestUsers();
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
                        // Rollen als String zusammenfügen
                        string rolesString = string.Join(";", user.Roles.Select(r => r.RoleName));
                        // Berechtigungen als String zusammenfügen
                        string permissionsString = string.Join(";", user.Permissions);

                        writer.WriteLine($"{user.Username},{user.PasswordHash},{rolesString},{user.CreatedAt},{user.LastLogin},{permissionsString}");
                    }
                }

                Console.WriteLine("Benutzerdaten erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Benutzer: {ex.Message}");
            }
        }

        // Lädt alle Benutzer aus der Datei
        public void LoadUsers(RoleRepository roleRepository)
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
                        string passwordHash = parts[1];
                        string createdAt = parts[3];
                        string lastLogin = parts[4];

                        // Rollen und Berechtigungen laden
                        var roles = new HashSet<Role>();


                        //DAS PROBLEM IST ZEILE 78
                        var role = new Role("test", new HashSet<string> { "testRole" });
                        roles.Add(role); // HIER DAS GANZ GROßE PROBLEM


                        //foreach (var roleName in parts[2].Split(';'))
                        //{
                        //    Console.WriteLine("Testöasdkljföalsdkjfösdkljf");

                        //    //var role = roleRepository.GetRoleByName(roleName);
                        //    var role = new Role("Admin", new HashSet<string> { "createUser", "deleteUser", "viewLogs", "modifySettings" });
                        //    if (role != null)
                        //    {
                        //        roles.Add(role);
                        //    }
                        //}

                        var permissions = new HashSet<string>(parts[5].Split(';'));
                        Console.WriteLine("Test5");
                        var user = new User(username, passwordHash, true, roles, permissions);

                        users[username] = user;
                    }
                }
            }

            Console.WriteLine("Benutzerdaten erfolgreich geladen.");
        }

        //public void InitializeTestUsers()
        //{
        //    bool addTestUsers = true;

        //    // Prüfen, ob die Datei existiert
        //    if (!File.Exists(dataFilePath))
        //    {
        //        Console.WriteLine("Benutzerdaten-Datei existiert nicht. Erstelle Testbenutzer...");
        //        addTestUsers = true;
        //        Console.WriteLine("test x.1");
        //    }
        //    else
        //    {
        //        // Datei existiert, aber prüfen, ob sie Benutzer enthält
        //        //LoadUsers(); // Benutzer aus Datei laden
        //        if (users.Count == 0)
        //        {
        //            Console.WriteLine("Benutzerdaten-Datei ist leer. Erstelle Testbenutzer...");
        //            addTestUsers = true;
        //        }
        //    }

        //    if (addTestUsers)
        //    {
        //        // Testbenutzer hinzufügen und das aktuelle Datum als CreatedAt verwenden
        //        var adminRole = new Role("Admin", new HashSet<string> { "createUser", "deleteUser", "viewLogs" });
        //        var userPermissions = new HashSet<string> { "viewReports" };

        //        users.Add("User1", new User("Admin", "adminPass", false, new HashSet<Role> { adminRole }, userPermissions));

        //        // Berechtigungen anzeigen
        //        foreach (var user in users.Values)
        //        {
        //            user.DisplayPermissions();
        //        }

        //        // Speichern der Benutzer in die Datei
        //        SaveUsers();
        //    }
        //    else
        //    {
        //        Console.WriteLine("Benutzerdaten-Datei enthält Benutzer. Überspringe Testbenutzer-Erstellung.");
        //    }
        //}

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
        //public void GetUserInfo(string username)
        //{
        //    if (users.ContainsKey(username))
        //    {
        //        User userInfo = users[username];
        //        // Berechtigungen formatieren
        //        Console.WriteLine($"Benutzer: {username}, " +
        //                          $"Passwort-Hash: {userInfo.PasswordHash}, " +
        //                          $"Rolle: {userInfo.Role}, " +
        //                          $"Erstellt am: {userInfo.CreatedAt}, " +
        //                          $"Letzter Login am: {userInfo.LastLogin}, "  );
        //        userInfo.DisplayPermissions();
        //    }
        //    else
        //    {
        //        Console.WriteLine("Benutzer existiert nicht.");
        //    }
        //}

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

        //public string GetUserRole(string username)
        //{
        //    if (users.ContainsKey(username))
        //    {
        //        return users[username].Roles; // Gibt die Rolle des Benutzers zurück
        //    }
        //    else
        //    {
        //        Console.WriteLine("Benutzer existiert nicht.");
        //        return null; // Alternativ könnte auch ein Standardwert wie "Unbekannt" zurückgegeben werden
        //    }
        //}

        public Dictionary<string, User> GetAllUsers()
        {
            return users;
        }
    }
}
