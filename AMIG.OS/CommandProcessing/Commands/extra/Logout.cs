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


namespace AMIG.OS.CommandProcessing.Commands.extra
{
    public class Logout : ICommand
    {
        private readonly UserManagement userManagement;
        private User LoggedInUser;
        public string Description => "logout to start screen";
        public string PermissionName { get; } = "LogOutSys"; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"logout", "to logout of current user."},
        };
        public Logout(UserManagement userManagement)
        {
            this.userManagement = userManagement;
        }
        // Implementing CanExecute as defined in the custom ICommand interface
        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        // Implementing Execute as defined in the custom ICommand interface
        public void Execute(string[] args, User currendUser)
        {
            if (args.Contains("-help"))
            {
                ShowHelp();
            }
            if (args.Length == 1) {

                userManagement.loginManager.ShowLoginOptions();
                
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
            Console.WriteLine("Usage: logout [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
