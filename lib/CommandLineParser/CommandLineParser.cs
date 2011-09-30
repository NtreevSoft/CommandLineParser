#region License
//Ntreev CommandLineParser for .Net 
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

namespace Ntreev.Library
{
    /// <summary>
    /// 커맨드 라인을 파싱할 수 있는 방법을 제공합니다.
    /// </summary>
    public partial class CommandLineParser
    {
        #region private variables

        string command;
        string arguments;

        List<string> unusedArguments = new List<string>();
        readonly UsagePrinter usagePrinter = null;

        object instance;
        SwitchAttributeCollection switchAttributes;
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
        /// 문자열을 파싱하여 데이터로 변환합니다.
        /// </summary>
        /// <returns>
        /// 모든 과정이 성공하면 true를, 그렇지 않다면 false를 반환합니다.
        /// </returns>
        /// <param name="commandLine">
        /// 파싱할 문자열입니다. 
        /// </param>
        /// <param name="options">
        /// 데이터를 설정할 속성과 스위치 특성이 포함되어 있는 인스턴스입니다.
        /// </param>
        public bool TryParse(string commandLine, object options)
        {
            return TryParse(commandLine, options, ParsingOptions.None);
        }

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

        public void Parse(string commandLine, object options)
        {
            Parse(commandLine, options, ParsingOptions.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="options"></param>
        /// <exception cref="SwitchException">
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
                SwitchAttributeCollection switches = null;

                if (options is IOptions)
                {
                    instance = (options as IOptions).Instance;
                    switches = (options as IOptions).SwitchAttributes;
                }

                this.instance = instance;
                this.switchAttributes = switches;
                this.parsingOptions = parsingOptions;

                this.unusedArguments.Clear();

                Regex regex = new Regex(@"^((""(?<exe>[^""]*)"")|(?<exe>\S+))\s+(?<arg>.*)", RegexOptions.ExplicitCapture);
                Match match = regex.Match(commandLine);

                this.command = match.Groups["exe"].ToString();
                this.arguments = match.Groups["arg"].ToString();
                this.arguments = this.arguments.Trim();

                if (arguments.Length == 0)
                    throw new ArgumentException(Resource.NoArguments, commandLine);

                string[] switchLines, unusedArgs;
                SplitSwitches(this.arguments, out switchLines);

                SwitchDescriptorCollection switchCollection = SwitchDescriptorCollection.GetSwitches(instance, switches);
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

        public void PrintSwitchUsage(string switchName)
        {
            this.usagePrinter.PrintSwitchUsage(switchName);
        }

        public void PrintUsage()
        {
            this.usagePrinter.PrintUsage();
        }

        #endregion

        #region protected methods

        protected void OnParsingUnusedArgument(string arg)
        {

        }

        virtual protected UsagePrinter CreateUsagePrinterCore(CommandLineParser parser)
        {
            return new UsagePrinter(parser);
        }

        /// <summary>
        /// 최종적으로 옵션에 유효성을 검사합니다.
        /// </summary>
        /// <param name="options">
        /// 파싱에 사용된 인스턴스입니다.
        /// </param>
        /// <exception cref="Exception">
        /// 데이터의 유효성 검사가 실패 했을때
        /// </exception>
        virtual protected void AssertValidation(object options)
        {

        }

        #endregion

        #region private methods

        void SplitSwitches(string arg, out string[] switchLines)
        {
            using (Tracer tracer = new Tracer("Split Switches"))
            {
                List<string> usedList = new List<string>();
                List<string> unusedList = new List<string>();
                string pattern = string.Format(@"{0}\S+((\s+""[^""]*"")|(\s+[\S-[{0}]][\S]*)|(\s*))*", SwitchAttribute.SwitchDelimiter);
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

        internal SwitchAttributeCollection SwitchAttributes
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
        /// 파싱과정중 생기는 다양한 정보를 출력할 수 있는 처리기를 지정합니다.
        /// </summary>
        public TextWriter TextWriter { get; set; }

        /// <summary>
        /// 파싱중 사용되지 않은 인자들을 가져옵니다.
        /// </summary>
        public string[] UnusedArguments
        {
            get { return this.unusedArguments.ToArray(); }
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