using AMIG.OS.CommandProcessing.Commands.FileSystem;
using AMIG.OS.CommandProcessing.Commands.UserSystem;
using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Threading;
using Sys = Cosmos.System;

namespace AMIG.OS.Utils
{
    public static class Helper
    {    
        public static String preInput = "> ";

        public static void AdiosHelper()
        {
            Console.Clear();
            Console.WriteLine("\n\tHASTA LA VISTA");
            Thread.Sleep(1500);
            Sys.Power.Shutdown();
        }


        public static readonly List<string> Argumente = new List<string>
        {
            "-user",
            "-permissions",
            "-role",
            "-file",
            "-dir",
            "-help",
        };

        public static HashSet<string> SpecialCommands = new HashSet<string>
        {
            "clear",
            "adios",
            "datetime",
            "logout"
        };



        // Prüft, ob die Berechtigung existiert (Case-Insensitive)
        public static bool IsValidArgument(string permission)
        {

            var trimmedPermission = permission.Trim();
            //Console.WriteLine($"Prüfe Berechtigung: '{trimmedPermission}'");

            foreach (var arg in Argumente)
            {
                if (string.Equals(arg, trimmedPermission, StringComparison.OrdinalIgnoreCase))
                {
                    //Console.WriteLine("Berechtigung gefunden.");
                    return true;
                }
            }

            //Console.WriteLine("Berechtigung nicht gefunden.");
            return false;
        }
    }

}
