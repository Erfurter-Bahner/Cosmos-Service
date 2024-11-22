using System;
using System.Collections.Generic;

namespace AMIG.OS.CommandProcessing
{
    internal static class Permissions
    {
        public const string addrole = nameof(addrole);
        public const string rmrole = nameof(rmrole);
        public const string extra = nameof(extra);
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
            extra
        };

        // Prüft, ob die Berechtigung existiert (Case-Insensitive)
        public static bool IsValidPermission(string permission)
        {

            var trimmedPermission = permission.Trim();
            Console.WriteLine($"Prüfe Berechtigung: '{trimmedPermission}'");

            foreach (var perm in AllPermissions)
            {
                if (string.Equals(perm, trimmedPermission, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Berechtigung gefunden.");
                    return true;
                }
            }

            Console.WriteLine("Berechtigung nicht gefunden.");
            return false;
        }
    }
}
