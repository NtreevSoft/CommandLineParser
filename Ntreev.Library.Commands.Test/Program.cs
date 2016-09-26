﻿#region License
//Ntreev CommandLineParser for .Net 1.0.4548.25168
//https://github.com/NtreevSoft/CommandLineParser

//Released under the MIT License.

//Copyright (c) 2010 Ntreev Soft co., Ltd.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Library;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Ntreev.Library.Commands.Test.Properties;
using System.Resources;

namespace Ntreev.Library.Commands.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var res = typeof(Program).Assembly.GetManifestResourceNames();

            var stream = typeof(Program).Assembly.GetManifestResourceStream(typeof(Commands.AddCommand).FullName + ".resources");

            ResourceSet resSet = new ResourceSet(stream);
            int qwe11r = 0;

            var reader = new StreamReader(stream);
            reader.ReadToEnd();
            for (var i = 255; i < 600; i++)
            {
                var c = Convert.ToChar(i);
                Console.CursorLeft = 0;
                Console.Write(c);
                if (Console.CursorLeft == 1)
                {
                    var category = char.GetUnicodeCategory(c);
                    if (category == System.Globalization.UnicodeCategory.LowercaseLetter)
                        continue;
                    int qwer = 0;
                }
            }
            var context = Container.GetService<CommandContext>();

            try
            {
                context.Execute(Environment.CommandLine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }
    }
}