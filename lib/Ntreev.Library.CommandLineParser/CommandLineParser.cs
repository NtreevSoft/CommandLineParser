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
    /// 커맨드 라인을 분석할 수 있는 방법을 제공합니다.
    /// </summary>
    public partial class CommandLineParser
    {
        private object instance;
        private string command;
        private string arguments;
        private ParseOptions parsingOptions;
        private UsagePrinter usagePrinter;

        /// <summary>
        /// <seealso cref="CommandLineParser"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public CommandLineParser()
        {
            this.TextWriter = Console.Out;
        }

        /// <summary>
        /// 문자열을 분석하여 데이터로 변환합니다.
        /// </summary>
        /// <returns>
        /// 모든 과정이 성공하면 true를, 그렇지 않다면 false를 반환합니다.
        /// </returns>
        /// <param name="commandLine">
        /// 실행파일의 경로와 인자가 포함되어 있는 전체 문자열입니다.
        /// </param>
        /// <param name="instance">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        public bool TryParse(string commandLine, object instance)
        {
            return TryParse(commandLine, instance, ParseOptions.None);
        }

        /// <summary>
        /// 문자열을 분석하여 데이터로 변환합니다.
        /// </summary>
        /// <returns>
        /// 모든 과정이 성공하면 true를, 그렇지 않다면 false를 반환합니다.
        /// </returns>
        /// <param name="commandLine">
        /// 실행파일의 경로와 인자가 포함되어 있는 전체 문자열입니다.
        /// </param>
        /// <param name="instance">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        /// <param name="parsingOptions">
        /// 문자열을 분석하기 위한 옵션입니다.
        /// </param>
        public bool TryParse(string commandLine, object instance, ParseOptions parsingOptions)
        {
            try
            {
                Parse(commandLine, instance, parsingOptions);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }
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
        public void Parse(string commandLine, object instance)
        {
            Parse(commandLine, instance, ParseOptions.None);
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
        public void Parse(string commandLine, object instance, ParseOptions parsingOptions)
        {
            using (Tracer tracer = new Tracer("Parsing"))
            {
                try
                {
                    this.usagePrinter = null;
                    Trace.WriteLine(string.Format("parsing options : {0}", parsingOptions));

                    this.instance = instance;
                    this.parsingOptions = parsingOptions;

                    Regex regex = new Regex(@"^((?<exe>""[^""]*"")|(?<exe>\S+))\s+(?<arg>.*)", RegexOptions.ExplicitCapture);
                    Match match = regex.Match(commandLine);

                    this.command = match.Groups["exe"].Value;
                    this.arguments = match.Groups["arg"].Value;
                    this.arguments = this.arguments.Trim();

                    if (arguments.Length == 0)
                        throw new ArgumentException(Resources.NoArguments, commandLine);

                    string[] switchLines = this.SplitSwitches(this.arguments);

                    SwitchHelper helper = new SwitchHelper(instance);
                    helper.Parse(instance, switchLines, this.CaseSensitive);
                }
                finally
                {
                    this.usagePrinter = this.CreateUsagePrinterCore(instance.GetType());
                }
            }
        }

        public bool TryParse(string commandLine, Type type)
        {
            return TryParse(commandLine, type, ParseOptions.None);
        }

        public bool TryParse(string commandLine, Type type, ParseOptions parsingOptions)
        {
            try
            {
                Parse(commandLine, type, parsingOptions);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }
        }

        public void Parse(string commandLine, Type type)
        {
            this.Parse(commandLine, type, ParseOptions.None);
        }

        public void Parse(string commandLine, Type type, ParseOptions parsingOptions)
        {
            using (Tracer tracer = new Tracer("Parsing"))
            {
                try
                {
                    this.usagePrinter = null;
                    Trace.WriteLine(string.Format("parsing options : {0}", parsingOptions));

                    this.instance = null;
                    this.parsingOptions = parsingOptions;

                    Regex regex = new Regex(@"^((?<exe>""[^""]*"")|(?<exe>\S+))\s+(?<arg>.*)", RegexOptions.ExplicitCapture);
                    Match match = regex.Match(commandLine);

                    this.command = match.Groups["exe"].Value;
                    this.arguments = match.Groups["arg"].Value;
                    this.arguments = this.arguments.Trim();

                    if (arguments.Length == 0)
                        throw new ArgumentException(Resources.NoArguments, commandLine);

                    string[] switchLines = this.SplitSwitches(this.arguments);

                    SwitchHelper helper = new SwitchHelper(type);
                    helper.Parse(instance, switchLines, this.CaseSensitive);
                }
                finally
                {
                    this.usagePrinter = this.CreateUsagePrinterCore(type);
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

        /// <summary>
        /// 분석과정중 생기는 다양한 정보를 출력할 수 있는 처리기를 지정합니다.
        /// </summary>
        public TextWriter TextWriter { get; set; }

        /// <summary>
        /// 사용방법을 출력하는 방법을 나타내는 인스턴를 가져옵니다.
        /// </summary>
        public UsagePrinter Usage
        {
            get { return this.usagePrinter; }
        }

        protected virtual UsagePrinter CreateUsagePrinterCore(Type type)
        {
            return new SwitchUsagePrinter(type, this.command);
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

        private bool CaseSensitive
        {
            get
            {
                return this.parsingOptions.HasFlag(ParseOptions.CaseSensitive);
            }
        }
    }
}