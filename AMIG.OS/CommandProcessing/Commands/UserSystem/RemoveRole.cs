using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class RemoveRole : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = "rmrole"; // Required permission name
        public string Description => "remove a role";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-rolename", "Name of the new role" },
            { "-help", "Shows usage information for the command." }
        };

        public RemoveRole(UserManagement userManagement)
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
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }      

            if (parameters.TryGetValue("role", out string roleName))
            {               
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
            }
            else
            {
                Console.WriteLine("Insufficient arguments. Use -help to see usage.");
            }
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine($"Usage: rmrole [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
