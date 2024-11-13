using System;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.FileManagement;
using AMIG.OS.Utils;
using System.IO;
using Sys = Cosmos.System;
using System.Threading;
using System.Data;
using System.Security;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing
{
    public class CommandHandler
    {
        private readonly UserManagement userManagement;
        private readonly Helpers helpers;
        private DateTime starttime;
        private string currentDirectory = @"0:\"; // Root-Verzeichnis als Startpunkt
        private readonly Action showLoginOptions;
        private readonly Sys.FileSystem.CosmosVFS vfs; //damit freier speicherplatz angezeigt werden kann
        
        public CommandHandler
            (UserManagement userMgmt,
            Action showLoginOptionsDelegate,
            Helpers _helpers,
            Sys.FileSystem.CosmosVFS vfs)
            
        {
            this.userManagement = userMgmt;
            showLoginOptions = showLoginOptionsDelegate; // Delegate speichern
            this.helpers = _helpers;
            this.vfs = vfs;

        }

        public void SetStartTime(DateTime loginTime)
        {
            starttime = loginTime;
        }

        public void ProcessCommand(string input, string loggedInUser)
        {
            var args = input.Split(' ');
            var currentUser = userManagement.GetUser(loggedInUser);
            //bool admin_true = true; //userManagement.GetUserRole(loggedInUser).ToLower() == "admin";
            //userManagement.GetUserRole(loggedInUser).ToLower();

            switch (args[0].ToLower())
            { 
                case "addrole": // to file roles.txt
                    {
                        Console.WriteLine("Geben Sie den Namen der neuen Rolle ein:");
                        string roleName = Console.ReadLine();

                        Console.WriteLine("Geben Sie die Berechtigungen für diese Rolle ein (mit Komma getrennt):");
                        string permissionsInput = Console.ReadLine();

                        // Erstellen eines HashSet aus den eingegebenen Berechtigungen
                        HashSet<string> permissions = new HashSet<string>(permissionsInput.Split(','));
                        userManagement.roleRepository.AddRole(new Role(roleName, permissions));
                        break;

                    }

                case "removerole": // from file roles.txt
                    {
                        Console.WriteLine("Geben Sie den Namen der zu löschenden Rolle ein:");
                        string roleName = Console.ReadLine();

                        // Entferne die Rolle aus dem Rollen-Repository
                        userManagement.roleRepository.RemoveRole(roleName);

                        // Hole das Dictionary aller Benutzer
                        Dictionary<string, User> usersDictionary = userManagement.userRepository.GetAllUsers();

                        foreach (var user in usersDictionary.Values)
                        {
                            // Überprüfen, ob der Benutzer die zu löschende Rolle hat
                            var role = user.Roles.FirstOrDefault(r => r.RoleName == roleName);

                            if (role != null)
                            {
                                // Entferne die Rolle aus der Benutzerliste
                                user.Roles.Remove(role);

                                // Entferne alle Berechtigungen, die von dieser Rolle kommen
                                foreach (var permission in role.Permissions)
                                {
                                    user.Permissions.Remove(permission); // Entferne die Berechtigung vom Benutzer
                                }

                                // Update der kombinierten Berechtigungen für den Benutzer
                                user.UpdateCombinedPermissions();
                            }
                        }

                        // Änderungen in den gespeicherten Benutzern und Rollen speichern
                        userManagement.userRepository.SaveUsers();
                        userManagement.roleRepository.SaveRoles();
                        break;
                    }

                case "addroletouser":
                    {
                        // Benutzername abfragen
                        Console.Write("Benutzername: ");
                        string username = Console.ReadLine();

                        User user = userManagement.userRepository.GetUserByUsername(username);

                        // Rollen abfragen und in eine Liste umwandeln
                        Console.Write("Rollen hinzufügen (durch Leerzeichen getrennt, z. B. Admin User): ");
                        string rolesInput = Console.ReadLine();
                        var roleNames = rolesInput.Split(' ').Select(roleName => roleName.Trim());

                        // Für jede Rolle überprüfen und hinzufügen
                        foreach (string roleName in roleNames)
                        {
                            Role role = userManagement.roleRepository.GetRoleByName(roleName);

                            // Überprüfen, ob die Rolle existiert
                            if (role == null)
                            {
                                Console.WriteLine($"Rolle '{roleName}' existiert nicht.");
                                continue; // Nächste Rolle prüfen
                            }

                            // Überprüfen, ob der Benutzer die Rolle bereits hat
                            if (user.Roles.Any(r => r.RoleName == roleName))
                            {
                                Console.WriteLine($"Benutzer '{username}' hat bereits die Rolle '{roleName}'.");
                                continue;
                            }

                            // Rolle und Berechtigungen zum Benutzer hinzufügen
                            user.AddRole(role);

                            Console.WriteLine($"Rolle '{roleName}' wurde erfolgreich zum Benutzer '{username}' hinzugefügt.");
                        }

                        // Benutzer speichern
                        userManagement.userRepository.SaveUsers();
                        break;
                    }

                case "addpermtouser":
                    {
                        // Benutzername abfragen
                        Console.Write("Benutzername: ");
                        string username = Console.ReadLine();

                        User user = userManagement.userRepository.GetUserByUsername(username);

                        if (user == null)
                        {
                            Console.WriteLine($"Benutzer '{username}' existiert nicht.");
                            break;
                        }

                        // Berechtigungen abfragen und in eine Liste umwandeln
                        Console.Write("Berechtigungen hinzufügen (durch Leerzeichen getrennt, z. B. lesen schreiben): ");
                        string permsInput = Console.ReadLine();
                        var permNames = permsInput.Split(' ').Select(permName => permName.Trim());

                        // Für jede Berechtigung überprüfen und hinzufügen
                        foreach (string permName in permNames)
                        {
                            // Überprüfen, ob der Benutzer die Berechtigung bereits hat
                            if (user.Permissions.Contains(permName))
                            {
                                Console.WriteLine($"Benutzer '{username}' hat bereits die Berechtigung '{permName}'.");
                                continue;
                            }

                            // Berechtigung zum Benutzer hinzufügen
                            user.AddPermission(permName);
                            Console.WriteLine($"Berechtigung '{permName}' wurde erfolgreich zum Benutzer '{username}' hinzugefügt.");
                        }

                        // Benutzer speichern
                        userManagement.userRepository.SaveUsers();

                        break;
                    }

                case "rmpermuser": //von user wird perm entfernt
                    {
                        // Benutzername abfragen
                        Console.Write("Benutzername: ");
                        string username = Console.ReadLine();

                        User user = userManagement.userRepository.GetUserByUsername(username);

                        if (user == null)
                        {
                            Console.WriteLine($"Benutzer '{username}' existiert nicht.");
                            break;
                        }

                        // Berechtigungen abfragen und in eine Liste umwandeln
                        Console.Write("Berechtigungen entfernen (durch Leerzeichen getrennt, z. B. lesen schreiben): ");
                        string permsInput = Console.ReadLine();
                        var permNames = permsInput.Split(' ').Select(permName => permName.Trim());

                        // Für jede Berechtigung überprüfen und hinzufügen
                        foreach (string permName in permNames)
                        {
                            // Überprüfen, ob der Benutzer die Berechtigung bereits hat
                            if (!user.Permissions.Contains(permName))
                            {
                                Console.WriteLine($"Benutzer '{username}' hat nicht die Berechtigung '{permName}'.");
                                continue;
                            }

                            // Berechtigung zum Benutzer hinzufügen
                            user.RemovePermission(permName);
                            Console.WriteLine($"Berechtigung '{permName}' wurde erfolgreich vom Benutzer '{username}' gelöscht.");
                        }

                        // Benutzer speichern
                        userManagement.userRepository.SaveUsers();

                        break;
                    }

                case "rmroleuser": //rolle mit perms wird entfernt
                    {
                        // Benutzername abfragen
                        Console.Write("Benutzername: ");
                        string username = Console.ReadLine();

                        User user = userManagement.userRepository.GetUserByUsername(username);

                        if (user == null)
                        {
                            Console.WriteLine($"Benutzer '{username}' existiert nicht.");
                            break;
                        }

                        // Rollen abfragen und in eine Liste umwandeln
                        Console.Write("Rollen entfernen (durch Leerzeichen getrennt, z. B. Admin User): ");
                        string rolesInput = Console.ReadLine();
                        var roleNames = rolesInput.Split(' ').Select(roleName => roleName.Trim());

                        // Für jede Rolle überprüfen und entfernen
                        foreach (string roleName in roleNames)
                        {
                            Role role = userManagement.roleRepository.GetRoleByName(roleName);

                            // Falls die Rolle nicht existiert, wird eine Warnung ausgegeben
                            if (role == null)
                            {
                                Console.WriteLine($"Rolle '{roleName}' existiert nicht.");
                                continue;
                            }

                            // Überprüfen, ob der Benutzer die Rolle besitzt
                            if (!user.Roles.Contains(role))
                            {
                                Console.WriteLine($"Benutzer '{username}' hat die Rolle '{roleName}' nicht.");
                                continue;
                            }

                            // Rolle entfernen
                            user.RemoveRole(role);
                            Console.WriteLine($"Rolle '{roleName}' wurde erfolgreich von Benutzer '{username}' entfernt.");
                        }

                        // Aktualisiere die kombinierten Berechtigungen nach dem Entfernen der Rollen
                        user.UpdateCombinedPermissions();

                        // Benutzer speichern
                        userManagement.userRepository.SaveUsers();

                        break;
                    }

                case "rmpermrole":
                    {
                        Console.Write("Geben Sie den Rollennamen ein: ");
                        string roleNameToRemovePermissions = Console.ReadLine();

                        // Abfrage der Berechtigungen, getrennt durch Leerzeichen oder Kommas
                        Console.Write("Geben Sie die Berechtigungen ein, die entfernt werden sollen (z. B. lesen, schreiben): ");
                        string permissionsToRemoveInput = Console.ReadLine();

                        // Berechtigungen in eine Liste umwandeln und Leerzeichen entfernen
                        var permissionsToRemove = permissionsToRemoveInput.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                          .Select(permission => permission.Trim())
                                                                          .ToList();

                        // Aufruf der Methode, um alle angegebenen Berechtigungen von der Rolle zu entfernen
                        userManagement.roleRepository.RemovePermissionsFromRole(roleNameToRemovePermissions, permissionsToRemove);

                        Dictionary<string, User> usersDictionary = userManagement.userRepository.GetAllUsers();

                        foreach (var user in usersDictionary.Values)
                        {
                            var role = user.Roles.FirstOrDefault(r => r.RoleName == roleNameToRemovePermissions);

                            if (role != null)
                            {
                                // Update der kombinierten Berechtigungen für den Benutzer
                                user.UpdateCombinedPermissions();
                            }

                        }

                        // Änderungen in den gespeicherten Rollen und Benutzern speichern
                        userManagement.userRepository.SaveUsers();
                        break;
                    }

                case "addpermtorole":
                    {
                        Console.Write("Geben Sie den Rollennamen ein: ");
                        string roleNameToAddPermissions = Console.ReadLine();

                        // Abfrage der Berechtigungen, getrennt durch Leerzeichen oder Kommas
                        Console.Write("Geben Sie die Berechtigungen ein, die hinzugefügt werden sollen (z. B. lesen, schreiben): ");
                        string permissionsToAddInput = Console.ReadLine();

                        // Berechtigungen in eine Liste umwandeln und Leerzeichen entfernen
                        var permissionsToAdd = permissionsToAddInput.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                          .Select(permission => permission.Trim())
                                                                          .ToList();

                        // Aufruf der Methode, um alle angegebenen Berechtigungen von der Rolle zu entfernen
                        userManagement.roleRepository.RemovePermissionsFromRole(roleNameToAddPermissions, permissionsToAdd);

                        Dictionary<string, User> usersDictionary = userManagement.userRepository.GetAllUsers();

                        foreach (var user in usersDictionary.Values)
                        {
                            var role = user.Roles.FirstOrDefault(r => r.RoleName == roleNameToAddPermissions);

                            if (role != null)
                            {
                                // Update der kombinierten Berechtigungen für den Benutzer
                                user.UpdateCombinedPermissions();
                            }

                        }

                        // Änderungen in den gespeicherten Rollen und Benutzern speichern
                        userManagement.userRepository.SaveUsers();
                        break;

                    }

                case "datetime":
                    Console.WriteLine(DateTime.Now);
                    break;

                case "adios":
                    if (args.Length == 2 && args[1].Equals("amigos"))
                    {
                        helpers.AdiosHelper();
                    }
                    else
                    {
                        helpers.error1();
                    }
                    break;

                // Benutzerbefehle
                case "showall": //Admin
                        helpers.ShowAllHelper(true); // Admin hat die Berechtigung
                    break;

                case "showme": //Both
                    helpers.ShowMeHelper(loggedInUser);
                    break;

                case "removeuser": //Admin
                    if (currentUser.HasPermission("RemoveUser")) // Berechtigungsprüfung
                    {
                        helpers.RemoveHelper(true); // Admin hat die Berechtigung
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung für diesen Befehl.");
                    }
                    break;

                case "removeall": //Admin
                    //if (currentUser.HasPermission("RemoveAllUsers")) // Berechtigungsprüfung
                    //{
                        userManagement.RemoveAllUser();
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Keine Berechtigung für diesen Befehl.");
                    //}
                    break;

                case "changename": // Both
                    if (currentUser.HasPermission("ChangeName")) // Berechtigungsprüfung
                    {
                        helpers.ChangeNameHelper(loggedInUser);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um den Namen zu ändern.");
                    }
                    break;

                case "changepw": //both
                    if (currentUser.HasPermission("ChangePassword")) // Berechtigungsprüfung
                    {
                        helpers.ChangePasswortHelper(loggedInUser);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um das Passwort zu ändern.");
                    }
                    break;

                case "showtime": //both
                    TimeSpan current = DateTime.Now - starttime;
                    Console.WriteLine($" Eingeloggte Zeit: {current}");
                    break;

                case "logout": //both
                    Console.Clear();
                    showLoginOptions.Invoke();
                    break;

                case "adduser":
                    {
                        // Benutzerdaten abfragen
                        Console.Write("Username: ");
                        string username = Console.ReadLine();

                        Console.Write("Password: ");
                        string password = Console.ReadLine();

                        // Rollen (durch Semikolon getrennt, z. B. Admin;User)
                        Console.Write("Roles (durch Semikolon getrennt, z. B. Admin;User): ");
                        string rolesInput = Console.ReadLine();

                        var roles = new List<Role>();  // Rollen als Dictionary speichern
                        var userPermissions = new HashSet<string>(); // Berechtigungen für den Benutzer werden hier gesammelt

                        foreach (var roleName in rolesInput.Split(';'))  // Rollen durch Semikolon trennen
                        {
                            var trimmedRoleName = roleName.Trim();  // Leere Leerzeichen entfernen
                            Role role = userManagement.roleRepository.GetRoleByName(trimmedRoleName); // Beispielmethode, um Rolle nach Name zu finden

                            if (role != null)
                            {
                                Console.WriteLine($"Rolle: {role.RoleName}");
                                Console.WriteLine("Berechtigungen:");
                                foreach (var permission in role.Permissions)
                                {
                                    Console.WriteLine($"- {permission}");
                                }

                                roles.Add(role);  // Rolle ins Dictionary einfügen, wobei der Schlüssel der Name der Rolle ist
                                userPermissions.UnionWith(role.Permissions);  // Berechtigungen der Rolle hinzufügen
                            }
                            else
                            {
                                Console.WriteLine($"Rolle '{trimmedRoleName}' nicht gefunden.");
                            }
                        }

                        // Benutzer erstellen
                        var user = new User(username, password, roles: roles, permissions: userPermissions);

                        // Hinzufügen des Benutzers zur Sammlung
                        userManagement.userRepository.AddUser(user);  // Methode, um den Benutzer hinzuzufügen

                        // Optional: Ausgabe zur Bestätigung
                        Console.WriteLine("Benutzer erfolgreich hinzugefügt!");

                        break;
                    }

                // Datei- und Verzeichnisbefehle
                case "mkdir": //admin
                    if (currentUser.HasPermission("CreateDirectory")) // Berechtigungsprüfung
                    {
                        helpers.mkdirHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um ein Verzeichnis zu erstellen.");
                    }
                    break;

                case "cd": //both             
                    helpers.cdHelper(args, ref currentDirectory);
                    break;

                case "ls": //both                    
                    helpers.lsHelper(args, currentDirectory);
                    break;

                case "write": //admin
                    if (currentUser.HasPermission("WriteToFile")) // Berechtigungsprüfung
                    {
                        helpers.writeHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um in die Datei zu schreiben.");
                    }
                    break;

                case "rm": //admin
                    if (currentUser.HasPermission("RemoveFile")) // Berechtigungsprüfung
                    {
                        helpers.rmHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um die Datei zu entfernen.");
                    }
                    break;

                case "rmdir": //admin
                    if (currentUser.HasPermission("RemoveDirectory")) // Berechtigungsprüfung
                    {
                        helpers.rmdirHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um das Verzeichnis zu entfernen.");
                    }
                    break;

                case "touch": //admin
                    if (currentUser.HasPermission("CreateFile")) // Berechtigungsprüfung
                    {
                        helpers.touchHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um die Datei zu erstellen.");
                    }
                    break;

                case "cat": //both
                    {
                        string role = "admin";
                        helpers.catHelper(args, currentDirectory, role);
                        break;
                    }
                    
                case "space": //both
                    var availableSpace = vfs.GetAvailableFreeSpace(@"0:\");
                    Console.WriteLine($"Verfügbarer Speicherplatz: {availableSpace} Bytes");
                    break;

                case "setfileperm": //admin
                    if (currentUser.HasPermission("SetFilePermission")) // Berechtigungsprüfung
                    {
                        helpers.setpermHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um Berechtigungen zu setzen.");
                    }
                    break;

                case "unlock": //admin
                    if (currentUser.HasPermission("UnlockFile")) // Berechtigungsprüfung
                    {
                        helpers.unlockHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um den Benutzer zu entsperren.");
                    }
                    break;

                case "clear": //both
                    Console.Clear();
                    break;

                // Beispiel für andere Befehle
                case "help": //both
                    Console.WriteLine("Available commands: showall, adduser, removeuser, mkdir, rmdir, touch, rm, cat.");
                    break;

                default: //both
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
