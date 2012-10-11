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

namespace Ntreev.Library
{
    /// <summary>
    /// 커맨드 라인을 분석해 메소드를 호출할 수 있는 방법을 제공합니다.
    /// </summary>
    public partial class CommandLineInvoker
    {
        public const string DefaultHelpMethod = "help";

        private object instance;
        private string command;
        private string arguments;
        private string method;
        private string helpMethod;
        //private InvokeOptions invokeOptions;
        private IUsagePrinter usagePrinter;

        /// <summary>
        /// <seealso cref="CommandLineParser"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public CommandLineInvoker()
        {
            this.TextWriter = Console.Out;
        }

        public bool TryInvoke(string commandLine, object instance)
        {
            return this.TryInvoke(commandLine, instance, InvokeOptions.None);
        }

        public bool TryInvoke(string commandLine, object instance, InvokeOptions invokeOptions)
        {
            try
            {
                this.Invoke(commandLine, instance, invokeOptions);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Invoke(string commandLine, object instance)
        {
            this.Invoke(commandLine, instance, InvokeOptions.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine">
        /// command subcommand arguments
        /// </param>
        /// <param name="instance"></param>
        /// <param name="parsingOptions"></param>
        public void Invoke(string commandLine, object instance, InvokeOptions invokeOptions)
        {
            using (Tracer tracer = new Tracer("Inovking"))
            {

                this.usagePrinter = null;
                Trace.WriteLine(string.Format("parsing options : {0}", invokeOptions));

                this.instance = instance;

                Match match = Regex.Match(commandLine, @"^((?<cmd>""[^""]*"")|(?<cmd>\S+))\s+((?<sub>""[^""]*"")|(?<sub>\S+))\s*(?<arg>.*)");

                this.command = match.Groups["cmd"].Value.Trim('\"');
                this.method = match.Groups["sub"].Value.Trim('\"');
                this.arguments = match.Groups["arg"].Value;
                this.arguments = this.arguments.Trim();

                if (this.command == string.Empty)
                    this.command = commandLine.Trim();

                this.usagePrinter = this.CreateUsagePrinterCore(instance.GetType());
                if (this.method == CommandLineInvoker.DefaultHelpMethod)
                {
                    this.PrintUsage(this.arguments);
                }
                else
                {
                    string[] parameters = this.SplitSwitches(this.arguments);

                    MethodDescriptor descriptor = CommandDescriptor.GetMethodDescriptor(this.instance, this.method);
                    if (descriptor == null)
                    {
                        throw new NotFoundMethodException(this.method);
                    }

                    try
                    {
                        descriptor.Invoke(this.instance, parameters);
                    }
                    catch (Exception e)
                    {
                        throw new MethodInvokeException(this.method, e);
                    }
                }
            }
        }

        public void Invoke(string commandLine, Type type, InvokeOptions invokeOptions)
        {
            using (Tracer tracer = new Tracer("Inovking"))
            {

                this.usagePrinter = null;
                Trace.WriteLine(string.Format("parsing options : {0}", invokeOptions));

                this.instance = null;

                Match match = Regex.Match(commandLine, @"^((?<cmd>""[^""]*"")|(?<cmd>\S+))\s+((?<sub>""[^""]*"")|(?<sub>\S+))\s*(?<arg>.*)");

                this.command = match.Groups["cmd"].Value.Trim('\"');
                this.method = match.Groups["sub"].Value.Trim('\"');
                this.arguments = match.Groups["arg"].Value;
                this.arguments = this.arguments.Trim();

                if (this.command == string.Empty)
                    this.command = commandLine.Trim();

                this.usagePrinter = this.CreateUsagePrinterCore(type);

                if (this.method == CommandLineInvoker.DefaultHelpMethod)
                {
                    this.PrintUsage(this.arguments);
                }
                else
                {
                    string[] parameters = this.SplitSwitches(this.arguments);

                    MethodDescriptor descriptor = CommandDescriptor.GetMethodDescriptor(type, this.method);
                    if (descriptor == null)
                    {
                        throw new NotFoundMethodException(this.method);
                    }

                    try
                    {
                        descriptor.Invoke(type, parameters);
                    }
                    catch (Exception e)
                    {
                        throw new MethodInvokeException(this.method, e);
                    }
                }

            }
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
        public IUsagePrinter Usage
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

        public string HelpMethod
        {
            get
            {
                if (string.IsNullOrEmpty(helpMethod) == true)
                    return CommandLineInvoker.DefaultHelpMethod;
                return this.helpMethod;
            }
            set
            {
                this.helpMethod = value;
            }
        }

        protected virtual IUsagePrinter CreateUsagePrinterCore(Type type)
        {
            return new MethodUsagePrinter(type, this.command);
        }

        private string[] SplitArguments(string arg)
        {
            using (Tracer tracer = new Tracer("Split Arguments"))
            {
                List<string> switches = new List<string>();
                string pattern = string.Format(@"((""[^""]*"")|(\S+))\s*");
                Regex regex = new Regex(pattern);
                Match match = regex.Match(arg);

                while (match.Success == true)
                {
                    string matchedString = match.ToString().Trim('\"', ' ');
                    Trace.WriteLine(matchedString);
                    switches.Add(matchedString);
                    match = match.NextMatch();
                }

                return switches.ToArray();
            }
        }

        private string[] SplitSwitches(string arg)
        {
            using (Tracer tracer = new Tracer("Split Switches"))
            {
                List<string> switches = new List<string>();
                string pattern = string.Format(@"{0}\S+((\s+""[^""]*"")|(\s+[\S-[{0}]][\S]*)|(\s*))*", CommandSwitchAttribute.SwitchDelimiter);
                Regex regex = new Regex(pattern);
                Match match = regex.Match(arg);

                while (match.Success == true)
                {
                    string matchedString = match.ToString().Trim();
                    Trace.WriteLine(matchedString);
                    switches.Add(matchedString);
                    match = match.NextMatch();
                }

                return switches.ToArray();
            }
        }
    }
}