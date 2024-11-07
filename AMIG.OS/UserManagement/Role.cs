using System.Collections.Generic;

namespace AMIG.OS.UserSystemManagement
{ 
    public class Role
    {
        public string RoleName { get; private set; }
        public HashSet<string> Permissions { get; private set; }

        //public Dictionary<string, HashSet<string>> roleanperms;
        public Role(string roleName, HashSet<string> permissions)
        {
            RoleName = roleName;
            Permissions = permissions;
        }

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }
    }
}


//using System.Data;
//using System;
//namespace AMIG.OS.UserSystemManagement
//{

//    public class RoleService
//    {
//        //braucht zugriff auf Userclass und Userrepository
//        private readonly UserRepository userRepository;

//        public RoleService(UserRepository userRepository)
//        {
//            this.userRepository = userRepository;
//        }

//        public void SetUserPermission( string username, string permission, string value)
//        {
//                // Überprüfen, ob der Benutzer, dessen Berechtigung geändert werden soll, existiert
//                if (userRepository.users.ContainsKey(username))
//                {
//                    // Berechtigung ändern
//                    userRepository.users[username].ChangePermission(permission, value);
//                    userRepository.SaveUsers(); // Änderungen speichern
//                    Console.WriteLine($"Berechtigung '{permission}' für Benutzer '{username}' auf '{value}' geändert.");
//                }
//                else
//                {
//                    Console.WriteLine($"Benutzer '{username}' existiert nicht.");
//                }
//        }
//    }
//}

