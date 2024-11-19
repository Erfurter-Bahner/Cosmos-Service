﻿using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class RemoveUser : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = "rmuser"; // Required permission name
        public string Description => "remove a user";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-user", "Name of the user/s" },          
            { "-help", "Shows usage information for the command." }
        };

        public RemoveUser(UserManagement userManagement)
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
            parameters.TryGetValue("user", out string usernames);

            // Wenn genügend Argumente übergeben werden
            if (!string.IsNullOrEmpty(usernames)) 
            {
                //-string username = parameters.Parameters.FirstOrDefault().Value;
                string[] usernamesInput = usernames.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var _user in usernamesInput)
                {
                     // Überprüfen, ob der Benutzer existiert
                    User user = userManagement.userRepository.GetUserByUsername(_user);
                    if (user == null)
                    {
                        Console.WriteLine($"Benutzer '{_user}' wurde nicht gefunden.");
                        return;
                    }
                    else if (currentUser.Username==_user)
                    {
                        Console.WriteLine("Du kannst dich nicht selbst löschen.");
                        return; 
                    }
                    else
                    {
                        userManagement.RemoveUser(_user);
                        Console.WriteLine($"Benutzer '{_user}' wurde entfernt.");
                    }
                }             
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
            Console.WriteLine($"Usage: {PermissionName} [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
