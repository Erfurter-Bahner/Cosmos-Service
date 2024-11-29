using AMIG.OS.CommandProcessing.Commands.Extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AMIG.OS.Utils;

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
            InitializeAdmin();
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
                        //Console.WriteLine($"Speichere Benutzer: {user.Username}, Rollen: {rolesString}, Berechtigungen: {permissionsString}, CombinedPermissons: {combinedPermissionsString}");

                        // Benutzerinformationen in die Datei schreiben
                        writer.WriteLine($"{user.Username},{user.PasswordHash},{rolesString},{user.CreatedAt},{user.LastLogin},{permissionsString}");
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleHelpers.WriteSuccess("Userinfo saved sucessfully.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Saving Users: {ex.Message}");
            }
        }

        //lade benutzer aus Datei
        public void LoadUsers()
        {
            // Überprüfen, ob die Benutzerdaten-Datei existiert
            if (!File.Exists(dataFilePath))
            {
                ConsoleHelpers.WriteError("Error: Userinfo file not existing.");
                return;
            }

            // Datei mit StreamReader öffnen und Zeilenweise auslesen
            using (var reader = new StreamReader(dataFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Zeile anhand des Trennzeichens ',' in Teile splitten
                    var parts = line.Split(',');

                    // Sicherstellen, dass genügend Teile vorhanden sind, um alle Felder zu laden
                    if (parts.Length >= 6)
                    {
                        string username = parts[0];
                        string passwordHash = parts[1];
                        string createdAt = parts[3];
                        string lastLogin = parts[4];

                        // Rollen des Benutzers basierend auf den Rollennamen laden
                        var roles = new List<Role>();
                        foreach (var roleName in parts[2].Split(';'))
                        {
                            // Leere Rollennamen überspringen und warnen
                            if (string.IsNullOrEmpty(roleName))
                            {
                                //Console.WriteLine($"Warnung: Leere Rolle gefunden für Benutzer '{username}'. Rolle wird übersprungen.");
                                continue;
                            }

                            // Rolle abrufen und hinzufügen, falls gefunden
                            var role = roleRepository.GetRoleByName(roleName);
                            if (role != null)
                            {
                                roles.Add(role);
                            }
                            //else
                            //{
                            //    Console.WriteLine($"Warnung: Rolle '{roleName}' wurde nicht gefunden für Benutzer '{username}'.");
                            //}
                        }

                        // Berechtigungen anhand von Semikolon getrennten Werten in ein HashSet laden
                        var permissions = new HashSet<string>(parts[5].Split(';'));

                        // Benutzerobjekt erstellen und in das Dictionary einfügen
                        try
                        {
                            var user = new User(username, passwordHash, true, roles: roles, permissions: permissions)
                            {
                                LastLogin = lastLogin,
                                CreatedAt = createdAt
                            };
                            users[username] = user;
                        }
                        catch (Exception ex)
                        {
                            ConsoleHelpers.WriteError($"Error: Creating user: '{username}': {ex.Message}");
                        }
                    }
                    else
                    {
                        ConsoleHelpers.WriteError("Error: Wrong format in User file.");
                    }
                }
            }

            ConsoleHelpers.WriteSuccess("User loaded successfully.");
        }

        // Initialisiert Testbenutzer, falls die Datei nicht existiert oder leer ist
        public void InitializeAdmin()
        {
            string userAdmin = "Admin";
            if (!users.ContainsKey(userAdmin))
            {
                Console.WriteLine("Erstelle Admin ...");
                var adminRole = roleRepository.GetRoleByName("admin");                              
                users.Add(userAdmin, new User(userAdmin, "adminPass", false, new List<Role> { adminRole }));
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
                ConsoleHelpers.WriteError("Error: Username already exist.");
            }
        }

        // Entfernt einen Benutzer und speichert die Benutzerliste in der Datei
        public void RemoveUser(string username)
        {
            if (users.Remove(username))
            {
                SaveUsers();
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
            if (users == null)
            {
                ConsoleHelpers.WriteError("Error: No User.");
                return null;
            }
            else
            {
                return users;               
            }            
        }
    }
}
