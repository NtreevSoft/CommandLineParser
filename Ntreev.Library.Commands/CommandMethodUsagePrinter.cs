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
            this.PrintUsage(writer, descriptors);
            this.PrintDescription(writer, descriptors);
            this.PrintSubcommands(writer, descriptors);
        }

        private void Print(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor[] memberDescriptors)
        {
            this.PrintSummary(writer, descriptor, memberDescriptors);
            this.PrintUsage(writer, descriptor, memberDescriptors);
            this.PrintDescription(writer, descriptor, memberDescriptors);
            this.PrintRequirements(writer, descriptor, memberDescriptors);
            this.PrintVariables(writer, descriptor, memberDescriptors);
            this.PrintOptions(writer, descriptor, memberDescriptors);
        }

        private void Print(CommandTextWriter writer, CommandMethodDescriptor descriptor, CommandMemberDescriptor memberDescriptor)
        {
            this.PrintSummary(writer, descriptor, memberDescriptor);
            this.PrintUsage(writer, descriptor, memberDescriptor);
            this.PrintDescription(writer, descriptor, memberDescriptor);
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
            if (this.Description == string.Empty)
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
            writer.WriteLine(Resources.SubCommands);
            writer.Indent++;

            foreach (var item in descriptors)
            {
                writer.WriteLine(item.Name);
                writer.Indent++;
                if (item.Summary == string.Empty)
                    writer.WriteMultiline("*<empty>*");
                else
                    writer.WriteMultiline(item.Summary);
                writer.Indent--;
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
                        orderby item.Required descending
                        select this.GetString(item);

            var maxWidth = writer.Width - (writer.TabString.Length * writer.Indent);

            var line = descriptor.Name;

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
            var items = memberDescriptors.Where(item => item.Required == true).ToArray();
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
                                .Where(item => item.Required == false)
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

            if (descriptor.Description != string.Empty)
            {
                writer.Indent++;
                writer.WriteMultiline(descriptor.Description);
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

            writer.Indent++;
            writer.WriteMultiline(descriptor.Description);
            writer.Indent--;
        }

        private string GetString(CommandMemberDescriptor descriptor)
        {
            if (descriptor.Required == true)
            {
                var text = string.Empty;
                if (descriptor is CommandParameterDescriptor == true)
                {
                    text = descriptor.DisplayName;
                }
                else
                {
                    var patternItems = new string[] { descriptor.ShortNamePattern, descriptor.NamePattern, };
                    text = string.Join(" | ", patternItems.Where(item => item != string.Empty));
                }
                if (descriptor.DefaultValue == DBNull.Value)
                    return string.Format("<{0}>", text);
                return string.Format("<{0}={1}>", text, descriptor.DefaultValue ?? "null");
            }
            else
            {
                var patternItems = new string[] { descriptor.ShortNamePattern, descriptor.NamePattern, };
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
