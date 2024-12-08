using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class AddUser : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = Permissions.adduser; // Required permission name
        public string Description => "Add a user";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            //{ "-user", "name of the new user" },
            //{ "-role", "Name of the new role/s" },           
            {"-help", "Show help for this command."},
        };

        public AddUser(UserManagement userManagement)
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
            // Hilfe anzeigen, wenn der "help"-Parameter enthalten ist
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }

            // Benutzername abfragen
            string username;
            do
            {
                Console.Write("Username: ");
                username = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(username))
                {
                    ConsoleHelpers.WriteError("Username cannot be empty. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(username));

            // Passwort abfragen
            string password;
            do
            {
                Console.Write("Password: ");
                password = ConsoleHelpers.GetPassword();
                if (string.IsNullOrWhiteSpace(password))
                {
                    ConsoleHelpers.WriteError("Password cannot be empty. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(password));

            // Rollen abfragen
            string rolesInput;
            do
            {
                Console.Write("Roles (r1 r2 r3): ");
                rolesInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(rolesInput))
                {
                    ConsoleHelpers.WriteError("Roles cannot be empty. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(rolesInput));

            // Rollen verarbeiten
            var roles = new List<Role>(); // Rollen als Liste speichern
            var userPermissions = new HashSet<string>(); // Berechtigungen für den Benutzer sammeln

            foreach (var roleName in rolesInput.Split(' ')) // Rollen durch Leerzeichen trennen
            {
                var trimmedRoleName = roleName.Trim(); // Leere Leerzeichen entfernen
                Role role = userManagement.roleRepository.GetRoleByName(trimmedRoleName); // Beispielmethode, um Rolle nach Name zu finden

                if (role != null)
                {
                    Console.WriteLine($"Role: {role.RoleName}");
                    Console.WriteLine("Permissions:");
                    foreach (var permission in role.Permissions)
                    {
                        Console.WriteLine($"- {permission}");
                    }

                    roles.Add(role); // Rolle zur Liste hinzufügen
                    userPermissions.UnionWith(role.Permissions); // Berechtigungen der Rolle hinzufügen
                }
                else
                {
                    Console.WriteLine($"Warning: Role '{trimmedRoleName}' doesn't exist.");
                }
            }

            // Benutzer erstellen
            var user = new User(username, password, roles: roles, permissions: userPermissions);

            // Benutzer zur Sammlung hinzufügen
            userManagement.userRepository.AddUser(user); // Methode, um den Benutzer hinzuzufügen

            // Optional: Ausgabe zur Bestätigung
            ConsoleHelpers.WriteSuccess("User successfully added");
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: addrole [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}
