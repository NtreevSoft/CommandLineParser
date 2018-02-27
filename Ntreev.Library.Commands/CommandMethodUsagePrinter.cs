//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    public class CommandMethodUsagePrinter
    {
        private readonly string name;
        private readonly object instance;
        private readonly string summary;
        private readonly string description;

        public CommandMethodUsagePrinter(string name, object instance)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(instance.GetType());
            this.name = name;
            this.instance = instance;
            this.summary = provider.GetSummary(instance);
            this.description = provider.GetDescription(instance);
        }

        public virtual void Print(TextWriter writer, CommandMethodDescriptor[] descriptors)
        {
            using (var tw = new CommandTextWriter(writer))
            {
                this.Print(tw, descriptors);
            }
        }

        public virtual void Print(TextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            using (var tw = new CommandTextWriter(writer))
            {
                this.Print(tw, descriptor, memberDescriptors);
            }
        }

        public virtual void Print(TextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor memberDescriptor)
        {
            using (var tw = new CommandTextWriter(writer))
            {
                this.Print(tw, descriptor, memberDescriptor);
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        public object Instance
        {
            get { return this.instance; }
        }

        public string Summary
        {
            get { return this.summary; }
        }

        public string Description
        {
            get { return this.description; }
        }

        private void Print(CommandTextWriter writer, CommandMethodDescriptor[] descriptors)
        {
            this.PrintSummary(writer, descriptors);
            this.PrintDescription(writer, descriptors);
            this.PrintUsage(writer, descriptors);
            this.PrintSubcommands(writer, descriptors);
        }

        private void Print(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            this.PrintSummary(writer, descriptor, memberDescriptors);
            this.PrintDescription(writer, descriptor, memberDescriptors);
            this.PrintUsage(writer, descriptor, memberDescriptors);
            this.PrintRequirements(writer, descriptor, memberDescriptors);
            this.PrintVariables(writer, descriptor, memberDescriptors);
            this.PrintOptions(writer, descriptor, memberDescriptors);
        }

        private void Print(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor memberDescriptor)
        {
            this.PrintSummary(writer, descriptor, memberDescriptor);
            this.PrintDescription(writer, descriptor, memberDescriptor);
            this.PrintUsage(writer, descriptor, memberDescriptor);
        }

        private void PrintSummary(CommandTextWriter writer, CommandMethodDescriptor[] descriptors)
        {
            if (this.Summary == string.Empty)
                return;

            writer.WriteLine(Resources.Summary);
            writer.Indent++;
            writer.WriteLine(this.Summary);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintSummary(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            if (this.Summary == string.Empty)
                return;

            writer.WriteLine(Resources.Summary);
            writer.Indent++;
            writer.WriteLine(descriptor.Summary);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintSummary(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor memberDescriptor)
        {
            writer.WriteLine(Resources.Summary);
            writer.Indent++;
            writer.WriteLine(memberDescriptor.Summary);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintDescription(CommandTextWriter writer, CommandMethodDescriptor[] descriptors)
        {
            if (this.Description == string.Empty)
                return;

            writer.WriteLine(Resources.Description);
            writer.Indent++;
            writer.WriteMultiline(this.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintDescription(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            if (descriptor.Description == string.Empty)
                return;

            writer.WriteLine(Resources.Description);
            writer.Indent++;
            writer.WriteMultiline(descriptor.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintDescription(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor memberDescriptor)
        {
            writer.WriteLine(Resources.Description);
            writer.Indent++;
            writer.WriteMultiline(memberDescriptor.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintSubcommands(CommandTextWriter writer, CommandMethodDescriptor[] descriptors)
        {
            writer.WriteLine(Resources.CommandMethod);
            writer.Indent++;

            foreach (var item in descriptors)
            {
                writer.WriteLine(item.Name);
                var summary = item.Summary != string.Empty ? item.Summary : item.Description;
                if (summary != string.Empty)
                {
                    writer.Indent++;
                    writer.WriteMultiline(summary);
                    writer.Indent--;
                }
                writer.WriteLine();
            }

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintUsage(CommandTextWriter writer, CommandMethodDescriptor[] descriptors)
        {
            writer.WriteLine(Resources.Usage);
            writer.Indent++;

            writer.WriteLine("{0} <sub-command> [options...]", this.Name);

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintUsage(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            writer.WriteLine(Resources.Usage);
            writer.Indent++;

            this.PrintMethodUsage(writer, descriptor, memberDescriptors);

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintUsage(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor memberDescriptor)
        {
            writer.WriteLine(Resources.Usage);
            writer.Indent++;

            this.PrintOption(writer, memberDescriptor);

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintMethodUsage(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            var indent = writer.Indent;
            var query = from item in memberDescriptors
                        orderby item.IsRequired descending
                        select this.GetString(item);

            var maxWidth = writer.Width - (writer.TabString.Length * writer.Indent);

            var line = this.name + " " + descriptor.Name;

            foreach (var item in query)
            {
                if (line != string.Empty)
                    line += " ";

                if (line.Length + item.Length >= maxWidth)
                {
                    writer.WriteLine(line);
                    line = string.Empty.PadLeft(descriptor.Name.Length + 1);
                }
                line += item;
            }

            writer.WriteLine(line);
            writer.Indent = indent;
        }

        private void PrintRequirements(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            var items = memberDescriptors.Where(item => item.IsRequired == true).ToArray();
            if (items.Any() == false)
                return;

            writer.WriteLine(Resources.Requirements);
            writer.Indent++;
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                this.PrintRequirement(writer, item);
                if (i + 1 < items.Length)
                    writer.WriteLine();
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintVariables(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            var variables = memberDescriptors.FirstOrDefault(item => item is CommandMemberArrayDescriptor);
            if (variables == null)
                return;

            writer.WriteLine(Resources.Variables);
            writer.Indent++;
            this.PrintVariables(writer, variables);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintOptions(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            var items = memberDescriptors.Where(item => item is CommandMemberArrayDescriptor == false)
                                .Where(item => item.IsRequired == false)
                                .ToArray();
            if (items.Any() == false)
                return;

            writer.WriteLine(Resources.Options);
            writer.Indent++;

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                this.PrintOption(writer, item);
                if (i + 1 < items.Length)
                    writer.WriteLine();
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintRequirement(CommandTextWriter writer, CommandMemberDescriptor descriptor)
        {
            if (descriptor is CommandParameterDescriptor == true)
            {
                writer.WriteLine(descriptor.DisplayName);
            }
            else
            {
                if (descriptor.ShortNamePattern != string.Empty)
                    writer.WriteLine(descriptor.ShortNamePattern);
                if (descriptor.NamePattern != string.Empty)
                    writer.WriteLine(descriptor.NamePattern);
            }

            var description = descriptor.Summary != string.Empty ? descriptor.Summary : descriptor.Description;
            if (description != string.Empty)
            {
                writer.Indent++;
                writer.WriteMultiline(description);
                writer.Indent--;
            }
        }

        private void PrintVariables(CommandTextWriter writer, CommandMemberDescriptor descriptor)
        {
            writer.WriteLine(descriptor.Name + " ...");

            writer.Indent++;
            writer.WriteMultiline(descriptor.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintOption(CommandTextWriter writer, CommandMemberDescriptor descriptor)
        {
            if (descriptor.ShortNamePattern != string.Empty)
                writer.WriteLine(descriptor.ShortNamePattern);
            if (descriptor.NamePattern != string.Empty)
                writer.WriteLine(descriptor.NamePattern);

            var description = descriptor.Summary != string.Empty ? descriptor.Summary : descriptor.Description;
            if (description != string.Empty)
            {
                writer.Indent++;
                writer.WriteMultiline(description);
                writer.Indent--;
            }
        }

        private string GetString(CommandMemberDescriptor descriptor)
        {
            if (descriptor.IsRequired == true)
            {
                var text = string.Empty;
                if (descriptor is CommandParameterDescriptor == true)
                {
                    text = descriptor.DisplayName;
                }
                else
                {
                    var patternItems = new string[] { descriptor.ShortNamePattern, descriptor.NamePattern };
                    text = string.Join(" | ", patternItems.Where(item => item != string.Empty));
                }
                if (descriptor.DefaultValue == DBNull.Value)
                    return string.Format("<{0}>", text);
                return string.Format("<{0}={1}>", text, descriptor.DefaultValue ?? "null");
            }
            else
            {
                var patternItems = new string[] { descriptor.ShortNamePattern, descriptor.NamePattern };
                var patternText = string.Join(" | ", patternItems.Where(item => item != string.Empty));
                if (descriptor is CommandParameterArrayDescriptor == true)
                {
                    patternText += " ...";
                }
                return string.Format("[{0}]", patternText);
            }
        }
    }
}
