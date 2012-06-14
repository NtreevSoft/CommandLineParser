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

namespace Ntreev.Library.CommandLineParser
{
    /// <summary>
    /// 커맨드 라인을 분석할 수 있는 방법을 제공합니다.
    /// </summary>
    public partial class CommandLineParser
    {
        #region private variables

        string command;
        string arguments;

        List<string> unusedArguments = new List<string>();
        readonly UsagePrinter usagePrinter = null;

        object instance;
        CommandSwitchAttributeCollection switchAttributes;
        ParsingOptions parsingOptions;

        #endregion

        #region public methods

        /// <summary>
        /// <seealso cref="CommandLineParser"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public CommandLineParser()
        {
            this.TextWriter = Console.Out;

            this.usagePrinter = CreateUsagePrinterCore(this);
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
        /// <param name="options">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        public bool TryParse(string commandLine, object options)
        {
            return TryParse(commandLine, options, ParsingOptions.None);
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
        /// <param name="options">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        /// <param name="parsingOptions">
        /// 문자열을 분석하기 위한 옵션입니다.
        /// </param>
        public bool TryParse(string commandLine, object options, ParsingOptions parsingOptions)
        {
            try
            {
                Parse(commandLine, options, parsingOptions);
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
        /// <param name="options">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        /// <exception cref="CommandSwitchException">
        /// 스위치의 인자가 문자열에 의한 변환을 지원하지 않거나 변환할 수 없는 경우
        /// </exception>
        /// <exception cref="ArgumentException">
        /// commandLine에 전달 인자가 하나도 포함되어 있지 않은 경우
        /// </exception>
        public void Parse(string commandLine, object options)
        {
            Parse(commandLine, options, ParsingOptions.None);
        }

        /// <summary>
        /// 문자열을 분석하여 데이터로 변환합니다.
        /// </summary>
        /// <param name="commandLine">
        /// 실행파일의 경로와 인자가 포함되어 있는 전체 문자열입니다.
        /// </param>
        /// <param name="options">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        /// <param name="parsingOptions">
        /// 문자열을 분석하기 위한 옵션입니다.
        /// </param>
        /// <exception cref="CommandSwitchException">
        /// 스위치의 인자가 문자열에 의한 변환을 지원하지 않거나 변환할 수 없는 경우
        /// </exception>
        /// <exception cref="ArgumentException">
        /// commandLine에 전달 인자가 하나도 포함되어 있지 않은 경우
        /// </exception>
        public void Parse(string commandLine, object options, ParsingOptions parsingOptions)
        {
            using (Tracer tracer = new Tracer("Parsing"))
            {
                Trace.WriteLine(string.Format("parsing options : {0}", parsingOptions));
                object instance = options;
                CommandSwitchAttributeCollection switches = null;

                if (options is IOptions)
                {
                    instance = (options as IOptions).Instance;
                    switches = (options as IOptions).SwitchAttributes;
                }

                this.instance = instance;
                this.switchAttributes = switches;
                this.parsingOptions = parsingOptions;

                this.unusedArguments.Clear();

                if (parsingOptions.HasFlag(ParsingOptions.NoExecutionPath) == false)
                {
                    Regex regex = new Regex(@"^((?<exe>""[^""]*"")|(?<exe>\S+))\s+(?<arg>.*)", RegexOptions.ExplicitCapture);
                    Match match = regex.Match(commandLine);

                    this.command = match.Groups["exe"].Value;
                    this.arguments = match.Groups["arg"].Value;
                    this.arguments = this.arguments.Trim();
                }
                else
                {
                    this.command = string.Empty;
                    this.arguments = commandLine.Trim();
                }

                if (arguments.Length == 0)
                    throw new ArgumentException(Properties.Resources.NoArguments, commandLine);

                string[] switchLines, unusedArgs;
                SplitSwitches(this.arguments, out switchLines);

                CommandSwitchDescriptorCollection switchCollection = CommandSwitchDescriptorContext.GetSwitches(instance, switches);
                switchCollection.AssertValidation();
                switchCollection.AssertMutuallyExclusive();

                switchCollection.Parse(switchLines, instance, parsingOptions, out unusedArgs);
                this.unusedArguments.AddRange(unusedArgs);


                foreach (string item in this.unusedArguments)
                {
                    OnParsingUnusedArgument(item);
                }

                switchCollection.AssertRequired();

                AssertValidation(options);
            }
        }

        /// <summary>
        /// 스위치의 사용법을 출력합니다.
        /// </summary>
        /// <param name="switchName">
        /// 사용법을 출력할 스위치의 문자열입니다.
        /// </param>
        public void PrintSwitchUsage(string switchName)
        {
            this.usagePrinter.PrintSwitchUsage(switchName);
        }

        /// <summary>
        /// 모든 스위치의 사용법을 출력합니다.
        /// </summary>
        public void PrintUsage()
        {
            this.usagePrinter.PrintUsage();
        }

        #endregion

        #region protected methods

        /// <summary>
        /// 사용되지 않은 인자들을 처리 합니다.
        /// </summary>
        /// <param name="arg">
        /// 사용되지 않은 인자의 문자열입니다.
        /// </param>
        /// <remarks>
        /// 일반적으로 명령의 형태는 /cmd arg 처럼 스위치와 인자 하나씩 존재합니다.
        /// 만약 /cmd arg1 arg2 와 같은 형태로 되 있다면 arg2는 분석에서 제외됩니다.
        /// 여러개의 인자로 받을 경우에는 /cmd "arg1 arg2" 처럼 따옴표로 인자들을 묶어줄 필요가 있습니다.
        /// </remarks>
        protected virtual void OnParsingUnusedArgument(string arg)
        {

        }

        protected virtual UsagePrinter CreateUsagePrinterCore(CommandLineParser parser)
        {
            return new UsagePrinter(parser);
        }

        /// <summary>
        /// 최종적으로 옵션에 유효성을 검사합니다.
        /// </summary>
        /// <param name="options">
        /// 분석에 사용된 인스턴스입니다.
        /// </param>
        /// <returns>
        /// 분석후 option에 대한 설정 과정이 정상적으로 끝났으면 true를, 그렇지 않다면 false를 반환합니다.
        /// </returns>
        protected virtual bool AssertValidation(object options)
        {
            return true;
        }

        #endregion

        #region private methods

        void SplitSwitches(string arg, out string[] switchLines)
        {
            using (Tracer tracer = new Tracer("Split Switches"))
            {
                List<string> usedList = new List<string>();
                List<string> unusedList = new List<string>();
                string pattern = string.Format(@"{0}\S+((\s+""[^""]*"")|(\s+[\S-[{0}]][\S]*)|(\s*))*", CommandSwitchAttribute.SwitchDelimiter);
                Regex regex = new Regex(pattern);
                Match match = regex.Match(arg);

                while (match.Success == true)
                {
                    string matchedString = match.ToString().Trim();
                    Trace.WriteLine(matchedString);
                    usedList.Add(matchedString);
                    match = match.NextMatch();
                }

                switchLines = usedList.ToArray();
            }
        }

        #endregion

        #region internal properties

        internal object Instance
        {
            get { return this.instance; }
        }

        internal CommandSwitchAttributeCollection SwitchAttributes
        {
            get { return this.switchAttributes; }
        }

        internal ParsingOptions ParsingOptions
        {
            get { return this.parsingOptions; }
        }

        #endregion

        #region public properties

        /// <summary>
        /// 분석과정중 생기는 다양한 정보를 출력할 수 있는 처리기를 지정합니다.
        /// </summary>
        public TextWriter TextWriter { get; set; }

        /// <summary>
        /// 분석중 사용되지 않은 인자들을 가져옵니다.
        /// </summary>
        public string[] UnusedArguments
        {
            get { return this.unusedArguments.ToArray(); }
        }

        /// <summary>
        /// 사용방법을 출력하는 방법을 나타내는 인스턴를 가져옵니다.
        /// </summary>
        public UsagePrinter Usage
        {
            get { return this.usagePrinter; }
        }

        #endregion

        #region private classes

        internal class Tracer : IDisposable
        {
            string message = null;

            public Tracer()
            {
                System.Diagnostics.Trace.Indent();
            }

            public Tracer(string message)
            {
                this.message = message;
                System.Diagnostics.Trace.WriteLine("begin: " + message);
                System.Diagnostics.Trace.Indent();
            }

            #region IDisposable 멤버

            public void Dispose()
            {
                System.Diagnostics.Trace.Unindent();
                if (message != null)
                    System.Diagnostics.Trace.WriteLine("end  : " + message);
            }

            #endregion
        }

        #endregion
    }
}