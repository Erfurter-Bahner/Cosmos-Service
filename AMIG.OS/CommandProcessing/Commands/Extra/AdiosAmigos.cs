using System.Threading;
using Sys = Cosmos.System;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;

namespace AMIG.OS.CommandProcessing.Commands.Extra
{
    public class Adios : ICommand
    {
        private readonly UserManagement userManagement;
        public string Description => "Shutdown the system.";
        public string PermissionName { get; } = Permissions.extra; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-amigos","Required to use the shutdown command."},
            {"-help","Show help for this command."}
        };

        public Adios(UserManagement userManagement)
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

            // Überprüfen, ob genau ein Parameter ("-amigos") übergeben wurde
            if (parameters.Parameters.Count == 1 && parameters.Parameters.ContainsKey("amigos"))
            {
                Console.Clear();
                ConsoleHelpers.WriteSuccess("\n\tHASTA LA VISTA");
                Thread.Sleep(1500);
                Sys.Power.Shutdown(); // Simuliert das Herunterfahren des Systems
            }
            else
            {
                ConsoleHelpers.WriteError("Missing or invalid arguments. Use '-help' for more information.");
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: adios [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}
