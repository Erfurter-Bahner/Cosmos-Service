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
    public class ChangePW : ICommand
    {
        private readonly UserManagement userManagement;
        public string Description => "Change the current user's password.";
        public string PermissionName { get; } = Permissions.changepw; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-help", "Show detailed help for the 'changepw' command."},
        };

        public ChangePW(UserManagement userManagement)
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

            if (parameters.Parameters.Count > 0)
            {
                ConsoleHelpers.WriteError("Invalid parameters. Use '-help' to see the correct usage.");
                return;
            }

            // Schritt 1: Überprüfung des alten Passworts
            while (true)
            {
                Console.Write("Old password: ");
                string oldPassword = ConsoleHelpers.GetPassword();

                if (currentUser.VerifyPassword(oldPassword))
                {
                    break;
                }
                else
                {
                    ConsoleHelpers.WriteError("Error: Incorrect password. Please try again.");
                }
            }

            // Schritt 2: Neues Passwort festlegen
            string newPassword;
            while (true)
            {
                Console.Write("New password: ");
                newPassword = ConsoleHelpers.GetPassword();

                Console.Write("Confirm new password: ");
                string confirmPassword = ConsoleHelpers.GetPassword();

                if (newPassword == confirmPassword)
                {
                    break;
                }
                else
                {
                    ConsoleHelpers.WriteError("Error: Passwords do not match. Please try again.");
                }
            }

            // Schritt 3: Passwort aktualisieren
            currentUser.PasswordHash = currentUser.HashPassword(newPassword);
            userManagement.userRepository.SaveUsers();
            ConsoleHelpers.WriteSuccess("Password changed successfully.");
        }

        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: changepw [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}
