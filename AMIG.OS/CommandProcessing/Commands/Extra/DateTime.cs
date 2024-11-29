using Sys = Cosmos.System;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;

namespace AMIG.OS.CommandProcessing.Commands.Extra
{
    public class DateTime : ICommand
    {
        public string Description => "Display the current date and time.";
        public string PermissionName { get; } = Permissions.extra; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
             {"-help", "Show help for this command."}
        };

        public DateTime() { }

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
                    // Zeigt das aktuelle Datum und die Uhrzeit an
                    ConsoleHelpers.WriteSuccess($"Current Date and Time: {System.DateTime.Now}");
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung für potenzielle Probleme mit Systemzeit
                    ConsoleHelpers.WriteError($"Error retrieving date and time: {ex.Message}");
                }
            }
            else
            {
                ConsoleHelpers.WriteError("Invalid arguments. Use '-help' to see the correct usage.");
            }
        }

        public void ShowHelp()
        {
            ConsoleHelpers.WriteSuccess(Description);
            Console.WriteLine("Usage: datetime [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
