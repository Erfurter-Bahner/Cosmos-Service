using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMIG.OS
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; private set; }
        public eRole Role { get; private set; }

        public User(string _Name, string _Password, eRole _Role) { 
            
            Name = _Name;
            Password = _Password;
            Role = _Role;
        
        }

        public enum eRole
        {
            User,
            Admin
        }
    }
}
