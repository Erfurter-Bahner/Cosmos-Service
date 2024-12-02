using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sys = Cosmos.System;

using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using AMIG.OS.Kernel;
using System.Linq.Expressions;
using System.Drawing;


namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class ShowSpecificRole : ICommand
    {
        private readonly UserManagement userManagement;
        private User LoggedInUser;
        public string Description => "show specific role";
        public string PermissionName { get; } = Permissions.showrole; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-role","names of role to show"},
            {"-help","Show help for this command."},
        };
        public ShowSpecificRole(UserManagement userManagement)
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
            parameters.TryGetValue("role", out string roles);
            if (!string.IsNullOrWhiteSpace(roles)) 
            {
                string[] rolesInput = roles.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var _role in rolesInput)
                {
                    // Überprüfen, ob der Benutzer existiert
                    Role role = userManagement.roleRepository.GetRoleByName(_role);
                    if (role == null)
                    {
                        Console.WriteLine($"Warning: User '{_role}' doesnt exist.");
                        return;
                    }
                    else
                    {
                        userManagement.roleRepository.DisplayRole(role);                      
                    }
                }
            }
            else
            {
                ConsoleHelpers.WriteError("Insufficient arguments. Use -help to see usage.");
            }            
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: showrole [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}
