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
        public string Description => "shutdown the system";
        public string PermissionName { get; } = "ShutdownSys"; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-amigos", "needed to use shutdown command."},
        };

        // Implementing CanExecute as defined in the custom ICommand interface
        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        public void Execute(CommandParameters parameters, User currentUser)
        {
            // Wenn der 'help'-Parameter übergeben wird, zeige die Hilfe
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }

            // Prüfen, ob der Befehl "amigos" ohne weitere Argumente aufgerufen wurde
            if (parameters.Parameters.Count == 1 && parameters.Parameters.ContainsKey("amigos"))
            {
                Console.Clear();
                Console.WriteLine("\n\tHASTA LA VISTA");
                Thread.Sleep(1500);
                Sys.Power.Shutdown(); // Simuliert das Herunterfahren des Systems
            }
            else
            {
                Console.WriteLine("Fehlende oder ungültige Argumente. Verwenden Sie -help für weitere Informationen.");
            }
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: adios [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
