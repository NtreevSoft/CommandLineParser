===========================================

Example
=======

Parse
--------------

��ɱ����� �м��� �̸� ���ǵ� �Ӽ��� ���� �����ϴ� ����Դϴ�.

��ɱ����� ���Խ�ų �Ӽ��� CommandPropertyAttribute�� �����մϴ�. 

�̸��� [kebab-case (spinal-case, Train-Case, Lisp-case)](https://en.wikipedia.org/wiki/Letter_case) ���·� �����Ǹ� CommandPropertyAttribute �����ڿ� �̸��� ��������� ������ �� �ֽ��ϴ�.

�ش� �Ӽ��� ���� ����� SummaryAttribute�� �ۼ��� �� �ְ� ���� �ڼ��� ������ �ʿ��Ҷ��� DescriptionAttribute�� ���ؼ� �ۼ��� �� �ֽ��ϴ�.

��� ������ Ư�� �Ӽ��� �ݵ�� �ʿ��Ҷ��� IsRequired�� true�� �����ϸ� �˴ϴ�.

IsRequired �� ������ �Ӽ��� ��ɱ������� ����ġ���� ������ �� ������ �� �ܿ� �Ӽ��� --[�̸�] [����] �Ǵ� -[ª���̸�] [����] ���¿��� �մϴ�.

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

��ɱ����� �м��� �̸� ���ǵ� �޼ҵ带 ȣ���ϴ� ����Դϴ�.

��ɱ����� ���Խ�ų �޼ҵ�� CommandMethodAttribute�� �����մϴ�. 

���� �ۼ��� ���� �κ��� Parsing �κп��� �ٷ�Ͱ� �����մϴ�.

�⺻������ �޼ҵ��� ���ڴ� IsRequierd�� �ڵ����� �����Ǹ� �߰������� ���� ���ڸ� ����Ҷ��� CommandMethodPropertyAttribute�� ����� �Ӽ��� �̸��� �����ϸ� �˴ϴ�.

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


�� ���� ���
--------------

- �������� ����� �� �ִ� static property
- �������� ����� �� �ִ� static method
- parse �� invoke ����� ���� ���� ���� ��� ü�踦 ��Ÿ���� CommandContext
- ��Ÿ�ӻ󿡼� ����� �� �ִ� �͹̳�
        

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
