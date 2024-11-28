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
    public class ChangeName : ICommand
    {
        private readonly UserManagement userManagement;
        private User LoggedInUser;
        public string Description => "change username";
        public string PermissionName { get; } = Permissions.changename; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-help", "to show help"},
        };
        public ChangeName(UserManagement userManagement)
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

                string newUsername;
                string entscheidung;

                do
                {
                    Console.Write("Neuer Benutzername: ");
                    newUsername = Console.ReadLine();

                    do
                    {
                        Console.Write("Bestätigen: y/n ");
                        entscheidung = Console.ReadLine().ToLower();

                        if (entscheidung == "n")
                        {
                            Console.WriteLine("Benutzername nicht bestätigt. Bitte erneut eingeben.");
                            break;  // Schleife verlassen, um neuen Benutzernamen einzugeben
                        }
                        else if (entscheidung != "y" && entscheidung != "n")
                        {
                            Console.WriteLine("Ungültige Eingabe: y/n ");
                        }

                    } while (entscheidung != "y" && entscheidung != "n");

                } while (entscheidung != "y");
                currentUser.Username = newUsername;
                userManagement.userRepository.SaveUsers();
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
            Console.WriteLine("Usage: changename [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
