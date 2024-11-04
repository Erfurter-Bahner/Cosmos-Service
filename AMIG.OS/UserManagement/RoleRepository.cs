using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AMIG.OS.Utils;

namespace AMIG.OS.UserSystemManagement
{
    public class RoleRepository
    {
        public Dictionary<string, Role> roles = new Dictionary<string, Role>();
        private readonly string dataFilePath = @"0:\role.txt"; // Pfad zur Benutzerdaten-Datei

        public RoleRepository()
        {
            InitializeDefaultRoles();
        }

        // Speichert alle Rollen in der Datei
        public void SaveRoles()
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
                    foreach (var role in roles.Values)
                    {
                        // Berechtigungen als String zusammenfügen
                        string permissionsString = string.Join(";", role.Permissions);
                        writer.WriteLine($"{role.RoleName},{permissionsString}");
                    }
                }

                Console.WriteLine("Rollendaten erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Rollendaten: {ex.Message}");
            }
        }

        // Lädt alle Rollen aus der Datei
        public void LoadRoles()
        {
            if (!File.Exists(dataFilePath))
            {
                Console.WriteLine("Rollendaten-Datei existiert nicht. Lade keine Rollen.");
                return;
            }

            using (var reader = new StreamReader(dataFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        string roleName = parts[0];
                        var permissions = new HashSet<string>(parts[1].Split(';')); // Berechtigungen als HashSet

                        var role = new Role(roleName, permissions);
                        roles[roleName] = role;
                    }
                }
            }

            Console.WriteLine("Rollendaten erfolgreich geladen.");
        }

        // Initialisiert Standardrollen und speichert sie, falls keine Rollendaten existieren
        public void InitializeDefaultRoles()
        {
            if (File.Exists(dataFilePath))
            {
                Console.WriteLine("Lade Rollen aus Datei...");
                LoadRoles();
            }
            else
            {
                Console.WriteLine("Erstelle Standardrollen, da keine Rollendaten-Datei existiert...");

                // Standardrollen hinzufügen
                roles["Admin"] = new Role("Admin", new HashSet<string> { "createUser", "deleteUser", "viewLogs", "modifySettings" });
                roles["StandardUser"] = new Role("StandardUser", new HashSet<string> { "viewLogs" });

                SaveRoles();
            }
        }

        // Rolle nach Name abrufen
        public Role GetRoleByName(string roleName)
        {
            roles.TryGetValue(roleName, out Role role);
            return role;
        }

        // Neue Rolle hinzufügen
        public void AddRole(Role role)
        {
            if (!roles.ContainsKey(role.RoleName))
            {
                roles[role.RoleName] = role;
                SaveRoles(); // Speichert die Liste nach dem Hinzufügen
            }
            else
            {
                Console.WriteLine("Rolle existiert bereits.");
            }
        }

        // Rolle entfernen
        public void RemoveRole(string roleName)
        {
            if (roles.Remove(roleName))
            {
                SaveRoles(); // Speichert die Liste nach dem Entfernen
            }
            else
            {
                Console.WriteLine("Rolle nicht gefunden.");
            }
        }

        // Alle Rollen abrufen
        public Dictionary<string, Role> GetAllRoles()
        {
            return roles;
        }
    }
}
