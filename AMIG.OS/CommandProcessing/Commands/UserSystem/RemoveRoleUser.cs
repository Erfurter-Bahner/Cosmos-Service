﻿using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class RemoveRoleUser : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = "rmroleuser"; // Required permission name
        public string Description => "remove a role from a user";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-user", "Name of the user" },
            { "-role", "Name of the role/s to remove" },
            { "-help", "Shows usage information for the command." }
        };

        public RemoveRoleUser(UserManagement userManagement)
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
                    Console.WriteLine($"Benutzer '{username}' wurde nicht gefunden.");
                    return;
                }

                // Die restlichen Argumente als Rollennamen behandeln

                if (parameters.TryGetValue("role", out string roleNames) && !string.IsNullOrEmpty(roleNames))
                {
                    string[] rolesNameInputs = roleNames.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    // Für jede Rolle überprüfen und hinzufügen
                    foreach (string roleName in rolesNameInputs)
                    {
                        Role role = userManagement.roleRepository.GetRoleByName(roleName);

                        // Überprüfen, ob die Rolle existiert
                        if (role == null)
                        {
                            Console.WriteLine($"Rolle '{roleName}' existiert nicht.");
                            continue; // Nächste Rolle prüfen
                        }

                        // Überprüfen, ob der Benutzer die Rolle bereits hat
                        if (!user.Roles.Contains(role))
                        {
                            Console.WriteLine($"Benutzer '{username}' hat nicht die Rolle '{roleName}'.");
                            continue;
                        }

                        // Rolle und Berechtigungen zum Benutzer hinzufügen
                        user.RemoveRole(role);
                        Console.WriteLine($"Rolle '{roleName}' wurde erfolgreich vom Benutzer '{username}' entfernt.");
                    }

                    // Benutzer speichern
                    userManagement.userRepository.SaveUsers();
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