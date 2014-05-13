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
    /// 커맨드 라인을 분석해 메소드를 호출할 수 있는 방법을 제공합니다.
    /// </summary>
    public class CommandLineInvoker
    {
        internal const string defaultMethod = "default";
        private const string helpMethod = "help";

        //private object instance;
        private string command;
        private string arguments;
        private string method;
        private MethodUsagePrinter usagePrinter;

        /// <summary>
        /// <seealso cref="CommandLineParser"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public CommandLineInvoker()
        {
            this.TextWriter = Console.Out;
        }

        public CommandLineInvoker(string executeName)
        {
            this.TextWriter = Console.Out;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine">
        /// command subcommand arguments
        /// </param>
        /// <param name="instance"></param>
        /// <param name="parsingOptions"></param>
        public bool Invoke(object instance, string commandLine)
        {
            return this.InvokeCore(instance, commandLine);
        }

        public bool Invoke(Type type, string commandLine)
        {
            return this.InvokeCore(type, commandLine);
        }

        /// <summary>
        /// 모든 스위치의 사용법을 출력합니다.
        /// </summary>
        public void PrintUsage()
        {
            this.usagePrinter.PrintUsage(this.TextWriter);
        }

        public void PrintUsage(string methodName)
        {
            this.usagePrinter.PrintUsage(this.TextWriter, methodName);
        }

        /// <summary>
        /// 분석과정중 생기는 다양한 정보를 출력할 수 있는 처리기를 지정합니다.
        /// </summary>
        public TextWriter TextWriter { get; set; }

        /// <summary>
        /// 사용방법을 출력하는 방법을 나타내는 인스턴를 가져옵니다.
        /// </summary>
        public MethodUsagePrinter Usage
        {
            get { return this.usagePrinter; }
        }

        public string Command
        {
            get { return this.command; }
        }

        public string Method
        {
            get { return this.method; }
        }

        public string Arguments
        {
            get { return this.arguments; }
        }

        protected virtual void PrintHelp(object target, string commandName, string methodName)
        {
            if (string.IsNullOrEmpty(methodName) == true)
            {
                this.PrintUsage();
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
                    this.PrintUsage(methodName);
                }
            }
        }

        protected virtual void PrintSummary(object target)
        {
            this.TextWriter.WriteLine("Type '{0} help' for usage.", this.command);
        }

        protected virtual MethodUsagePrinter CreateUsagePrinterCore(object target)
        {
            return new MethodUsagePrinter(target, this.command);
        }

        private bool InvokeCore(object target, string commandLine)
        {
            using (Tracer tracer = new Tracer("Inovking"))
            {
                string cmdLine = commandLine;

                Regex regex = new Regex(@"^((""[^""]*"")|(\S+))");
                Match match = regex.Match(cmdLine);
                this.command = match.Value.Trim(new char[] { '\"', });

                if (File.Exists(this.command) == true)
                    this.command = Path.GetFileNameWithoutExtension(this.command).ToLower();

                cmdLine = cmdLine.Substring(match.Length).Trim();
                match = regex.Match(cmdLine);
                this.method = match.Value;

                this.arguments = cmdLine.Substring(match.Length).Trim();
                this.arguments = this.arguments.Trim();

                this.usagePrinter = this.CreateUsagePrinterCore(target);

                if (string.IsNullOrEmpty(this.method) == true)
                {
                    this.PrintSummary(target);
                    return false;
                }
                else if (this.method == CommandLineInvoker.helpMethod)
                {
                    this.PrintHelp(target, this.command, this.arguments);
                    return false;
                }
                else
                {
                    MethodDescriptor descriptor = CommandDescriptor.GetMethodDescriptor(target, this.method);

                    if (descriptor == null)
                    {
                        throw new NotFoundMethodException(this.method);
                    }

                    try
                    {
                        descriptor.Invoke(target, this.arguments);
                        return true;
                    }
                    catch (SwitchException e)
                    {
                        throw e;
                    }
                    catch (TargetInvocationException e)
                    {
                        if(e.InnerException != null)
                            throw new MethodInvokeException(this.method, e.InnerException);
                        throw e;
                    }
                    catch (Exception e)
                    {
                        throw new MethodInvokeException(this.method, e);
                    }
                }
            }
        }
    }


    public class InvokeEventArgs : EventArgs
    {
        private readonly string commandName;
        private readonly string methodName;
        private readonly object target;

        public InvokeEventArgs(object target, string commandName, string methodName)
        {
            this.target = target;
            this.commandName = commandName;
            this.methodName = methodName;
        }

        public object Target
        {
            get { return this.target; }
        }

        public string CommandName
        {
            get { return this.commandName; }
        }

        public string MethodName
        {
            get { return this.methodName; }
        }
    }
}