using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;

namespace AMIG.OS
{
    public class Kernel : Sys.Kernel
    {
        DateTime start;
        protected override void BeforeRun()
        {
            Console.WriteLine("\n\t\t\t _____\r\n\t\t\t/     \\\r\n\t_______/_______\\_______\r\n\t\\\\______AMIG.OS______//\n");
        }

        protected override void Run()
        {
            Console.Write(">> ");
            var input = Console.ReadLine();
            string[] args = input.Split(' ');


            switch (args[0])
            {
                case "help":
                    {
                        Console.WriteLine("for futher information contact us");
                        break;
                    }

                case "adios":
                    {
                        if (args.GetLength(0) == 2 && args[1].Equals("amigos"))
                        {
                            Console.WriteLine("\n\tHASTA LA VISTA");
                            Thread.Sleep(1500);
                            Sys.Power.Shutdown();
                        }
                        break;
                    }
            }
        }
    }
}
