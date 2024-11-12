using System;

namespace AMIG.OS.UserSystemManagement
{
    public class AuthenticationService
    {
        private readonly UserRepository userRepository;
        private readonly RoleRepository roleRepository; 

        public AuthenticationService(UserRepository userRepository, RoleRepository roleRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
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
        public bool Register(string username, string password, string roleName)
        {
            // Rolle aus dem Rollen-Repository abrufen
            var roleToAssign = roleRepository.GetRoleByName(roleName);

            // Überprüfen, ob die Rolle existiert
            if (roleToAssign == null)
            {
                Console.WriteLine("Die angegebene Rolle existiert nicht.");
                return false;
            }

            // Überprüfen, ob der Benutzername bereits vergeben ist
            if (userRepository.GetUserByUsername(username) == null)
            {
                // Benutzer mit der angegebenen Rolle erstellen
                var newUser = new User(username, password, false);
                newUser.AddRole(roleToAssign);  // Rolle hinzufügen

                // Benutzer zum Repository hinzufügen und sofort speichern
                userRepository.AddUser(newUser);
                //userRepository.SaveUsers(); // Benutzer sofort speichern

                Console.WriteLine("Registrierung erfolgreich. Rolle zugewiesen.");
                return true;
            }

            Console.WriteLine("Benutzername ist bereits vergeben.");
            return false;
        }


        // Prüfen, ob ein Benutzername existiert
        public bool UserExists(string username)
        {
            return userRepository.GetUserByUsername(username) != null;
        }
    }
}
