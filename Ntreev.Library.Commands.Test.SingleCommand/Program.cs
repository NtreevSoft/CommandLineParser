using System;
using Ntreev.Library.Commands;
using System.ComponentModel;

namespace Ntreev.Library.Commands.Test.SingleCommand
{
    

    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();
            var parser = new CommandLineParser(settings);

            try
            {
                if (parser.Parse(Environment.CommandLine) == false)
                {
                    Environment.Exit(1);
                }

                // todo

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(2);
            }
        }
    }
}