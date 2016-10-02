﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.MultiCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Commands();
            var parser = new CommandLineParser(settings);

            try
            {
                if (parser.Invoke(Environment.CommandLine) == false)
                {
                    Environment.Exit(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(2);
            }
        }
    }
}
