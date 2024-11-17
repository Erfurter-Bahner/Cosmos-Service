using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMIG.OS.Utils
{
    public interface ICommand
    {
        string Description { get; } // Beschreibung des Befehls
        string PermissionName { get; } // Name der benötigten Permission
        Dictionary<string, string> Parameters { get; } // Mögliche Parameter und ihre Beschreibungen

        void Execute(CommandParameters parameters, User currentUser);
        bool CanExecute(User currentUser);

        // Gibt eine Übersicht über den Befehl und seine Parameter zurück
        void ShowHelp();
    }
}
