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
        private SwitchUsagePrinter usagePrinter;

        /// <summary>
        /// <seealso cref="CommandLineParser"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        [Obsolete]
        public CommandLineParser()
        {
            this.TextWriter = Console.Out;
        }

        public CommandLineParser(object instance)
            : this(Path.GetFileName(Assembly.GetEntryAssembly().Location), instance)
        {

        }

        public CommandLineParser(string name, object instance)
        {
            this.instance = instance;
            this.name = name;
            this.usagePrinter = this.CreateUsagePrinterCore(name, instance);
            this.TextWriter = Console.Out;
        }

        public void Parse(string commandLine)
        {
            this.ParseCore(commandLine);
        }

        /// <summary>
        /// 문자열을 분석하여 데이터로 변환합니다.
        /// </summary>
        /// <param name="commandLine">
        /// 실행파일의 경로와 인자가 포함되어 있는 전체 문자열입니다.
        /// </param>
        /// <param name="instance">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        /// <exception cref="SwitchException">
        /// 스위치의 인자가 문자열에 의한 변환을 지원하지 않거나 변환할 수 없는 경우
        /// </exception>
        /// <exception cref="ArgumentException">
        /// commandLine에 전달 인자가 하나도 포함되어 있지 않은 경우
        /// </exception>
        public void Parse(object instance, string commandLine)
        {
            this.instance = instance;
            this.ParseCore(commandLine);
        }

        /// <summary>
        /// 문자열을 분석하여 데이터로 변환합니다.
        /// </summary>
        /// <param name="commandLine">
        /// 실행파일의 경로와 인자가 포함되어 있는 전체 문자열입니다.
        /// </param>
        /// <param name="instance">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        /// <param name="parsingOptions">
        /// 문자열을 분석하기 위한 옵션입니다.
        /// </param>
        /// <exception cref="SwitchException">
        /// 스위치의 인자가 문자열에 의한 변환을 지원하지 않거나 변환할 수 없는 경우
        /// </exception>
        /// <exception cref="ArgumentException">
        /// commandLine에 전달 인자가 하나도 포함되어 있지 않은 경우
        /// </exception>
        [Obsolete]
        public void Parse(object instance, string commandLine, ParseOptions parsingOptions)
        {
            this.ParseCore(commandLine);
        }

        private void ParseCore(string commandLine)
        {
            //using (Tracer tracer = new Tracer("Parsing"))
            {
                string cmdLine = commandLine;

                Match match = Regex.Match(cmdLine, @"^((""[^""]*"")|(\S+))");
                this.name = match.Value.Trim(new char[] { '\"', });

                if (File.Exists(this.name) == true)
                    this.name = Path.GetFileNameWithoutExtension(this.name).ToLower();

                string arguments = cmdLine.Substring(match.Length).Trim();

                if (arguments == "help")
                {
                    this.PrintHelp(this.instance, this.name);
                }
                else
                {
                    SwitchHelper helper = new SwitchHelper(this.instance);
                    helper.Parse(this.instance, arguments);
                }
            }
        }

        protected virtual void PrintHelp(object target, string commandName)
        {
            this.PrintUsage();
        }

        protected virtual void PrintSummary(object target)
        {
            this.TextWriter.WriteLine("Type '{0} help' for usage.", this.name);
        }

        public void Parse(Type type, string commandLine)
        {
            this.Parse(type, commandLine, ParseOptions.None);
        }

        public void Parse(Type type, string commandLine, ParseOptions parsingOptions)
        {
            this.ParseCore(commandLine);
        }

        /// <summary>
        /// 모든 스위치의 사용법을 출력합니다.
        /// </summary>
        public void PrintUsage()
        {
            this.usagePrinter.PrintUsage(this.TextWriter);
        }

        /// <summary>
        /// 분석과정중 생기는 다양한 정보를 출력할 수 있는 처리기를 지정합니다.
        /// </summary>
        public TextWriter TextWriter { get; set; }

        /// <summary>
        /// 사용방법을 출력하는 방법을 나타내는 인스턴를 가져옵니다.
        /// </summary>
        public SwitchUsagePrinter Usage
        {
            get { return this.usagePrinter; }
        }

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
    }
}