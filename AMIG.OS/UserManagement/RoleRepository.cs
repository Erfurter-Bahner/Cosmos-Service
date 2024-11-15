using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace AMIG.OS.UserSystemManagement
{
    public class RoleRepository
    {
        public Dictionary<string, Role> roles = new Dictionary<string, Role>();
        private readonly string dataFilePath = @"0:\role.txt"; // Pfad zur Benutzerdaten-Datei

        public RoleRepository()
        {
            InitializeDefaultRoles();
            LoadRoles();
        }

        public void RoleDicTest()
        {
            if (roles.Count == 0)
            {
                Console.WriteLine("Keine Rollen im Dictionary vorhanden.");
                return;
            }

            foreach (var role in roles)
            {
                Console.WriteLine($"Rolle: {role.Key}");
                Console.WriteLine("Berechtigungen:");

                foreach (var permission in role.Value.Permissions)
                {
                    Console.WriteLine($"- {permission}");
                }

                Console.WriteLine(); // Leerzeile zur besseren Lesbarkeit
            }
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

            try
            {
                using (var reader = new StreamReader(dataFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Debug-Ausgabe zur Überprüfung jeder Zeile
                        Console.WriteLine($"Einlesen der Zeile: {line}");

                        var parts = line.Split(',');
                        if (parts.Length >= 2)
                        {
                            string roleName = parts[0].Trim(); // Eventuell vorhandene Leerzeichen entfernen
                            var permissions = new HashSet<string>(parts[1].Split(';').Select(p => p.Trim())); // Berechtigungen als HashSet

                            // Debug-Ausgabe zur Überprüfung der Rollendaten
                            Console.WriteLine($"Gefundene Rolle: {roleName} mit Berechtigungen: {string.Join(", ", permissions)}");

                            var role = new Role(roleName, permissions);
                            roles[roleName] = role;
                        }
                        else
                        {
                            Console.WriteLine($"Zeile konnte nicht verarbeitet werden: {line}");
                        }
                    }
                }

                Console.WriteLine("Rollendaten erfolgreich geladen.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Rollendaten: {ex.Message}");
            }
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

                // Standardrollen hinzufügen7
                string admin = "admin";
                string standarduser = "standarduser";
                roles[admin] = new Role(admin, new HashSet<string> { "CreateUser", "DeleteUser", "ViewLogs", "ModifySettings", "AddRole" });
                roles[standarduser] = new Role(standarduser, new HashSet<string> { "viewLogs" });

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

        // Fügt eine Berechtigung zu einer bestimmten Rolle hinzu
        public void AddPermissionToRole(string roleName, List<string> permissionsToAdd)
        {
            if (roles.TryGetValue(roleName, out Role role))
            {
                foreach (var permission in permissionsToAdd)
                {
                    if (role.Permissions.Contains(permission))
                    {
                        role.Permissions.Add(permission);
                        Console.WriteLine($"Berechtigung '{permission}' wurde aus der Rolle '{roleName}' hinzugefügt.");
                    }
                    else
                    {
                        Console.WriteLine($"Berechtigung '{permission}' ist in der Rolle '{roleName}' nicht vorhanden.");
                    }
                }

                // Rollen speichern, nachdem Berechtigungen entfernt wurden
                SaveRoles();
            }
            else
            {
                Console.WriteLine($"Rolle '{roleName}' wurde nicht gefunden.");
            }
        }

        // Entfernt eine Berechtigung von einer bestimmten Rolle
        public void RemovePermissionsFromRole(string roleName, List<string> permissionsToRemove)
        {
            if (roles.TryGetValue(roleName, out Role role))
            {
                foreach (var permission in permissionsToRemove)
                {
                    if (role.Permissions.Contains(permission))
                    {
                        role.Permissions.Remove(permission);
                        Console.WriteLine($"Berechtigung '{permission}' wurde aus der Rolle '{roleName}' entfernt.");
                    }
                    else
                    {
                        Console.WriteLine($"Berechtigung '{permission}' ist in der Rolle '{roleName}' nicht vorhanden.");
                    }
                }

                // Rollen speichern, nachdem Berechtigungen entfernt wurden
                SaveRoles();
            }
            else
            {
                Console.WriteLine($"Rolle '{roleName}' wurde nicht gefunden.");
            }
        }
        public Dictionary<string, Role> GetAllRoles()
        {
            return roles;
        }
    }
}
