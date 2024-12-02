using System;
using System.Collections.Generic;

namespace AMIG.OS.CommandProcessing
{
    internal static class Permissions
    {
        
        public const string extra = nameof(extra);
        //user
        public const string addrole = nameof(addrole);
        public const string rmrole = nameof(rmrole);       
        public const string addroletouser = nameof(addroletouser);
        public const string rmroleuser = nameof(rmroleuser);
        public const string addpermtouser = nameof(addpermtouser);
        public const string rmpermuser = nameof(rmpermuser);
        public const string rmpermrole = nameof(rmpermrole);
        public const string addpermtorole = nameof(addpermtorole);
        public const string adduser = nameof(adduser);
        public const string rmuser = nameof(rmuser);
        public const string showall = nameof(showall);
        public const string showme = nameof(showme);
        public const string changename = nameof(changename);
        public const string changepw = nameof(changepw);
        public const string showuser = nameof(showuser);
        //file
        public const string ls = nameof(ls);
        public const string cat = nameof(cat);
        public const string touch = nameof(touch);
        public const string cd = nameof(cd);
        public const string write = nameof(write);
        public const string rmfile = nameof(rmfile);
        public const string rmdir = nameof(rmdir);
        public const string mkdir = nameof(mkdir);
        public const string showallperms = nameof(showallperms);
        public const string showallroles = nameof(showallroles);
        public const string showrole = nameof(showrole);



        // Liste aller Berechtigungen
        public static readonly List<string> AllPermissions = new List<string>
        {
            addrole,
            rmrole,
            addroletouser,
            rmroleuser,
            addpermtouser,
            rmpermuser,
            rmpermrole,
            addpermtorole,
            adduser,
            rmuser,
            showall,
            showme,
            changename,
            changepw,
            extra,
            ls,
            cat,
            touch,
            cd,
            write,
            rmfile,
            rmdir,
            showuser,
            mkdir,
            showallperms,
            showallroles,
            showrole,

        };



        // Prüft, ob die Berechtigung existiert (Case-Insensitive)
        public static bool IsValidPermission(string permission)
        {

            var trimmedPermission = permission.Trim();
            //Console.WriteLine($"Prüfe Berechtigung: '{trimmedPermission}'");

            foreach (var perm in AllPermissions)
            {
                if (string.Equals(perm, trimmedPermission, StringComparison.OrdinalIgnoreCase))
                {
                    //Console.WriteLine("Berechtigung gefunden.");
                    return true;
                }
            }

            //Console.WriteLine("Berechtigung nicht gefunden.");
            return false;
        }

        public static void ShowAllPermissionsHelp()
        {
            const int columnWidth = 20; // Maximale Breite pro Spalte
            const int columns = 2;     // Anzahl der Spalten

            int rows = (int)Math.Ceiling((double)AllPermissions.Count / columns);

            Console.WriteLine("Available Permissions:");
            for (int row = 0; row < rows; row++)
            {
                string line = "";
                for (int col = 0; col < columns; col++)
                {
                    int index = row + col * rows;
                    if (index < AllPermissions.Count)
                    {
                        line += AllPermissions[index].PadRight(columnWidth);
                    }
                }
                Console.WriteLine(line);
            }
        }
    }
}
