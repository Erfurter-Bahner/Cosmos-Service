using AMIG.OS.Utils;
using System;
using System.Linq;

namespace AMIG.OS.UserSystemManagement
{
    public class LoginManager
    {
        private RoleRepository roleRepository;
        private UserRepository userRepository;
        private AuthenticationService authService;
        internal User LoggedInUser;
        public LoginManager(RoleRepository roleRepository, UserRepository userRepository, AuthenticationService authService)
        {
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
            this.authService = authService;
        }
        public void ShowLoginOptions()
        {
            //userManagement.DisplayAllUsers(); // nur zum testen
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Select an option: ");
            Console.WriteLine("[1] Login");
            Console.WriteLine("[2] Register");
            Console.ResetColor();
            var key = Console.ReadKey(intercept: true);
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    Helper.AdiosHelper();
                    break;
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Login();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Register();
                    ShowLoginOptions();
                    break;
                default:
                    ConsoleHelpers.WriteError("Error: Invalid option! Please use a Number as an Input");
                    ShowLoginOptions();
                    break;
            }
        }

        private void Login()
        {
            Console.WriteLine("");
            Console.Write("Username: ");
            var username = Console.ReadLine();
            // Prüfen, ob der Benutzername leer oder nur Leerzeichen ist
            if (string.IsNullOrWhiteSpace(username))
            {
                ConsoleHelpers.WriteError("Error: Username cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            Console.Write("Password: ");
            var password = ConsoleHelpers.GetPassword();

            // Prüfen, ob das Passwort leer oder nur Leerzeichen ist
            if (string.IsNullOrWhiteSpace(password))
            {
                ConsoleHelpers.WriteError("Error: Password cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            if (authService.Login(username, password))
            {
                LoggedInUser = userRepository.GetUserByUsername(username);
                if (LoggedInUser == null)
                {                
                    ConsoleHelpers.WriteError("Error: User doesnt exist");                   
                }
                Console.WriteLine($"User: {LoggedInUser.Username} with role {string.Join(", ", LoggedInUser.Roles.Select(r => r.RoleName))}");
                //commandHandler.SetStartTime(DateTime.Now); // Startzeit setzen
                
                ConsoleHelpers.WriteSuccess("Login successful!");
                
                // Systemstart fortsetzen
            }
            else
            {              
                ConsoleHelpers.WriteError("Error: Invalid credentials. Try again.");                
                ShowLoginOptions();
            }
        }

        private void Register()
        {
            Console.WriteLine("");
            Console.Write("Username: ");
            var username = Console.ReadLine();
            // Prüfen, ob der Benutzername leer oder nur Leerzeichen ist
            if (string.IsNullOrWhiteSpace(username))
            {
                ConsoleHelpers.WriteError("Error: Username cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            Console.Write("Password: ");
            var password = ConsoleHelpers.GetPassword();

            // Prüfen, ob das Passwort leer oder nur Leerzeichen ist
            if (string.IsNullOrWhiteSpace(password))
            {
                ConsoleHelpers.WriteError("Error: Password cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            if (authService.Register(username, password))
            {

                ConsoleHelpers.WriteSuccess("Registration successful! Please log in.");
               
            }
            else
            {               
                ConsoleHelpers.WriteError("Error: Registration failed. Username may already exist.");
            }
        }
    }
}
