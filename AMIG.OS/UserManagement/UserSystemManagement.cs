using AMIG.OS.Utils;
using AMIG.OS.FileManagement;
using System;
using System.Data;
using System.IO;
using Sys =Cosmos.System;
using System.Collections.Generic;

namespace AMIG.OS.UserSystemManagement
{
    public class UserManagement
    {
        private readonly UserRepository userRepository;
        private readonly AuthenticationService authService;
        private readonly RoleRepository roleRepository;
        private static FileSystemManager fileSystemManager = new FileSystemManager();

        public UserManagement()
        {
            //zum testen!!!!!
            var helpers = new Helpers(this, fileSystemManager);
            string filePath = Path.Combine(@"0:\", "role.txt");
            //fileSystemManager.DeleteFile(filePath);

            roleRepository = new RoleRepository();// RoleRepository initialisieren
            roleRepository.LoadRoles(); // Rollen laden

            //zum testen!!!!!
            fileSystemManager.ReadFile(filePath, "admin");
            roleRepository.GetRoleByName("viewLogs");


            userRepository = new UserRepository(roleRepository); // UserRepository initialisieren
            //userRepository.LoadUsers(); // Benutzer laden

            authService = new AuthenticationService(userRepository); // Authentifizierungsdienst mit geladenen Benutzern initialisieren
        }

        public bool Login(string username, string password)
        {
            return authService.Login(username, password);
        }

        public bool Register(string username, string password, string role)
        {
            return authService.Register(username, password, role);
        }

        public void RemoveUser(string username)
        {
            userRepository.RemoveUser(username);
        }

        public void AddUserWithRoleAndPassword(string username, string password, string role)
        {

            if (!authService.UserExists(username))
            {
                DateTime created = DateTime.Now;
                //var user = new User(username, password, role, DateTime.Now.ToString()); // Neues User-Objekt erstellen
                var user = new User(username, password, true);
                userRepository.AddUser(user); // Benutzer zum Repository hinzufügen
                userRepository.SaveUsers(); // Änderungen speichern
                Console.WriteLine($"Benutzer {username} mit der Rolle {role} hinzugefügt.");
            }
            else
            {
                Console.WriteLine("Benutzername existiert bereits.");
            }
        }

        public void RemoveAllUser()
        {
            userRepository.RemoveAllUsers();
            Console.WriteLine("Alle Benutzer wurden entfernt");
            userRepository.SaveUsers(); // Speichere die Änderungen
        }
        public void DisplayAllUsers()
        {
            var users = userRepository.GetAllUsers();
            foreach (var user in users.Values)
            {
                Console.WriteLine($"Username: {user.Username}," +
                    $"PW: {user.PasswordHash} " +
                    $"Role: {user.Roles}, " +
                    $"Erstellt am {user.CreatedAt}," +
                    $"Letzter Login am {user.LastLogin}");
                //user.DisplayPermissions();
            }
        }

        public bool ChangeUsername(string oldUsername, string newUsername)
        {
            return userRepository.ChangeUsername(oldUsername, newUsername);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return userRepository.ChangePassword(username, oldPassword, newPassword);
        }
        public User GetUser(string username)
        {
            // Überprüfen, ob der Benutzer existiert
            if (userRepository.users.ContainsKey(username))
            {
                return userRepository.users[username]; // Benutzer zurückgeben
            }
            else
            {
                Console.WriteLine($"Benutzer '{username}' existiert nicht.");
                return null; // Null zurückgeben, wenn der Benutzer nicht existiert
            }
        }
        public string GetPasswordHash(string username)
        {
            return userRepository.GetPasswordHash(username);
        }

        public Role GetRoleByName(string roleName)
        {
            return roleRepository.GetRoleByName(roleName);
        }

        // Zugriff auf einen Benutzer
        //public void GetUserInfo(string username)
        //{
        //    userRepository.GetUserInfo(username);
        //}

        //public string GetUserRole(string username)
        //{
        //    return userRepository.GetUserRole(username);
        //}

        public bool UserExists(string username)
        {
            return authService.UserExists(username);
        }

        //public void SetUserPermission(string requesterUsername, string username, string permission, string value )
        //{
        //    // Überprüfen, ob der anfordernde Benutzer existiert
        //    if (!userRepository.users.ContainsKey(requesterUsername))
        //    {
        //        Console.WriteLine($"Benutzer '{requesterUsername}' existiert nicht.");
        //        return;
        //    }

        //    var requesterUser = userRepository.users[requesterUsername];

        //    // Überprüfen, ob der anfordernde Benutzer Admin ist oder die Berechtigung hat
        //    if (requesterUser.Role == "Admin" || requesterUser.Permissions.ContainsKey("PermissionChange"))
        //    {
        //        // Überprüfen, ob der Benutzer, dessen Berechtigung geändert werden soll, existiert
        //        if (userRepository.users.ContainsKey(username))
        //        {
        //            // Berechtigung ändern
        //            userRepository.users[username].ChangePermission(permission, value);
        //            userRepository.SaveUsers(); // Änderungen speichern
        //            Console.WriteLine($"Berechtigung '{permission}' für Benutzer '{username}' auf '{value}' geändert.");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Benutzer '{username}' existiert nicht.");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Zugriff verweigert: Nur Admins oder Benutzer mit der Berechtigung 'PermissionChange' können Berechtigungen ändern.");
        //    }
        //}

        public void LoadUsers()
        {

            userRepository.LoadUsers();


        }
    }
}
