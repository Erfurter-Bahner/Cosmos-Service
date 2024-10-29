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

        public void AddUser(string username, string role, string password)
        {
            if (!authService.UserExists(username))
            {
                userRepository.AddUser(new User(username, password, role));
                userRepository.SaveUsers();
            }
        }

        public void RemoveUser(string username)
        {
            userRepository.RemoveUser(username);
            userRepository.SaveUsers();
        }

        public void DisplayAllUsers()
        {
            var users = userRepository.GetAllUsers();
            foreach (var user in users.Values)
            {
                Console.WriteLine($"Username: {user.Username}, Role: {user.Role}");
            }
        }

        public bool UserExists(string username)
        {
            return authService.UserExists(username);
        }
    }
}
