#region License
//Ntreev CommandLineParser for .Net 1.0.4461.33698
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
using Ntreev.Library;

namespace CommandLineParserTest.Library
{
    class CommandLineBuilder
    {
        string executeName;
        Dictionary<string, object> switches = new Dictionary<string, object>();

        public CommandLineBuilder()
        {
            this.executeName = "Test.exe";
        }

        public CommandLineBuilder(string executeName)
        {
            this.executeName = executeName;
        }

        public void AddSwitch(string switchName)
        {
            this.switches.Add(switchName, null);
        }

        public void AddSwitch(string switchName, object arg)
        {
            this.switches.Add(switchName, arg);
        }

        public void AddSwitch(string switchName, params object[] args)
        {
            AddSwitch(switchName, ',', args);
        }

        public void AddSwitch(string switchName, char seperator, params object[] args)
        {
            Args item = new Args();
            item.args = args;
            item.seperator = seperator;
            this.switches.Add(switchName, item);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(this.executeName);
            builder.Append(" ");

            foreach(KeyValuePair<string, object> item in this.switches)
            {
                builder.Append(SwitchAttribute.SwitchDelimiter);
                builder.Append(item.Key);

                object value = item.Value;

                if (value.GetType() == typeof(Args))
                {
                    Args args = (Args)value;
                    builder.Append(" \"");
                    foreach (object arg in args.args)
                    {
                        builder.AppendFormat("{0}{1} ", arg, args.seperator);
                    }
                    builder.Append("\"");
                }
                else
                {
                    builder.AppendFormat("\"{0}\"", value);
                }
            }

            return builder.ToString();
        }

        public object this[string switchName]
        {
            get
            {
                return this.switches[switchName];
            }
        }

        public object this[string switchName, int arrayIndex]
        {
            get
            {
                Args args = (Args)this.switches[switchName];
                return args.args[arrayIndex];
            }
        }

        public int GetArgCount(string switchName)
        {
            object value = this.switches[switchName];
            if(value.GetType() == typeof(Args))
                return ((Args)value).args.Length;

            return value == null ? 0 : 1;
        }

        struct Args
        {
            public object[] args;
            public char seperator;
        }
    }
}