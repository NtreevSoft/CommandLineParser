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
using Ntreev.Library.Commands.Properties;
using System.CodeDom.Compiler;

namespace Ntreev.Library.Commands
{
    public class CommandUsagePrinter
    {
        private readonly string name;
        private readonly object instance;
        private readonly string description;
        private readonly SwitchDescriptor[] switches;

        public CommandUsagePrinter(string name, object instance)
        {
            this.name = name;
            this.instance = instance;
            this.description = instance.GetType().GetDescription();
            this.switches = CommandDescriptor.GetSwitchDescriptors(instance.GetType()).ToArray();
        }

        public virtual void PrintUsage(TextWriter textWriter)
        {
            using (var tw = new IndentedTextWriter(textWriter))
            {
                this.PrintUsage(tw);
            }
        }

        protected string Name
        {
            get { return this.name; }
        }

        protected object Instance
        {
            get { return this.instance; }
        }

        protected string Description
        {
            get { return this.description; }
        }

        protected SwitchDescriptor[] Switches
        {
            get { return this.switches; }
        }

        //protected SwitchDescriptor[] Options
        //{
        //    get { return this.options; }
        //}

        private void PrintUsage(IndentedTextWriter textWriter)
        {
            this.PrintName(textWriter);
            this.PrintSynopsis(textWriter);
            this.PrintDescription(textWriter);
            this.PrintOptions(textWriter);
        }

        private void PrintName(IndentedTextWriter textWriter)
        {
            var summary = this.Instance.GetType().GetSummary();
            textWriter.WriteLine("Name");
            textWriter.Indent++;
            textWriter.WriteLine("{0} - {1}", this.Name, summary);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintSynopsis(IndentedTextWriter textWriter)
        {
            var query = from item in this.switches
                        where item.Required == false
                        let patternItems = new string[] { item.ShortNamePattern, item.NamePattern, }
                        select string.Join(" | ", patternItems.Where(i => i != string.Empty));

            var options = query.Aggregate("", (l, n) => l += "[" + n + "] ", item => item);

            var switches = this.Switches.Where(item => item.Required).Aggregate("", (l, n) => l += "[" + n.DisplayName + "] ", item => item);

            textWriter.WriteLine("Synopsis");
            textWriter.Indent++;
            textWriter.WriteLine("{0}{1}", options, switches);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintDescription(IndentedTextWriter textWriter)
        {
            textWriter.WriteLine("Description");
            textWriter.Indent++;
            textWriter.WriteMultiline(this.Description);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintOptions(IndentedTextWriter textWriter)
        {
            textWriter.WriteLine("Options");
            textWriter.Indent++;
            foreach (var item in this.Switches.Where(i => i.Required == true))
            {
                this.PrintSwitch(textWriter, item);
            }
            foreach (var item in this.Switches.Where(i => i.Required == false))
            {
                this.PrintOption(textWriter, item);
            }
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintSwitch(IndentedTextWriter textWriter, SwitchDescriptor descriptor)
        {
            if (descriptor.Description == string.Empty)
                return;

            textWriter.WriteLine(descriptor.DisplayName);
            textWriter.Indent++;
            textWriter.WriteMultiline(descriptor.Description);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintOption(IndentedTextWriter textWriter, SwitchDescriptor descriptor)
        {
            if (descriptor.ShortNamePattern != string.Empty)
                textWriter.WriteLine(descriptor.ShortNamePattern);
            if (descriptor.NamePattern != string.Empty)
                textWriter.WriteLine(descriptor.NamePattern);

            textWriter.Indent++;
            textWriter.WriteMultiline(descriptor.Description);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private string GetOptionsString(IEnumerable<SwitchDescriptor> switches)
        {
            var query = from item in switches
                        let patternItems = new string[] { item.ShortNamePattern, item.NamePattern, }
                        select string.Join(" | ", patternItems.Where(i => i != string.Empty));

            return string.Join(" ", query.Select(item => "[" + item + "]"));
        }

        private string GetSwitchesString(IEnumerable<SwitchDescriptor> switches)
        {
            return string.Join(" ", switches.Select(item => "[" + item.DisplayName + "]"));
        }
    }
}