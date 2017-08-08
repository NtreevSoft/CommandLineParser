===========================================

Example
=======

Parse
--------------

명령구문을 분석해 미리 정의된 속성에 값을 설정하는 기능입니다.

명령구문에 포함시킬 속성은 CommandPropertyAttribute로 설정합니다. 

이름은 [kebab-case (spinal-case, Train-Case, Lisp-case)](https://en.wikipedia.org/wiki/Letter_case) 형태로 설정되며 CommandPropertyAttribute 생성자에 이름을 명시적으로 설정할 수 있습니다.

해당 속성에 대한 요약은 SummaryAttribute를 작성할 수 있고 좀더 자세한 설명이 필요할때는 DescriptionAttribute를 통해서 작성할 수 있습니다.

명령 구문에 특정 속성이 반드시 필요할때는 IsRequired를 true로 설정하면 됩니다.

IsRequired 가 설정된 속성은 명령구문에서 스위치값을 생략할 수 있으며 그 외에 속성은 --[이름] [변수] 또는 -[짧은이름] [변수] 형태여야 합니다.

    using System;
    using Ntreev.Library.Commands;
    using System.ComponentModel;

    namespace ConsoleApp
    {
        class Settings
        {
            [CommandProperty("param1", IsRequired = true)]
            [Description("parameter1 description")]
            public string Parameter1
            {
                get; set;
            }

            [CommandProperty("param2", IsRequired = true)]
            [Description("parameter2 description")]
            public int Parameter2
            {
                get; set;
            }

            [CommandProperty('o')]
            [Description("option1 description")]
            public bool Option1
            {
                get; set;
            }

            [CommandProperty("text-option")]
            [Description("option2 description")]
            public string Option2
            {
                get; set;
            }
        }

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
                    Console.Error.WriteLine(e);
                    Environment.Exit(2);
                }
            }
        }
    }


You can call like this in console:

    Example.exe help

    Example.exe --version

    Example.exe text1 123 

    Example.exe text1 123 -o --text-option "text string"

Invoke
--------------

명령구문을 분석해 미리 정의된 메소드를 호출하는 기능입니다.

명령구문에 포함시킬 메소드는 CommandMethodAttribute로 설정합니다. 

설명 작성에 대한 부분은 Parsing 부분에서 다룬것과 동일합니다.

기본적으로 메소드의 인자는 IsRequierd가 자동으로 설정되며 추가적으로 선택 인자를 사용할때는 CommandMethodPropertyAttribute를 사용해 속성의 이름을 설정하면 됩니다.

    using System;
    using Ntreev.Library.Commands;
    using System.ComponentModel;

    namespace ConsoleApp
    {
        class Commands
        {
            [CommandMethod("method1")]
            public void Method1(string arg)
            {
            
            }

            [CommandMethod]
            public void Method2(string arg)
            {
            
            }

            [CommandMethod]
            [CommandMethodProperty(nameof(Message))]
            public void Method3(string arg0, string arg1 = null)
            {

            }

            [CommandProperty('m')]
            [DefaultValue("")]
            public string Message
            {
                get; set;
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                var commands = new Commands();
                var parser = new CommandLineParser(commands);

                try
                {
                    if (parser.Invoke(Environment.CommandLine) == true)
                    {
                        Environment.Exit(1);
                    }

                    // todo

                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(2);
                }
            }
        }
    }


You can call like this in console:

    Example.exe help

    Example.exe --version

    Example.exe method1 123 

    Example.exe method2 123

    Example.exe method3 1

    or

    Example.exe method3 1 2

    or

    Example.exe method3 1 2 -m "message"


그 밖의 기능
--------------

- 공통으로 사용할 수 있는 static property
- 공통으로 사용할 수 있는 static method
- parse 과 invoke 기능을 합쳐 종합 적인 명령 체계를 나타내는 CommandContext
- 런타임상에서 사용할 수 있는 터미널
        

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
