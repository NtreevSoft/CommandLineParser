#region License
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
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

using Trace = System.Diagnostics.Trace;
using Ntreev.Library.Commands.Properties;
using System.Reflection;

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// 커맨드 라인을 분석할 수 있는 방법을 제공합니다.
    /// </summary>
    public partial class CommandLineParser
    {
        private string name;
        private object instance;
        private CommandUsagePrinter switchUsagePrinter;
        private MethodUsagePrinter methodUsagePrinter;

        public CommandLineParser(object instance)
            : this(string.Empty, instance)
        {

        }

        public CommandLineParser(string name, object instance)
        {
            this.HelpName = "help";
            this.instance = instance;
            this.name = name ?? string.Empty;
            if (this.name == string.Empty)
            {
                if (instance is ICommand)
                    this.name = (instance as ICommand).Name;
                else
                    this.name = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            }
            this.switchUsagePrinter = this.CreateUsagePrinterCore(this.name, instance);
            this.methodUsagePrinter = this.CreateMethodUsagePrinterCore(this.name, instance);
            this.Out = Console.Out;
        }

        /// <summary>
        /// 문자열을 분석하여 해당 인스턴스에 적절한 값을 설정합니다. 만약 <see cref="DefaultCommandAttribute"/> 로 설정된 메소드가 있을때는 메소드 기준으로 값을 읽어들입니다.
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public bool Parse(string commandLine)
        {
            var match = Regex.Match(commandLine, @"^((""[^""]*"")|(\S+))");
            var name = match.Value.Trim(new char[] { '\"', });

            if (File.Exists(name) == true)
                name = Path.GetFileNameWithoutExtension(name);

            if (this.name != name)
                throw new ArgumentException(string.Format("'{0}' 은 잘못된 명령입니다.", name));

            var arguments = commandLine.Substring(match.Length).Trim();

            if (arguments == this.HelpName)
            {
                this.PrintHelp();
                return false;
            }
            else
            {
                var helper = new SwitchHelper(this.instance);
                helper.Parse(this.instance, arguments);
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine">
        /// command subcommand arguments
        /// </param>
        /// <param name="instance"></param>
        /// <param name="parsingOptions"></param>
        public bool Invoke(string commandLine)
        {
            var cmdLine = commandLine;

            var regex = new Regex(@"^((""[^""]*"")|(\S+))");
            var match = regex.Match(cmdLine);
            this.name = match.Value.Trim(new char[] { '\"', });

            if (File.Exists(this.name) == true)
                this.name = Path.GetFileNameWithoutExtension(this.name).ToLower();

            cmdLine = cmdLine.Substring(match.Length).Trim();
            match = regex.Match(cmdLine);
            var method = match.Value;

            var arguments = cmdLine.Substring(match.Length).Trim();

            if (string.IsNullOrEmpty(method) == true)
            {
                this.PrintSummary();
                return false;
            }
            else if (method == this.HelpName)
            {
                this.PrintMethodHelp(arguments);
                return false;
            }
            else
            {
                var descriptor = CommandDescriptor.GetMethodDescriptor(this.instance.GetType(), method);

                if (descriptor == null)
                {
                    throw new NotFoundMethodException(method);
                }

                try
                {
                    descriptor.Invoke(this.instance, arguments);
                    return true;
                }
                catch (SwitchException e)
                {
                    throw e;
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException != null)
                        throw new MethodInvokeException(method, e.InnerException);
                    throw e;
                }
                catch (Exception e)
                {
                    throw new MethodInvokeException(method, e);
                }
            }
        }

        /// <summary>
        /// 모든 스위치의 사용법을 출력합니다.
        /// </summary>
        public void PrintUsage()
        {
            this.switchUsagePrinter.Print(this.Out);
        }

        public void PrintMethodUsage()
        {
            this.methodUsagePrinter.Print(this.Out);
        }

        public void PrintMethodUsage(string methodName)
        {
            this.methodUsagePrinter.Print(this.Out, methodName);
        }

        public static string[] Split(string commandLine)
        {
            var match = Regex.Match(commandLine, @"^((""[^""]*"")|(\S+))");
            var name = match.Value.Trim(new char[] { '\"', });

            if (File.Exists(name) == true)
                name = Path.GetFileNameWithoutExtension(name);

            var arguments = commandLine.Substring(match.Length).Trim();

            return new string[] { name, arguments, };
        }

        protected virtual void PrintHelp()
        {
            this.PrintUsage();
        }

        protected virtual void PrintMethodHelp(string methodName)
        {
            if (string.IsNullOrEmpty(methodName) == true)
            {
                this.PrintMethodUsage();
            }
            else
            {
                var descriptor = CommandDescriptor.GetMethodDescriptor(this.instance.GetType(), methodName);
                if (descriptor == null)
                {
                    this.Out.WriteLine("{0} is not subcommand", methodName);
                }
                else
                {
                    this.PrintMethodUsage(methodName);
                }
            }
        }

        protected virtual void PrintSummary()
        {
            this.Out.WriteLine("Type '{0} {1}' for usage.", this.name, this.HelpName);
        }

        /// <summary>
        /// 분석과정중 생기는 다양한 정보를 출력할 수 있는 처리기를 지정합니다.
        /// </summary>
        public TextWriter Out { get; set; }

        public string Name
        {
            get { return this.name; }
        }

        public object Instance
        {
            get { return this.instance; }
        }

        public string HelpName
        {
            get;set;
        }

        protected virtual CommandUsagePrinter CreateUsagePrinterCore(string name, object instance)
        {
            return new CommandUsagePrinter(name, instance);
        }

        protected virtual MethodUsagePrinter CreateMethodUsagePrinterCore(string name, object instance)
        {
            return new MethodUsagePrinter(name, instance);
        }
    }
}