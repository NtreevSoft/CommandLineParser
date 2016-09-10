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

        public virtual void Print(TextWriter writer)
        {
            using (var tw = new IndentedTextWriter(writer))
            {
                this.Print(tw);
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

        private void Print(IndentedTextWriter textWriter)
        {
            this.PrintSummary(textWriter);
            this.PrintUsage(textWriter);
            this.PrintDescription(textWriter);
            this.PrintRequirements(textWriter);
            this.PrintOptions(textWriter);
        }

        private void PrintSummary(IndentedTextWriter textWriter)
        {
            var summary = this.Instance.GetType().GetSummary();
            textWriter.WriteLine("Summary");
            textWriter.Indent++;
            textWriter.WriteLine(summary);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintUsage(IndentedTextWriter writer)
        {
            //var query = from item in this.switches
            //            where item.Required == false
            //            let patternItems = new string[] { item.ShortNamePattern, item.NamePattern, }
            //            select string.Join(" | ", patternItems.Where(i => i != string.Empty));

            //var options = string.Join(" ", query.Select(item => "[" + item + "]"));

            //var switches = string.Join(" ", this.Switches.Where(item => item.Required).Select(item => "<" + item.DisplayName + ">"));

            //textWriter.WriteLine("Usage");
            //textWriter.Indent++;
            //textWriter.WriteLine("{0} {1} {2}", this.Name, switches, options);
            //textWriter.Indent--;
            //textWriter.WriteLine();

            writer.WriteLine("Usage");
            writer.Indent++;

            var switches = this.GetSwitchesString(this.Switches.Where(i => i.Required));
            var options = this.GetOptionsString(this.Switches.Where(i => i.Required == false));
            writer.WriteLine("{0} {1} {2}", this.Name, switches, options);

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintDescription(IndentedTextWriter textWriter)
        {
            textWriter.WriteLine("Description");
            textWriter.Indent++;
            textWriter.WriteMultiline(this.Description);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintRequirements(IndentedTextWriter textWriter)
        {
            var switches = this.Switches.Where(i => i.Required == true).ToArray();
            if (switches.Any() == false)
                return;

            textWriter.WriteLine("Requirements");
            textWriter.Indent++;
            foreach (var item in switches)
            {
                this.PrintRequirement(textWriter, item);
            }
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintOptions(IndentedTextWriter textWriter)
        {
            var switches = this.Switches.Where(i => i.Required == false).ToArray();
            if (switches.Any() == false)
                return;

            textWriter.WriteLine("Options");
            textWriter.Indent++;
            foreach (var item in switches)
            {
                this.PrintOption(textWriter, item);
            }
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintRequirement(IndentedTextWriter textWriter, SwitchDescriptor descriptor)
        {
            textWriter.WriteLine(descriptor.DisplayName);
            if (descriptor.Description != string.Empty)
            {
                textWriter.Indent++;
                textWriter.WriteMultiline(descriptor.Description);
                textWriter.Indent--;
            }
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

        private string GetOptionString(SwitchDescriptor descriptor)
        {
            var patternItems = new string[] { descriptor.ShortNamePattern, descriptor.NamePattern, };
            return string.Join(" | ", patternItems.Where(i => i != string.Empty));
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
            return string.Join(" ", switches.Select(item =>
            {
                var text = item.Required == true ? item.DisplayName : this.GetOptionString(item);
                if (item.DefaultValue == DBNull.Value)
                    return string.Format("<{0}>", text);
                return string.Format("<{0} = {1}>", text, item.DefaultValue ?? "null");
            }));
        }
    }
}