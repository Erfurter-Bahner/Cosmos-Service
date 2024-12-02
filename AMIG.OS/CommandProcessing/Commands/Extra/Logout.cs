using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;

namespace AMIG.OS.CommandProcessing.Commands.Extra
{
    public class Logout : ICommand
    {
        private readonly UserManagement userManagement;
        public string Description => "Logs out the current user and returns to the login screen.";
        public string PermissionName { get; } = Permissions.extra; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
             {"-help","Show help for this command."}
        };

        public Logout(UserManagement userManagement)
        {
            this.userManagement = userManagement;
        }

        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        public void Execute(CommandParameters parameters, User currentUser)
        {
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }

            if (parameters.Parameters.Count == 0)
            {
                try
                {
                    userManagement.loginManager.ShowLoginOptions();
                    ConsoleHelpers.WriteSuccess("You have successfully logged out.");
                    CommandHistory.commandHistory.Clear();
                }
                catch (Exception ex)
                {
                    ConsoleHelpers.WriteError($"An error occurred during logout: {ex.Message}");
                }
            }
            else
            {
                ConsoleHelpers.WriteError("Invalid arguments. Use '-help' to see usage instructions.");
            }
        }

        public void ShowHelp()
        {
            ConsoleHelpers.WriteSuccess(Description);
            Console.WriteLine("Usage: logout [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}
