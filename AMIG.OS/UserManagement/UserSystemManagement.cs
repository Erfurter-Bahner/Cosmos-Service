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
            userRepository = new UserRepository();
            authService = new AuthenticationService(userRepository);
            roleService = new RoleService();
            userRepository.LoadUsers();
        }

        public bool Login(string username, string password)
        {
            return authService.Login(username, password);
        }

        public bool Register(string username, string password, UserRole role)
        {
            return authService.Register(username, password, role);
        }

        public void AddUser(string username, UserRole role, string password)
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
            foreach (var user in users)
            {
                System.Console.WriteLine($"Username: {user.Username}, Role: {user.Role}");
            }
        }

        public bool UserExists(string username)
        {
            return authService.UserExists(username);
        }
    }
}
