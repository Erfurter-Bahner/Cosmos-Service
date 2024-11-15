using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class AddRoleToUser : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = "addroletouser"; // Required permission name
        public string Description => "add a role to a user";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-rolename", "Name of the new role" },
            { "-help", "Shows usage information for the command." }
        };

        public AddRoleToUser(UserManagement userManagement)
        {
            this.userManagement = userManagement;
        }

        // Implementing CanExecute as defined in the custom ICommand interface
        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        // Implementing Execute as defined in the custom ICommand interface
        public void Execute(string[] args, User currentUser)
        {
            if (args.Contains("-help"))
            {
                ShowHelp();
                return;
            }

            if (args.Length >= 2)
            {
              
                string username = args[0];

                User user = userManagement.userRepository.GetUserByUsername(username);

                string[] rolesNameInputs = args.Skip(1).ToArray();
                
                
                // Für jede Rolle überprüfen und hinzufügen
                foreach (string roleName in rolesNameInputs)
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
            Console.WriteLine($"Usage: {PermissionName} [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
