using System;
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

    }
}