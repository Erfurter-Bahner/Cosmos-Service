using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;
using System.Text;
using Sys = Cosmos.System;
using System.Security.Cryptography;
using System.Drawing;

namespace AMIG.OS
{
    public class Kernel : Sys.Kernel
    {

        protected override void BeforeRun()
        {
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");

            UserManagement userManagement = new UserManagement();

            // Benutzer hinzufügen (mit Passwort)
            userManagement.AddUser("User1", "Standard", "password123");
            userManagement.AddUser("User2", "Admin", "adminPass");

            // Alle Benutzer anzeigen
            userManagement.DisplayAllUsers();
            
            // Informationen über einen bestimmten Benutzer abrufen
            userManagement.GetUserInfo("User1");                                       
        }

        protected override void Run()

        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            Console.Write("Text typed: ");
            Console.WriteLine(input);
        }

    }

}
