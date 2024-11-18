using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class AddPermToRole : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = "addpermtorole"; // Required permission name
        public string Description => "Add permission/s to a role";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-role", "Name of the role to add permissions" },
            { "-permissions", "Permissions to add to role" },
            { "-help", "Shows usage information for the command." }
        };

        public AddPermToRole(UserManagement userManagement)
        {
            this.userManagement = userManagement;
        }

        // Implementing CanExecute as defined in the custom ICommand interface
        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        // Implementing Execute as defined in the custom ICommand interface
        public void Execute(CommandParameters parameters, User currentUser)
        {
            if (parameters == null)
            {
                Console.WriteLine("parameters null in addrole");
            }
            // Hilfe anzeigen, wenn der "help"-Parameter enthalten ist
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
            }

            parameters.TryGetValue("permissions", out string permissionsRaw);
            parameters.TryGetValue("role", out string roleName);

            // Überprüfen, ob der notwendige Parameter "role" vorhanden ist
            if (!string.IsNullOrEmpty(roleName))
            {
                Console.WriteLine($"Gefundene Berechtigungen: {roleName}");
                // Überprüfen, ob der "permissions"-Parameter vorhanden ist
                if (!string.IsNullOrEmpty(permissionsRaw))
                {
                    // Erstelle ein HashSet der angegebenen Berechtigungen
                    var permissionsToRemove = permissionsRaw.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                                              .Select(permission => permission.Trim())
                                                                                              .ToList();

                    // Aufruf der Methode, um alle angegebenen Berechtigungen zu der Rolle hinzuzufügen
                    userManagement.roleRepository.AddPermissionToRole(roleName, permissionsToRemove);

                    Dictionary<string, User> usersDictionary = userManagement.userRepository.GetAllUsers();

                    foreach (var user in usersDictionary.Values)
                    {
                        var role = user.Roles.FirstOrDefault(r => r.RoleName == roleName);

                        if (role != null)
                        {
                            // Update der kombinierten Berechtigungen für den Benutzer
                            user.UpdateCombinedPermissions();
                        }

                    }

                    // Änderungen in den gespeicherten Rollen und Benutzern speichern
                    userManagement.userRepository.SaveUsers();
                }
                else
                {
                    Console.WriteLine("Fehler: Es wurden keine Berechtigungen angegeben. Verwenden Sie -permissions <Liste>");
                }
            }
            else
            {
                Console.WriteLine("Fehler: Der Parameter '-role' fehlt. Verwenden Sie -help, um die Syntax zu sehen.");
            }
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: rmpermrole [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}

