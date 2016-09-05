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
using Ntreev.Library.Properties;
using System.Reflection;

namespace Ntreev.Library
{
    /// <summary>
    /// 커맨드 라인을 분석할 수 있는 방법을 제공합니다.
    /// </summary>
    public partial class CommandLineParser
    {
        private string name;
        private object instance;
        //private string arguments;
        //private ParseOptions parsingOptions;
        private SwitchUsagePrinter switchUsagePrinter;
        private MethodUsagePrinter methodUsagePrinter;

        public CommandLineParser(object instance)
            : this(System.Diagnostics.Process.GetCurrentProcess().ProcessName, instance)
        {

        }

        public CommandLineParser(string name, object instance)
        {
            this.instance = instance;
            this.name = name;
            this.switchUsagePrinter = this.CreateUsagePrinterCore(name, instance);
            this.methodUsagePrinter = this.CreateMethodUsagePrinterCore(name, instance);
            this.TextWriter = Console.Out;
        }

        public bool Parse(string commandLine)
        {
            var match = Regex.Match(commandLine, @"^((""[^""]*"")|(\S+))");
            var name = match.Value.Trim(new char[] { '\"', });

            if (File.Exists(name) == true)
                name = Path.GetFileNameWithoutExtension(name);

            if (this.name != name)
                throw new ArgumentException(string.Format("'{0}' 은 잘못된 명령입니다.", name));

            var arguments = commandLine.Substring(match.Length).Trim();

            if (arguments == "help")
            {
                this.PrintHelp(this.instance, this.name);
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
                this.PrintSummary(this.instance);
                return false;
            }
            else if (method == "help")
            {
                this.PrintMethodHelp(this.instance, this.name, arguments);
                return false;
            }
            else
            {
                var descriptor = CommandDescriptor.GetMethodDescriptor(this.instance, method);

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
            this.switchUsagePrinter.PrintUsage(this.TextWriter);
        }

        public void PrintMethodUsage()
        {
            this.methodUsagePrinter.PrintUsage(this.TextWriter);
        }

        public void PrintMethodUsage(string methodName)
        {
            this.methodUsagePrinter.PrintUsage(this.TextWriter, methodName);
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

        protected virtual void PrintHelp(object target, string commandName)
        {
            this.PrintUsage();
        }

        protected virtual void PrintMethodHelp(object target, string commandName, string methodName)
        {
            if (string.IsNullOrEmpty(methodName) == true)
            {
                this.PrintMethodUsage();
            }
            else
            {
                MethodDescriptor descriptor = CommandDescriptor.GetMethodDescriptor(target, methodName);
                if (descriptor == null)
                {
                    this.TextWriter.WriteLine("{0} is not subcommand", methodName);
                }
                else
                {
                    this.PrintMethodUsage(methodName);
                }
            }
        }

        protected virtual void PrintSummary(object target)
        {
            this.TextWriter.WriteLine("Type '{0} help' for usage.", this.name);
        }

        /// <summary>
        /// 분석과정중 생기는 다양한 정보를 출력할 수 있는 처리기를 지정합니다.
        /// </summary>
        public TextWriter TextWriter { get; set; }

        public string Name
        {
            get { return this.name; }
        }

        public object Instance
        {
            get { return this.instance; }
        }

        protected virtual SwitchUsagePrinter CreateUsagePrinterCore(string name, object target)
        {
            return new SwitchUsagePrinter(target, name);
        }

        protected virtual MethodUsagePrinter CreateMethodUsagePrinterCore(string name, object target)
        {
            return new MethodUsagePrinter(target, name);
        }
    }
}