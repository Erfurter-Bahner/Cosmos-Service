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
        public string PermissionName { get; } = Permissions.addpermtorole; // Required permission name
        public string Description => "Add permission/s to a role";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-role", "Name of the role to add permissions" },
            { "-permissions", "Permissions to add to role" },
            {"-help", "Show help for this command."},
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
            // Hilfe anzeigen, wenn der "help"-Parameter enthalten ist
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }

            parameters.TryGetValue("permissions", out string permissionsRaw);
            parameters.TryGetValue("role", out string roleName);

            // Überprüfen, ob der notwendige Parameter "role" vorhanden ist
            if (!string.IsNullOrEmpty(roleName))
            {
                Console.WriteLine($"Found permissions: {roleName}");
                // Überprüfen, ob der "permissions"-Parameter vorhanden ist
                if (!string.IsNullOrEmpty(permissionsRaw))
                {
                    // Erstelle ein HashSet der angegebenen Berechtigungen
                    var permissionsToAdd = permissionsRaw.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                                              .Select(permission => permission.Trim())
                                                                                              .ToList();

                    // Aufruf der Methode, um alle angegebenen Berechtigungen zu der Rolle hinzuzufügen
                    userManagement.roleRepository.AddPermissionToRole(roleName, permissionsToAdd);

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
                    ConsoleHelpers.WriteError("Error: No permissions were specified. Use -permissions p1 p2 p3");
                }
            }
            else
            {
                ConsoleHelpers.WriteError("Error: Parameter '-role' is missing. Use -help to see usage.");
            }
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: addpermrole [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}

