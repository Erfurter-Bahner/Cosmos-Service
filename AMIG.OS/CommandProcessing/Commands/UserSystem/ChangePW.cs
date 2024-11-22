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
    public class ChangePW : ICommand
    {
        private readonly UserManagement userManagement;
        private User LoggedInUser;
        public string Description => "change password";
        public string PermissionName { get; } = "changepassword"; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-help", "to show help"},
        };
        public ChangePW(UserManagement userManagement)
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

            if (parameters.Parameters.Count == 0)
            {
                // Schritt 1: Überprüfung des alten Passworts
                while (true)
                {
                    Console.Write("Altes Passwort: ");
                    string oldPassword = ConsoleHelpers.GetPassword();

                    // Überprüfen, ob das eingegebene Passwort korrekt ist
                    if (currentUser.VerifyPassword(oldPassword))
                    {
                        break; // Passwort korrekt, Schleife verlassen
                    }
                    else
                    {
                        Console.WriteLine("Falsches Passwort. Bitte erneut versuchen.");
                    }
                }

                // Schritt 2: Neues Passwort festlegen
                string newPassword;
                while (true)
                {
                    Console.Write("Neues Passwort: ");
                    newPassword = ConsoleHelpers.GetPassword();

                    Console.Write("Neues Passwort bestätigen: ");
                    string confirmPassword = ConsoleHelpers.GetPassword();

                    // Prüfen, ob beide Eingaben übereinstimmen
                    if (newPassword == confirmPassword)
                    {
                        break; // Beide Passwörter stimmen überein
                    }
                    else
                    {
                        Console.WriteLine("Passwörter stimmen nicht überein. Bitte erneut versuchen.");
                    }
                }

                // Schritt 3: Passwort aktualisieren
                currentUser.PasswordHash = currentUser.HashPassword(newPassword);
                userManagement.userRepository.SaveUsers();
                Console.WriteLine("Passwort erfolgreich geändert.");
            }
            else
            {
                Console.WriteLine("Falsche Parameter. Verwenden Sie '-help', um die richtige Nutzung anzuzeigen.");
            }
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: changepw [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
