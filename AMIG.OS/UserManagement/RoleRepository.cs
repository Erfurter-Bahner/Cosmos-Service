using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AMIG.OS.CommandProcessing;
using AMIG.OS.CommandProcessing.Commands.UserSystem;
using AMIG.OS.Utils;

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

                ConsoleHelpers.WriteSuccess("Role saved successfully");
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Saving Roles: {ex.Message}");
            }
        }

        // Lädt alle Rollen aus der Datei
        public void LoadRoles()
        {
            if (!File.Exists(dataFilePath))
            {
                ConsoleHelpers.WriteError("Error: Role file not existing. Roles not loaded.");
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
                        //Console.WriteLine($"Reading line: {line}");

                        var parts = line.Split(',');
                        if (parts.Length >= 2)
                        {
                            string roleName = parts[0].Trim(); // Eventuell vorhandene Leerzeichen entfernen
                            var permissions = new HashSet<string>(parts[1].Split(';').Select(p => p.Trim())); // Berechtigungen als HashSet

                            // Debug-Ausgabe zur Überprüfung der Rollendaten
                            //Console.WriteLine($"Gefundene Rolle: {roleName} mit Berechtigungen: {string.Join(", ", permissions)}");

                            var role = new Role(roleName, permissions);
                            roles[roleName] = role;
                        }
                        //else
                        //{
                        //    Console.WriteLine($"Zeile konnte nicht verarbeitet werden: {line}");
                        //}
                    }
                }

                ConsoleHelpers.WriteSuccess("Roles loaded successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Loading Roles: {ex.Message}");
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
                    Permissions.mkdir,
                    Permissions.showallperms,
                    Permissions.showallroles,
                    Permissions.showrole
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
                    Permissions.mkdir,
                    Permissions.showallperms,
                    Permissions.showallroles,
                    Permissions.showrole
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
                Console.Write("Warning: Role will be overwritten.");
                return; 
            }
        }

        // Rolle entfernen
        public void RemoveRole(string roleName)
        {
            if (roles.Remove(roleName))
            {
                return; 
            }
            else
            {
                ConsoleHelpers.WriteError("Error: Role not found.");
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
                        Console.WriteLine($"Warning: Permission'{permission}' doesnt exist.");
                        continue;
                    }

                    if (role.Permissions.Contains(permission))
                    {
                        Console.WriteLine($"Warning: Permission '{permission}' already exist in role '{roleName}'.");
                        continue;
                    }

                    role.Permissions.Add(permission);
                    ConsoleHelpers.WriteSuccess($"Permission '{permission}' was added to role '{roleName}' sucessfully.");
                }

                // Rollen speichern, nachdem Berechtigungen hinzugefügt wurden
                SaveRoles();
            }
            else
            {
                ConsoleHelpers.WriteError($"Error: Role not found: '{roleName}' ");
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
                        ConsoleHelpers.WriteSuccess($"Permission'{permission}' was removed from role '{roleName}' successfully.");
                    }
                    else
                    {
                        Console.Write($"Warning: Permission '{permission}' doesnt exist in role '{roleName}'.");
                    }
                }

                // Rollen speichern, nachdem Berechtigungen entfernt wurden
                SaveRoles();
            }
            else
            {
                ConsoleHelpers.WriteError($"Error: Role not found: '{roleName}'");
            }
        }
        public Dictionary<string, Role> GetAllRoles()
        {
            return roles;
        }

        //showall
        public void DisplayAllRoles()
        {
            if (roles.Count == 0)
            {
                ConsoleHelpers.WriteError("No roles available.");
                return;
            }
            foreach (var role in roles.Values)
            {
                // Überprüfen, ob der Benutzer gefunden wurde
                if (role != null)
                {
                    Console.WriteLine($"Rolename: {role.RoleName}");
                }
                else
                {
                    ConsoleHelpers.WriteError($"Error: Role '{role.RoleName}' does not exist.");
                }
            }
        }

        //showspecific 
        public void DisplayRole(Role role)
        {
            // Überprüfen, ob der Benutzer gefunden wurde
            if (role != null)
            {
                string permissionsDisplay = role.Permissions != null && role.Permissions.Count > 0
                    ? string.Join(", ", role.Permissions)
                    : "No permissions"; // Anzeige "Keine Berechtigungen", falls leer


                Console.WriteLine($"Rolename: {role.RoleName}, " +
                    //$"PW: {user.PasswordHash}, " + +
                    $"RolePerm: {permissionsDisplay}");
            }
            else
            {
                ConsoleHelpers.WriteError($"Error: User '{role.RoleName}' does not exist.");
            }
        }

    }
}
