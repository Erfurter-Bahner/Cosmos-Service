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
        public string PermissionName { get; } = "adduser"; // Required permission name
        public string Description => "Add a user";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            //{ "-user", "name of the new user" },
            //{ "-role", "Name of the new role/s" },           
            //{ "-help", "Shows usage information for the command." }
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
            }
            // Benutzerdaten abfragen
            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = ConsoleHelpers.GetPassword();

            // Rollen (durch Semikolon getrennt, z. B. Admin;User)
            Console.Write("Roles (durch leerzeichen getrennt): ");
            string rolesInput = Console.ReadLine();

            var roles = new List<Role>();  // Rollen als Dictionary speichern
            var userPermissions = new HashSet<string>(); // Berechtigungen für den Benutzer werden hier gesammelt

            foreach (var roleName in rolesInput.Split(' '))  // Rollen durch Semikolon trennen
            {
                var trimmedRoleName = roleName.Trim();  // Leere Leerzeichen entfernen
                Role role = userManagement.roleRepository.GetRoleByName(trimmedRoleName); // Beispielmethode, um Rolle nach Name zu finden

                if (role != null)
                {
                    Console.WriteLine($"Rolle: {role.RoleName}");
                    Console.WriteLine("Berechtigungen:");
                    foreach (var permission in role.Permissions)
                    {
                        Console.WriteLine($"- {permission}");
                    }

                    roles.Add(role);  // Rolle ins Dictionary einfügen, wobei der Schlüssel der Name der Rolle ist
                    userPermissions.UnionWith(role.Permissions);  // Berechtigungen der Rolle hinzufügen
                }
                else
                {
                    Console.WriteLine($"Rolle '{trimmedRoleName}' nicht gefunden.");
                }
            }

            // Benutzer erstellen
            var user = new User(username, password, roles: roles, permissions: userPermissions);

            // Hinzufügen des Benutzers zur Sammlung
            userManagement.userRepository.AddUser(user);  // Methode, um den Benutzer hinzuzufügen

            // Optional: Ausgabe zur Bestätigung
            Console.WriteLine("Benutzer erfolgreich hinzugefügt!");
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: addrole");
            //foreach (var param in Parameters)
            //{
            //    Console.WriteLine($"  {param.Key}\t{param.Value}");
            //}
        }
    }
}
