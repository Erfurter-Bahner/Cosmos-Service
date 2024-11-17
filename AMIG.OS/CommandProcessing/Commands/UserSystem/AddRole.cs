using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMIG.OS.CommandProcessing.Commands.UserSystem
{
    public class AddRole : ICommand
    {
        private readonly UserManagement userManagement;
        public string PermissionName { get; } = "addrole"; // Required permission name
        public string Description => "Add a role";

        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            { "-rolename", "Name of the new role" },
            { "-permissions", "Permissions of the new role" },
            { "-help", "Shows usage information for the command." }
        };

        public AddRole(UserManagement userManagement)
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
            if (parameters==null)
            {
                Console.WriteLine("parameters null in addrole");
            }
            // Hilfe anzeigen, wenn der "help"-Parameter enthalten ist
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();                
            }


            if (parameters.TryGetValue("permissions", out string permissionsRaw))
            {
                Console.WriteLine($"Gefundene Berechtigungen: {permissionsRaw}");

            }
            else
            {
                Console.WriteLine("Debug: Der Schlüssel 'permissions' wurde nicht gefunden.");
            }
            // Überprüfen, ob der notwendige Parameter "role" vorhanden ist
            if (parameters.TryGetValue("role", out string roleName))
            {
                Console.WriteLine($"Gefundene Berechtigungen: {roleName}");
                // Überprüfen, ob der "permissions"-Parameter vorhanden ist
                //if (parameters.TryGetValue("permissions".Trim(), out string permissionsRaw))
                //{
                Console.WriteLine($"Gefundene Berechtigungen: {permissionsRaw}");
                // Erstelle ein HashSet der angegebenen Berechtigungen
                var permissions = permissionsRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                HashSet<string> permissionsHash = new HashSet<string>(permissions);

                // Neue Rolle hinzufügen
                userManagement.roleRepository.AddRole(new Role(roleName, permissionsHash));
                Console.WriteLine($"Rolle '{roleName}' mit Berechtigungen '{string.Join(", ", permissions)}' hinzugefügt.");
                //}
                //else
                //{
                //    Console.WriteLine("Fehler: Es wurden keine Berechtigungen angegeben. Verwenden Sie -permissions <Liste>");
                //}
            }
            else
            {
                Console.WriteLine("Fehler: Der Parameter '-role' fehlt. Verwenden Sie -help, um die Syntax zu sehen.");
            }
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: AddRole [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
