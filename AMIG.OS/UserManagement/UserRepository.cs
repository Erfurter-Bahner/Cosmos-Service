using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AMIG.OS.UserSystemManagement
{
    public class UserRepository
    {
        // Speichert alle Benutzer mit dem Benutzernamen als Schlüssel
        public Dictionary<string, User> users = new Dictionary<string, User>();
        private readonly string dataFilePath = @"0:\user.txt"; // Pfad zur Benutzerdaten-Datei
        private readonly RoleRepository roleRepository;

        public UserRepository(RoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
            LoadUsers();
            InitializeTestUsers();
        }

        // Speichert alle Benutzer in die Datei
        public void SaveUsers()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    File.Delete(dataFilePath); // Löscht bestehende Datei, um Überschneidungen zu vermeiden
                }

                using (var stream = File.Create(dataFilePath))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var user in users.Values)
                    {
                        // Konvertiert Rollen und Berechtigungen in Strings zur Speicherung
                        string rolesString = string.Join(";", user.Roles.Select(r => r.RoleName));
                        string permissionsString = string.Join(";", user.Permissions);
                        string combinedPermissionsString = string.Join(";", user.CombinedPermissions);

                        // Debug-Ausgabe zur Überprüfung der Daten
                        Console.WriteLine($"Speichere Benutzer: {user.Username}, Rollen: {rolesString}, Berechtigungen: {permissionsString}, CombinedPermissons: {combinedPermissionsString}");

                        // Benutzerinformationen in die Datei schreiben
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

        // Lädt alle Benutzer aus der Datei und weist Rollen und Berechtigungen zu
        public void LoadUsers()
        {
            if (!File.Exists(dataFilePath))
            {
                Console.WriteLine("Benutzerdaten-Datei nicht gefunden.");
                return;
            }

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

                        // Rollen des Benutzers anhand der Rollennamen laden
                        var roles = new List<Role>();
                        foreach (var roleName in parts[2].Split(';'))
                        {
                            var role = roleRepository.GetRoleByName(roleName.Trim());
                            if (role != null)
                            {
                                roles.Add(role);
                            }
                            else
                            {
                                Console.WriteLine($"Warnung: Rolle '{roleName.Trim()}' wurde nicht gefunden.");
                            }
                        }

                        // Berechtigungen in ein HashSet konvertieren
                        var permissions = new HashSet<string>(parts[5].Split(';'));

                        // Benutzerobjekt erstellen und in das Dictionary einfügen
                        var user = new User(username, passwordHash, true, roles, permissions)
                        {
                            LastLogin = lastLogin
                        };
                        users[username] = user;
                    }
                }
            }

            Console.WriteLine("Benutzerdaten erfolgreich geladen.");
        }

        // Initialisiert Testbenutzer, falls die Datei nicht existiert oder leer ist
        public void InitializeTestUsers()
        {
            if (!File.Exists(dataFilePath) || users.Count == 0)
            {
                Console.WriteLine("Erstelle Testbenutzer...");
                var adminRole = roleRepository.GetRoleByName("Admin");
                var userPermissions = new HashSet<string> { "viewReports" };

                // Beispielbenutzer hinzufügen
                users.Add("User1", new User("User1", "adminPass", false, new List<Role> { adminRole }, userPermissions));
                SaveUsers();
            }
            else
            {
                Role newRole = roleRepository.GetRoleByName("StandardUser");
                users["User1"].AddRole(newRole);
                SaveUsers();
            }
        }

        // Gibt den Benutzer anhand des Benutzernamens zurück
        public User GetUserByUsername(string username)
        {
            users.TryGetValue(username, out User user);
            return user;
        }

        // Fügt einen Benutzer hinzu und speichert die Benutzerliste in der Datei
        public void AddUser(User user)
        {
            if (!users.ContainsKey(user.Username))
            {
                users[user.Username] = user;
                SaveUsers();
            }
            else
            {
                Console.WriteLine("Benutzername existiert bereits.");
            }
        }

        // Entfernt einen Benutzer und speichert die Benutzerliste in der Datei
        public void RemoveUser(string username)
        {
            if (users.Remove(username))
            {
                SaveUsers();
            }
            else
            {
                Console.WriteLine("Benutzer nicht gefunden.");
            }
        }

        // Entfernt alle Benutzer und speichert die leere Benutzerliste
        public void RemoveAllUsers()
        {
            users.Clear();
            SaveUsers();
        }

        // Ändert das Passwort eines Benutzers, sofern das alte Passwort korrekt ist
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (users.TryGetValue(username, out User user))
            {
                if (user.VerifyPassword(oldPassword))
                {
                    user.ChangePassword(newPassword);
                    SaveUsers();
                    Console.WriteLine("Passwort erfolgreich geändert.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Das alte Passwort ist falsch.");
                }
            }
            else
            {
                Console.WriteLine("Benutzer nicht gefunden.");
            }

            return false;
        }

        // Gibt alle Benutzer im Repository zurück
        public Dictionary<string, User> GetAllUsers()
        {
            return users;
        }
    }
}
