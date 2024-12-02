using System.Threading;
using Sys = Cosmos.System;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;

namespace AMIG.OS.CommandProcessing.Commands.Extra
{
    public class Clear : ICommand
    {
        private readonly UserManagement userManagement;
        public string Description => "Clear the screen.";
        public string PermissionName { get; } = Permissions.extra; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
             {"-help","Show help for this command."}
        };

        public Clear(UserManagement userManagement)
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

            // Überprüfen, ob keine zusätzlichen Parameter angegeben wurden
            if (parameters.Parameters.Count == 0)
            {
                Console.Clear();                
            }
            else
            {
                ConsoleHelpers.WriteError("Invalid arguments. Use '-help' to see the correct usage.");
            }
        }

        public void ShowHelp()
        {
            ConsoleHelpers.WriteSuccess(Description);
            Console.WriteLine("Usage: clear [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}
