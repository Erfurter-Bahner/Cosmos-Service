using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class AddRole : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = Permissions.addrole; // Required permission name
        public string Description => "Add a role";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-role", "Name of the new role/s" },
            { "-permissions", "Permissions of the new role" },
           {"-help", "Show help for this command."},
        };

        public AddRole(UserManagement userManagement)
        {
            this.userManagement = userManagement;
        }

        // Implementing CanExecute as defined in the custom ICommand interface
        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        public void Execute(CommandParameters parameters, User currentUser)
        {
            // Hilfe anzeigen, wenn der "help"-Parameter enthalten ist
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }

            // Extrahiere die Werte der Parameter
            parameters.TryGetValue("permissions", out string permissionsRaw);
            parameters.TryGetValue("role", out string roleName);

            // Überprüfen, ob der notwendige Parameter "role" vorhanden ist
            if (string.IsNullOrEmpty(roleName))
            {
                ConsoleHelpers.WriteError("Error: Parameter '-role' is missing. Use -help to see usage.");
                return;
            }

            // Überprüfen, ob Berechtigungen angegeben wurden
            if (string.IsNullOrEmpty(permissionsRaw))
            {
                ConsoleHelpers.WriteError("Error: No permissions were specified. Use -permissions p1 p2 p3");
                return;
            }

            // Erstelle ein HashSet der angegebenen Berechtigungen
            var permissions = permissionsRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> permissionsHash = new HashSet<string>();

            // Gültige Berechtigungen überprüfen und hinzufügen
            foreach (var permission in permissions)
            {
                if (Permissions.IsValidPermission(permission))
                {
                    permissionsHash.Add(permission);
                }
                else
                {
                    Console.WriteLine($"Warning: '{permission}' isnt a known permission and was skipped.");
                }
            }

            // Prüfen, ob nach der Validierung noch Berechtigungen übrig sind
            if (permissionsHash.Count == 0)
            {
                ConsoleHelpers.WriteError("Error: No valid permissions specified. Role has not been created.");
                return;
            }

            // Neue Rolle hinzufügen
            userManagement.roleRepository.AddRole(new Role(roleName, permissionsHash));
            ConsoleHelpers.WriteSuccess($"Role '{roleName}' with Permissions '{string.Join(", ", permissionsHash)}' added.");
        }


        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine($"Usage: addrole [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}
