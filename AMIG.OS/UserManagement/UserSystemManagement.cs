using AMIG.OS.Utils;
using AMIG.OS.FileManagement;
using System;
using System.IO;
using System.Linq;

namespace AMIG.OS.UserSystemManagement
{
    // Zentraler Dienst für Benutzerverwaltung, Rollen- und Berechtigungsmanagement
    public class UserManagement
    {
        public UserRepository userRepository { get; private set; }  // Verwaltet alle Benutzerinformationen
        public AuthenticationService authService { get; private set; } // Zuständig für Authentifizierung
        public RoleRepository roleRepository { get; private set; }  // Speichert Rollen und Berechtigungen
        public LoginManager loginManager { get; private set; }

        public UserManagement()
        {
            // Initialisiere Repositorys und den Authentifizierungsdienst
            this.roleRepository = new RoleRepository();
            this.userRepository = new UserRepository(roleRepository);
            this.authService = new AuthenticationService(userRepository, roleRepository);
            this.loginManager = new LoginManager(roleRepository,userRepository,authService);
        }

        // Führt die Benutzeranmeldung durch
        public bool Login(string username, string password)
        {
            return authService.Login(username, password);
        }

        public Role GetRoleByName(string roleName)
        {
           return roleRepository.GetRoleByName(roleName);   
        }

        // Registriert einen neuen Benutzer mit Benutzernamen, Passwort und zugewiesener Rolle
        public bool Register(string username, string password, string role)
        {
            return authService.Register(username, password, role);
        }

        // Entfernt einen Benutzer aus dem System
        public void RemoveUser(string username)
        {
            userRepository.RemoveUser(username);
        }

        // Fügt einen Benutzer mit einer Rolle und einem Passwort hinzu, falls er noch nicht existiert
        public void AddUserWithRoleAndPassword(string username, string password, string role)
        {
            if (!authService.UserExists(username))
            {
                var user = new User(username, password, isHashed: true); // Neues Benutzerobjekt mit Passwort-Hash erstellen
                userRepository.AddUser(user); // Benutzer zum Repository hinzufügen
                Console.WriteLine($"Benutzer {username} mit der Rolle {role} hinzugefügt.");
            }
            else
            {
                Console.WriteLine("Benutzername existiert bereits.");
            }
        }

        // Entfernt alle Benutzer aus dem System
        public void RemoveAllUser()
        {
            userRepository.RemoveAllUsers();
            Console.WriteLine("Alle Benutzer wurden entfernt.");
        }

        // Gibt Informationen aller Benutzer in der Konsole aus
        public void DisplayAllUsers()
        {
            var users = userRepository.GetAllUsers();
            foreach (var user in users.Values)
            {
                // Prüfen, ob der Benutzer Rollen oder Berechtigungen hat
                string rolesDisplay = user.Roles != null && user.Roles.Count > 0
                    ? string.Join(", ", user.Roles.Select(r => r.RoleName))
                    : "Keine Rollen"; // Anzeige "Keine Rollen", falls leer

                string permissionsDisplay = user.Permissions != null && user.Permissions.Count > 0
                    ? string.Join(", ", user.Permissions)
                    : "Keine Berechtigungen"; // Anzeige "Keine Berechtigungen", falls leer

                Console.WriteLine($"Username: {user.Username}, " +
                    $"PW: {user.PasswordHash}, " +
                    $"Role: {rolesDisplay}, " +
                    $"Erstellt am: {user.CreatedAt}, " +
                    $"Letzter Login: {user.LastLogin}, " +
                    $"Perm: {permissionsDisplay}");
            }
        }

        public void DisplayUser(string loggedInUser)
        {
            var user = userRepository.GetUserByUsername(loggedInUser);

            // Überprüfen, ob der Benutzer gefunden wurde
            if (user != null)
            {
                // Prüfen, ob der Benutzer Rollen oder Berechtigungen hat
                string rolesDisplay = user.Roles != null && user.Roles.Count > 0
                    ? string.Join(", ", user.Roles.Select(r => r.RoleName))
                    : "Keine Rollen"; // Anzeige "Keine Rollen", falls leer

                string permissionsDisplay = user.Permissions != null && user.Permissions.Count > 0
                    ? string.Join(", ", user.Permissions)
                    : "Keine Berechtigungen"; // Anzeige "Keine Berechtigungen", falls leer

                Console.WriteLine($"Username: {user.Username}, " +
                    $"PW: {user.PasswordHash}, " +
                    $"Role: {rolesDisplay}, " +
                    $"Erstellt am: {user.CreatedAt}, " +
                    $"Letzter Login: {user.LastLogin}, " +
                    $"Perm: {permissionsDisplay}");
            }
            else
            {
                Console.WriteLine($"Benutzer '{loggedInUser}' wurde nicht gefunden.");
            }
        }




        // Ändert das Passwort eines Benutzers, sofern das alte Passwort korrekt ist
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return userRepository.ChangePassword(username, oldPassword, newPassword);
        }

        // Gibt das Benutzerobjekt anhand des Benutzernamens zurück, falls vorhanden
        public User GetUser(string username)
        {
            if (userRepository.users.ContainsKey(username))
            {
                return userRepository.users[username];
            }
            else
            {
                Console.WriteLine($"Benutzer '{username}' existiert nicht.");
                return null;
            }
        }

        // Überprüft, ob ein Benutzer im System existiert
        public bool UserExists(string username)
        {
            return authService.UserExists(username);
        }


 
        


    }
}
