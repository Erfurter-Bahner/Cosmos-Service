using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMIG.OS.Utils
{
    public class LoginManager
    {
        private RoleRepository roleRepository;
        private UserRepository userRepository;
        private AuthenticationService authService;
        internal  User LoggedInUser;
        public LoginManager(RoleRepository roleRepository, UserRepository userRepository, AuthenticationService authService)
        {
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
            this.authService = authService;
        }
        public void ShowLoginOptions()
        {
            //userManagement.DisplayAllUsers(); // nur zum testen
            Console.WriteLine("Select an option: ");
            Console.WriteLine("[1] Login");
            Console.WriteLine("[2] Register");
            var key = Console.ReadKey(intercept: true);
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    //helpers.AdiosHelper();
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
                    Console.WriteLine("Invalid option! Please use a Number as an Input");
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
                Console.WriteLine("Username cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            Console.Write("Password: ");
            var password = ConsoleHelpers.GetPassword();

            // Prüfen, ob das Passwort leer oder nur Leerzeichen ist
            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Password cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            if (authService.Login(username, password))
            {
                LoggedInUser = userRepository.GetUserByUsername(username);
                if (LoggedInUser == null)
                {
                    Console.WriteLine("Benutzer nicht gefunden");
                }
                Console.WriteLine($"Der eingeloggte User heißt: {LoggedInUser.Username} mit der Rolle {string.Join(", ", LoggedInUser.Roles.Select(r => r.RoleName))}");
                //commandHandler.SetStartTime(DateTime.Now); // Startzeit setzen
                Console.WriteLine("Login successful!");
                // Systemstart fortsetzen
            }
            else
            {
                Console.WriteLine("Invalid credentials. Try again.");
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
                Console.WriteLine("Username cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            Console.Write("Password: ");
            var password = ConsoleHelpers.GetPassword();

            // Prüfen, ob das Passwort leer oder nur Leerzeichen ist
            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Password cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            Console.Write("Choose a role (Admin or Standard): ");
            var roleInput = Console.ReadLine().ToLower();
            if (string.IsNullOrWhiteSpace(roleInput))
            {
                Console.WriteLine("Role cannot be empty. Please try again.");
                ShowLoginOptions();
                return;
            }

            if (authService.Register(username, password, roleInput))
            {
                Console.WriteLine("Registration successful! Please log in.");
            }
            else
            {
                Console.WriteLine("Registration failed. Username may already exist.");
            }

        }
    }
}
