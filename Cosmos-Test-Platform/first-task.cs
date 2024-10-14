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


        Memory mem= new Memory();


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

            //System.IO.File.WriteAllText(@"C:\Desktop\test.txt", input);

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
                case "write":
                {
                        mem.Schreiben(uint.Parse(args[1]), byte.Parse(args[2]));
                        break;
                }

                case "read":
                    {
                        int a= mem.Lesen(uint.Parse(args[1]));
                        Console.WriteLine(a);
                        
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


    class Memory
    {
        Cosmos.Core.ManagedMemoryBlock newBlock = new Cosmos.Core.ManagedMemoryBlock(16);
        public void Schreiben(uint index, byte value)
        {
            newBlock.Write8(index, value);
        }
        public ushort Lesen(uint index)
        {
            ushort val = 0;
            val = newBlock.Read16(index);
            return (byte)(val & 0xFF);
        }


    }
}
