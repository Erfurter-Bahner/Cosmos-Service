using AMIG.OS.UserSystemManagement;
using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMIG.OS.Utils
{
    public class Helpers
    {
        private readonly UserManagement userManagement;
        public Helpers(UserManagement userManagement)
        {
            this.userManagement = userManagement;
        }

        // Neue Methode zum Hinzufügen eines Benutzers
        public void AddUserCommand()
        {
            Console.WriteLine("Name des anzulegende Benutzers");
            string username = Console.ReadLine();

            if (userManagement.UserExists(username))
            {
                do
                {
                    Console.WriteLine("Benutzer existiert bereits");
                    Console.WriteLine("Name des anzulegende Benutzers");
                    username = Console.ReadLine();
                }
                while (userManagement.UserExists(username));
            }

            Console.WriteLine("Rolle des anzulegende Benutzers: Standard oder Admin");
            string role = Console.ReadLine().ToLower();

            if (role != "admin" && role != "standard")
            {
                do
                {
                    Console.WriteLine("Rolle des anzulegende Benutzers: Standard oder Admin");
                    role = Console.ReadLine().ToLower();
                }
                while (role != "admin" && role != "standard");

            }

            // Passwortabfrage
            Console.WriteLine("Passwort des anzulegende Benutzers");
            string pw = ConsoleHelpers.GetPassword();
            string pw_2;
            do
            {
                Console.WriteLine("PW des anzulegende Benutzers wiederholen");
                pw_2 = ConsoleHelpers.GetPassword();
            } while (pw != pw_2);

            userManagement.AddUserWithRoleAndPassword(username, pw, role);
        }
    }
}
