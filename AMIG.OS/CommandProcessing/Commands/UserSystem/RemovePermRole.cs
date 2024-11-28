using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class RemovePermRole : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = Permissions.rmpermrole; // Required permission name
        public string Description => "Remove permissions form a role";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-role", "Name of the role, to remove perms from" },
            { "-permissions", "Permissions to remove from the role" },
            { "-help", "Shows usage information for the command." }
        };

        public RemovePermRole(UserManagement userManagement)
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
                Console.WriteLine($"Gefundene Berechtigungen: {roleName}");
                // Überprüfen, ob der "permissions"-Parameter vorhanden ist
                if (!string.IsNullOrEmpty(permissionsRaw))
                {
                    // Erstelle ein HashSet der angegebenen Berechtigungen
                    var permissionsToRemove = permissionsRaw.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                                              .Select(permission => permission.Trim())
                                                                                              .ToList();

                    // Aufruf der Methode, um alle angegebenen Berechtigungen von der Rolle zu entfernen
                    userManagement.roleRepository.RemovePermissionsFromRole(roleName, permissionsToRemove);

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
