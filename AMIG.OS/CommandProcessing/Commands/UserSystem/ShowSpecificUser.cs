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
    public class ShowSpecificUser : ICommand
    {
        private readonly UserManagement userManagement;
        private User LoggedInUser;
        public string Description => "show specific users";
        public string PermissionName { get; } = Permissions.showuser; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-user", "names of users to show"},
            {"-help", "Show help for this command."},
        };
        public ShowSpecificUser(UserManagement userManagement)
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
            parameters.TryGetValue("user", out string usernames);
            if (!string.IsNullOrWhiteSpace(usernames)) 
            {
                string[] usernamesInput = usernames.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var _user in usernamesInput)
                {
                    // Überprüfen, ob der Benutzer existiert
                    User user = userManagement.userRepository.GetUserByUsername(_user);
                    if (user == null)
                    {
                        Console.WriteLine($"Warning: User '{_user}' doesnt exist.");
                        return;
                    }
                    else
                    {
                        userManagement.DisplayUser(user);                      
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
            Console.WriteLine("Usage: showuser [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
