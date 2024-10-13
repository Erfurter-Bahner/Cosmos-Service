using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;
using System.Text;
using Sys = Cosmos.System;

namespace Cosmos_Test_Platform
{
    public class Kernel : Sys.Kernel
    {
        DateTime start;

        protected override void BeforeRun()
        {
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
            start = DateTime.Now;
        }

        protected override void Run()
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            Console.Write("Text typed : ");
            string[] args = input.Split(' ');
            Console.WriteLine(input);
            
            switch (args[0])
            {
                case "help":
                {
                        Console.WriteLine("phillip stinkt");
                        break; 
                }
                case "time":
                {
                        getSysTime();
                        break;
                }
                default: Console.WriteLine("eingabe falsch");
                    break; 
            }
            
            
        }
        private void getSysTime() 
        {
            TimeSpan runtime = DateTime.Now - start;
            Console.WriteLine("running for: " + runtime.TotalSeconds + " seconds");

        }


    }
}
