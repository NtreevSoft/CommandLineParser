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
using System.IO;
using System.Reflection;
using Ntreev.Library.Properties;
using System.CodeDom.Compiler;

namespace Ntreev.Library
{
    public class SwitchUsagePrinter : UsagePrinter
    {
        private string location;
        private readonly Type type;

        public SwitchUsagePrinter(Type type, string command)
            : base(type, command)
        {
             
        }

         public string Usage { get; set; }

        public void PrintSwitchUsage(TextWriter textWriter, string switchName)
        {
            this.PrintSwitchUsage(textWriter, switchName, 0);
        }

        public void PrintSwitchUsage(TextWriter textWriter, string switchName, int indentLevel)
        {
            SwitchDescriptor switchDescriptor = CommandDescriptor.GetSwitchDescriptors(this.type)[switchName];
            this.OnPrintSwitchUsage(switchDescriptor, textWriter, indentLevel);
        }

        //public void PrintUsage(TextWriter textWriter)
        //{
        //    this.PrintUsage(textWriter, 0);
        //}

        public override void PrintUsage(TextWriter textWriter, int indentLevel)
        {
            SwitchDescriptor[] switches = CommandDescriptor.GetSwitchDescriptors(this.type).ToArray();
            OnPrintUsage(switches, textWriter, indentLevel);
        }

        protected virtual void OnPrintUsage(SwitchDescriptor[] switchDescriptors, TextWriter textWriter, int indentLevel)
        {
            using (IndentedTextWriter tw = new IndentedTextWriter(textWriter))
            {
                tw.Indent = indentLevel;
                tw.WriteLine("{0} {1}", this.Title, this.Version);
                tw.WriteLine(this.Copyright);
                tw.WriteLine(this.Description);
                tw.WriteLine(this.License);
                tw.WriteLine();
                if (this.Usage == null)
                    tw.WriteLine("{0}: {1}", Resources.Usage, this.GetDefaultUsage(switchDescriptors));
                else
                    tw.WriteLine("{0}: {1}", Resources.Usage, this.Usage);

                foreach (SwitchDescriptor item in switchDescriptors)
                {
                    SwitchUsageProvider usageProvider = item.UsageProvider;
                    tw.Indent++;
                    tw.WriteLine("{0} {1}", usageProvider.Usage, usageProvider.Description);

                    if (usageProvider.ArgumentTypeDescription != string.Empty)
                    {
                        tw.Indent++;
                        tw.WriteLine("{0}", usageProvider.ArgumentTypeDescription);
                        tw.Indent--;
                    }
                    tw.Indent--;
                }
            }
        }

        protected virtual void OnPrintSwitchUsage(SwitchDescriptor switchDescriptor, TextWriter textWriter, int indentLevel)
        {
            using (IndentedTextWriter tw = new IndentedTextWriter(textWriter))
            {
                tw.Indent = indentLevel;
                if (switchDescriptor == null)
                {
                    tw.WriteLine("{0} is invalid switch", switchDescriptor.Name);
                }
                else
                {
                    SwitchUsageProvider usageProvider = switchDescriptor.UsageProvider;
                    tw.WriteLine("Invalid usage");
                    tw.WriteLine("{0} {1}", usageProvider.Usage, usageProvider.Description);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        string GetDefaultUsage(SwitchDescriptor[] switchDescriptors)
        {
            FileInfo fileInfo = new FileInfo(this.location);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("{0}", fileInfo.Name));

            foreach (SwitchDescriptor item in switchDescriptors)
            {
                if (item.Required == false)
                    continue;
                stringBuilder.Append(" " + item.UsageProvider.Usage);
            }

            stringBuilder.Append(" [options | ...]");

            return stringBuilder.ToString();
        }

        //public void PrintUsage(TextWriter textWriter, string memberName)
        //{
        //    this.PrintSwitchUsage(textWriter, memberName);
        //}

        public override void PrintUsage(TextWriter textWriter, string memberName, int indentLevel)
        {
            this.PrintSwitchUsage(textWriter, memberName, indentLevel);
        }
    }
}