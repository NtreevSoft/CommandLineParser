using System;
using Ntreev.Library.Commands;
using System.ComponentModel;
using System.IO;

namespace Ntreev.Library.Commands.Parse
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();
            var parser = new CommandLineParser(typeof(Settings));

            try
            {
                if (parser.Parse(Environment.CommandLine) == false)
                {
                    Environment.Exit(1);
                }

                Console.WriteLine("use cache : {0}", settings.UseCache);
                Console.WriteLine("cache size : {0}", settings.CacheSize);
                foreach (var item in settings.Libraries)
                {
                    Console.WriteLine("library loaded : {0}", item);
                }

                Console.WriteLine("service port : {0}", settings.Port);
                Console.WriteLine("service workingPath : {0}", new DirectoryInfo(settings.WorkingPath).FullName);
                Console.WriteLine("{0} service is started.", settings.ServiceName);
                Console.WriteLine("press 'Q' to quit");

                while (Console.Read() == (int)ConsoleKey.Q)
                    ;
                Console.WriteLine("{0} service is finshed.", settings.ServiceName);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(2);
            }
        }
    }
}