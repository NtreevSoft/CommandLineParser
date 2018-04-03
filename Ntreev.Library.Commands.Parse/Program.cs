//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
            var parser = new CommandLineParser(settings);

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