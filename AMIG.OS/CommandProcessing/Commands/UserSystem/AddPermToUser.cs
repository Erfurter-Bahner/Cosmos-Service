using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class AddPermToUser : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = Permissions.addpermtouser; // Required permission name
        public string Description => "add a perm to a user";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-user", "Name of the user" },
            { "-permissions", "Permissions to add to user" },
            {"-help", "Show help for this command."},
        };

        public AddPermToUser(UserManagement userManagement)
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
            // Wenn der 'help'-Parameter übergeben wird, zeige die Hilfe
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }

            // Wenn genügend Argumente übergeben werden
            if (parameters.TryGetValue("user", out string username) && !string.IsNullOrEmpty(username)) //anpassem 
            {
                // Benutzername ist das erste Argument
                //-string username = parameters.Parameters.FirstOrDefault().Value;

                // Überprüfen, ob der Benutzer existiert
                User user = userManagement.userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    ConsoleHelpers.WriteError($"User '{username}' not found.");
                    return;
                }

                // Die restlichen Argumente als Rollennamen behandeln
                if (parameters.TryGetValue("permissions", out string permNames) && !string.IsNullOrEmpty(permNames))
                {
                    string[] permNameInputs = permNames.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    // Hinzufügen der Berechtigungen
                    foreach (string permName in permNameInputs)
                    {
                        // Überprüfen, ob die Berechtigung gültig ist
                        if (!Permissions.IsValidPermission(permName))
                        {
                            Console.WriteLine($"Warning: '{permName}' isnt a known permission and was skipped.");
                            continue;
                        }

                        // Überprüfen, ob der Benutzer die Berechtigung bereits hat
                        if (user.Permissions.Contains(permName))
                        {
                            Console.WriteLine($"Warning: User '{username}' already has the permission '{permName}'.");
                            continue;
                        }

                        // Berechtigung hinzufügen
                        user.AddPermission(permName);
                        userManagement.userRepository.SaveUsers();
                        ConsoleHelpers.WriteSuccess($"Permission '{permName}' was added to '{username}' successfully.");
                    }
                }
                else
                {
                    ConsoleHelpers.WriteError("Insufficient arguments. Use -help to see usage.");
                }
            }
        }
            // Show help method as defined in the custom ICommand interface
            public void ShowHelp()
            {
                Console.WriteLine(Description);
                Console.WriteLine($"Usage: addpermuser [options]");
                foreach (var param in Parameters)
                {
                    Console.WriteLine($"  {param.Key}\t{param.Value}");
                }
            }        
    }
}
