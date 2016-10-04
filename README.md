===========================================

Example
=======

Single Command
--------------

    using System;
    using Ntreev.Library.Commands;

    namespace Program
    {
        class Settings
        {
            [CommandSwitch(Name = "param1", Required = true)]
            [Description("parameter1 description")]
            public string Parameter1
            {
                get; set;
            }

            [CommandSwitch(Name = "param2", Required = true)]
            [Description("parameter2 description")]
            public int Parameter2
            {
                get; set;
            }

            [CommandSwitch(ShortName = 'o', NameType = SwitchNameTypes.ShortName)]
            [Description("option1 description")]
            public bool Option1
            {
                get; set;
            }

            [CommandSwitch("text-option"]
            [Description("option2 description")]
            public string Option2
            {
                get; set;
            }
        }

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


You can call like this in console:

    Example.exe help

    Example.exe --version

    Example.exe text1 123 

    Example.exe text1 123 -o --text-option "text string"



License
=======

Ntreev CommandLineParser for .Net 
https://github.com/NtreevSoft/CommandLineParser

Released under the MIT License.

Copyright (c) 2010 Ntreev Soft co., Ltd.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
