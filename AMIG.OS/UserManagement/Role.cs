using System.Collections.Generic;

namespace AMIG.OS.UserSystemManagement
{
    // Klasse, die eine Rolle im System repräsentiert
    public class Role
    {
        // Name der Rolle (z. B. "Admin", "StandardUser")
        public string RoleName { get; private set; }

        // Berechtigungen der Rolle als Menge (HashSet), z. B. {"createUser", "deleteUser"}
        public HashSet<string> Permissions { get; set; }

        // Konstruktor, der den Namen der Rolle und eine Menge von Berechtigungen initialisiert
        public Role(string roleName, HashSet<string> permissions)
        {
            RoleName = roleName;          // Setzt den Namen der Rolle
            Permissions = permissions;     // Setzt die Berechtigungen
        }

        // Überprüft, ob eine bestimmte Berechtigung in der Menge der Berechtigungen enthalten ist
        //public bool HasPermission(string permission)
        //{
        //    return Permissions.Contains(permission);
        //}
    }
}

