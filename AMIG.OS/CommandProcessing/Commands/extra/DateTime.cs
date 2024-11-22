using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sys = Cosmos.System;

using AMIG.OS.UserSystemManagement;
using AMIG.OS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using AMIG.OS.Kernel;
using System.Linq.Expressions;
using System.Drawing;


namespace AMIG.OS.CommandProcessing.Commands.extra
{
    public class DateTime : ICommand
    {
        private readonly UserManagement userManagement;
        private User LoggedInUser;
        public string Description => "show current date";
        public string PermissionName { get; } = "datetime"; // Required permission name
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"datetime", "show current date"},
        };
        public DateTime()
        {
            
        }
        // Implementing CanExecute as defined in the custom ICommand interface
        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        // Implementing Execute as defined in the custom ICommand interface
        public void Execute(CommandParameters parameters, User currentUser)
        {
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
            }
            if (parameters.Parameters.Count == 0) {

                Console.WriteLine(System.DateTime.Now);

            }
            else
            {
                Console.WriteLine("Insufficient arguments. Use -help to see usage.");
            }            
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: datetime [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}
