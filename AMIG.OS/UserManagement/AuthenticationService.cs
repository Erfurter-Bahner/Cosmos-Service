using System;

namespace AMIG.OS.UserSystemManagement
{
    public class AuthenticationService
    {
        private readonly UserRepository userRepository;

        public AuthenticationService(UserRepository repository)
        {
            userRepository = repository;
        }

        // Login-Prozess
        public bool Login(string username, string password)
        {
            var user = userRepository.GetUserByUsername(username);
            if (user != null && user.VerifyPassword(password))
            {
                user.LastLogin = DateTime.Now.ToString(); // Datum der letzten Anmeldung aktualisieren
                userRepository.SaveUsers(); // Änderungen speichern
                return true;
            }
            return false;
        }

        // Registrierungsprozess
        public bool Register(string username, string password, string role)
        {
            if (userRepository.GetUserByUsername(username) == null)
            {
                var newUser = new User(username, password, role, DateTime.Now.ToString());
                userRepository.AddUser(newUser);
                //userRepository.SaveUsers(); // Benutzer sofort speichern
                return true;
            }
            Console.WriteLine("Username already taken.");
            return false;
        }

        // Prüfen, ob ein Benutzername existiert
        public bool UserExists(string username)
        {
            return userRepository.GetUserByUsername(username) != null;
        }
    }
}
