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
    public class ShowAllPermissions : ICommand
    {
        private readonly UserManagement userManagement;
        private User LoggedInUser;
        public string Description => "show all permissions";
        public string PermissionName { get; } = Permissions.showallperms; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-help","Show help for this command."},
        };
        public ShowAllPermissions(UserManagement userManagement)
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
            if (parameters.Parameters.Count == 0) {

                Permissions.ShowAllPermissionsHelp();
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
            Console.WriteLine("Usage: showallperms [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}
