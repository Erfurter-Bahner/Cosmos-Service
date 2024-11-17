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

namespace AMIG.OS.CommandProcessing.Commands.extra
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

        // Implementing Execute as defined in the custom ICommand interface
        public void Execute(string[] args, User currendUser)
        {
            if (args.Contains("-help"))
            {
                ShowHelp();
                return;
            }
            if (args.Length == 2 && args[1].Equals("amigos"))
            {
                Console.Clear();
                Console.WriteLine("\n\tHASTA LA VISTA");
                Thread.Sleep(1500);
                Sys.Power.Shutdown();
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
            Console.WriteLine("Usage: adios [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
