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
        public string PermissionName { get; } = "AddRole"; // Required permission name
        public string Description => "Add a role";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-rolename", "Name of the new role" },
            { "-permissions", "Permissions of the new role" },
            { "-help", "Shows usage information for the command." }
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

        // Implementing Execute as defined in the custom ICommand interface
        public void Execute(string[] args, User currentUser)
        {
            if (args.Contains("-help"))
            {
                ShowHelp();
                return;
            }

            if (args.Length > 2)
            {
                string roleName = args[1];
                string[] permissions = args.Skip(2).ToArray();

                // Create a HashSet from the entered permissions
                HashSet<string> permissionsHash = new HashSet<string>(permissions);
                userManagement.roleRepository.AddRole(new Role(roleName, permissionsHash));
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
            Console.WriteLine("Usage: AddRole [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
