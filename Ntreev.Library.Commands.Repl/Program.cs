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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Library;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Ntreev.Library.Commands.Shell.Properties;
using System.Resources;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Ntreev.Library.Commands.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorSize = 100;
            for (var i = 0; i < Console.BufferHeight - 2; i++)
            {
                Console.WriteLine(i);
            }
            var shell = Container.GetService<IShell>();
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1000);

                //Console.Write(string.Empty.PadRight(Console.BufferWidth + 2, 'c'));

                int i = 0;
                while (true)
                {
                    
                    //if (Console.BufferWidth == 81)
                    {
                        //Console.Write($"\r{i++}");
                        //Console.WriteLine("44");
                        //break;
                    }
                    System.Threading.Thread.Sleep(1000);
                    //break;
                }
                //    //Console.WriteLine(DateTime.Now);
                //    System.Threading.Thread.Sleep(1000);
                //}
                //System.Threading.Thread.Sleep(10000);
                //Console.Write(DateTime.Now);
            });

            shell.Prompt = Directory.GetCurrentDirectory();
            shell.Start();
        }
    }
}