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
        private readonly SwitchDescriptor[] switches;
        private readonly string summary;
        private readonly string description;

        public CommandUsagePrinter(string name, object instance)
        {
            this.name = name;
            this.instance = instance;
            this.switches = CommandDescriptor.GetSwitchDescriptors(instance.GetType()).ToArray();
            var provider = CommandDescriptor.GetUsageDescriptionProvider(instance.GetType());
            this.summary = provider.GetSummary(instance);
            this.description = provider.GetDescription(instance);
        }

        public virtual void Print(TextWriter writer)
        {
            using (var tw = new CommandTextWriter(writer))
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

        protected string Summary
        {
            get { return this.summary; }
        }

        protected string Description
        {
            get { return this.description; }
        }

        protected SwitchDescriptor[] Switches
        {
            get { return this.switches; }
        }

        private void Print(CommandTextWriter writer)
        {
            this.PrintSummary(writer);
            this.PrintUsage(writer);
            this.PrintDescription(writer);
            this.PrintRequirements(writer);
            this.PrintOptions(writer);
        }

        private void PrintSummary(CommandTextWriter writer)
        {
            if (this.Summary == string.Empty)
                return;

            writer.WriteLine(Resources.Summary);
            writer.Indent++;
            writer.WriteLine(this.Summary);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintUsage(CommandTextWriter writer)
        {
            var query = from item in this.Switches
                        orderby item.Required descending
                        select this.GetString(item);

            var maxWidth = writer.Width - (writer.TabString.Length * writer.Indent);

            var line = this.Name;

            writer.WriteLine(Resources.Usage);
            writer.Indent++;

            foreach (var item in query)
            {
                if (line != string.Empty)
                    line += " ";

                if (line.Length + item.Length >= maxWidth)
                {
                    writer.WriteLine(line);
                    line = string.Empty.PadLeft(this.Name.Length + 1);
                }
                line += item;
            }
            writer.WriteLine(line);

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintDescription(CommandTextWriter writer)
        {
            if (this.Description == string.Empty)
                return;

            writer.WriteLine(Resources.Description);
            writer.Indent++;
            writer.WriteMultiline(this.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintRequirements(CommandTextWriter writer)
        {
            var switches = this.Switches.Where(i => i.Required == true).ToArray();
            if (switches.Any() == false)
                return;

            writer.WriteLine(Resources.Requirements);
            writer.Indent++;
            foreach (var item in switches)
            {
                this.PrintRequirement(writer, item);
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintOptions(CommandTextWriter writer)
        {
            var switches = this.Switches.Where(i => i.Required == false).ToArray();
            if (switches.Any() == false)
                return;

            writer.WriteLine(Resources.Options);
            writer.Indent++;
            foreach (var item in switches)
            {
                this.PrintOption(writer, item);
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintRequirement(CommandTextWriter writer, SwitchDescriptor descriptor)
        {
            writer.WriteLine(descriptor.DisplayName);
            if (descriptor.Description != string.Empty)
            {
                writer.Indent++;
                writer.WriteMultiline(descriptor.Description);
                writer.Indent--;
            }
            writer.WriteLine();
        }

        private void PrintOption(CommandTextWriter writer, SwitchDescriptor descriptor)
        {
            if (descriptor.ShortNamePattern != string.Empty)
                writer.WriteLine(descriptor.ShortNamePattern);
            if (descriptor.NamePattern != string.Empty)
                writer.WriteLine(descriptor.NamePattern);

            writer.Indent++;
            writer.WriteMultiline(descriptor.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private string GetString(SwitchDescriptor descriptor)
        {
            if (descriptor.Required == true)
            {
                var text = descriptor.DisplayName;
                if (descriptor.DefaultValue == DBNull.Value)
                    return string.Format("<{0}>", text);
                return string.Format("<{0}={1}>", text, descriptor.DefaultValue ?? "null");
            }
            else
            {
                var patternItems = new string[] { descriptor.ShortNamePattern, descriptor.NamePattern, };
                var patternText = string.Join(" | ", patternItems.Where(i => i != string.Empty));
                return string.Format("[{0}]", patternText);
            }
        }
    }
}