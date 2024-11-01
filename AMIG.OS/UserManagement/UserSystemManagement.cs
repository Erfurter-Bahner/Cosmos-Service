using System;

namespace AMIG.OS.UserSystemManagement
{
    public class UserManagement
    {
        private readonly UserRepository userRepository;
        private readonly AuthenticationService authService;
        private readonly RoleService roleService;

        public UserManagement()
        {

            userRepository = new UserRepository(); // UserRepository initialisieren
            userRepository.LoadUsers(); // Benutzer laden
            authService = new AuthenticationService(userRepository); // Authentifizierungsdienst mit geladenen Benutzern initialisieren
            roleService = new RoleService(); // Rolle-Service initialisieren
        }

        public bool Login(string username, string password)
        {
            return authService.Login(username, password);
        }

        public bool Register(string username, string password, string role)
        {
            return authService.Register(username, password, role);
        }
        /*
        public void AddUser(string username, string role, string password)
        {
            if (!authService.UserExists(username))
            {
                userRepository.AddUser(new User(username, password, role));
                userRepository.SaveUsers();
            }
        }
        */
        public void RemoveUser(string username)
        {
            userRepository.RemoveUser(username);
        }
        
        public void AddUserWithRoleAndPassword(string username, string password, string role)
        {
            if (!authService.UserExists(username))
            {
                DateTime created = DateTime.Now;
                var user = new User(username, password, role, DateTime.Now.ToString()); // Neues User-Objekt erstellen
                userRepository.AddUser(user); // Benutzer zum Repository hinzufügen
                //userRepository.SaveUsers(); // Änderungen speichern
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
                    $"Role: {user.Role}, " +
                    $"Erstellt am {user.CreatedAt},"+
                    $"Letzter Login am {user.LastLogin}");
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

        public string GetPasswordHash(string username)
        {
            return userRepository.GetPasswordHash(username);
        }

        // Zugriff auf einen Benutzer
        public void GetUserInfo(string username)
        {
            userRepository.GetUserInfo(username);
        }

        public string GetUserRole(string username)
        {
            return userRepository.GetUserRole(username);
        }

        public bool UserExists(string username)
        {
            return authService.UserExists(username);
        }
    }
}
