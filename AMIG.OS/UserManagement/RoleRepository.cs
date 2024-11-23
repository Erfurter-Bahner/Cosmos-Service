using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AMIG.OS.CommandProcessing;

namespace AMIG.OS.UserSystemManagement
{
    public class RoleRepository
    {
        public Dictionary<string, Role> roles = new Dictionary<string, Role>();
        private readonly string dataFilePath = @"0:\role.txt"; // Pfad zur Benutzerdaten-Datei

        public RoleRepository()
        {
            LoadRoles();
            InitializeDefaultRoles();           
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
            string admin = "admin";
            string standarduser = "standard";

            // Überschreibt oder fügt die Standardrollen hinzu
            if (!roles.ContainsKey(admin))
            {
                roles[admin] = new Role(admin, new HashSet<string>
                {
                    Permissions.addrole,
                    Permissions.rmrole,
                    Permissions.addroletouser,
                    Permissions.rmroleuser,
                    Permissions.addpermtouser,
                    Permissions.rmpermuser,
                    Permissions.rmpermrole,
                    Permissions.addpermtorole,
                    Permissions.adduser,
                    Permissions.rmuser,
                    Permissions.showall,
                    Permissions.showme,
                    Permissions.showuser,
                    Permissions.changename,
                    Permissions.changepw,
                    Permissions.extra,
                    Permissions.ls,
                    Permissions.cat,
                    Permissions.touch,
                    Permissions.cd,
                    Permissions.write,
                    Permissions.rmfile,
                    Permissions.rmdir,
                    Permissions.mkdir

                });
            }
            else
            {
                // Rolle existiert bereits, also nur Berechtigungen überschreiben
                roles[admin].Permissions = new HashSet<string>
                {
                    Permissions.addrole,
                    Permissions.rmrole,
                    Permissions.addroletouser,
                    Permissions.rmroleuser,
                    Permissions.addpermtouser,
                    Permissions.rmpermuser,
                    Permissions.rmpermrole,
                    Permissions.addpermtorole,
                    Permissions.adduser,
                    Permissions.rmuser,
                    Permissions.showall,
                    Permissions.showme,
                    Permissions.showuser,
                    Permissions.changename,
                    Permissions.changepw,
                    Permissions.extra,
                    Permissions.ls,
                    Permissions.cat,
                    Permissions.touch,
                    Permissions.cd,
                    Permissions.write,
                    Permissions.rmfile,
                    Permissions.rmdir,
                    Permissions.mkdir
                };
            }

            // Standarduser-Rolle hinzufügen oder überschreiben
            if (!roles.ContainsKey(standarduser))
            {
                roles[standarduser] = new Role(standarduser, new HashSet<string>
                {
                    Permissions.extra,
                    Permissions.showme,
                    Permissions.changename,
                    Permissions.changepw
                });
            }
            else
            {
                // Rolle existiert bereits, also nur Berechtigungen überschreiben
                roles[standarduser].Permissions = new HashSet<string>
                {
                    Permissions.extra,
                    Permissions.showme,
                    Permissions.changename,
                    Permissions.changepw
                };
            }
            // Speichern der Rollen
            SaveRoles();
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

        public void AddPermissionToRole(string roleName, List<string> permissionsToAdd)
        {
            if (roles.TryGetValue(roleName, out Role role))
            {
                foreach (var permission in permissionsToAdd)
                {
                    
                    if (!Permissions.IsValidPermission(permission.ToString()))
                    {
                        Console.WriteLine($"Berechtigung '{permission}' existiert in der Berechtigungsliste nicht.");
                        continue;
                    }

                    if (role.Permissions.Contains(permission))
                    {
                        Console.WriteLine($"Berechtigung '{permission}' ist in der Rolle '{roleName}' schon vorhanden.");
                        continue;
                    }

                    role.Permissions.Add(permission);
                    Console.WriteLine($"Berechtigung '{permission}' wurde der Rolle '{roleName}' hinzugefügt.");
                }

                // Rollen speichern, nachdem Berechtigungen hinzugefügt wurden
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
