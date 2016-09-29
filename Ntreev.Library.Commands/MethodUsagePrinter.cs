using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    public class MethodUsagePrinter
    {
        private readonly string name;
        private readonly object instance;
        private readonly MethodDescriptor[] descriptors;
        private readonly string summary;
        private readonly string description;

        public MethodUsagePrinter(string name, object instance)
        {
            this.name = name;
            this.instance = instance;
            this.descriptors = CommandDescriptor.GetMethodDescriptors(instance.GetType()).ToArray();
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

        public virtual void Print(TextWriter writer, string methodName)
        {
            using (var tw = new CommandTextWriter(writer))
            {
                this.Print(tw, methodName);
            }
        }

        protected MethodDescriptor[] MethodDescriptors
        {
            get { return this.descriptors; }
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

        protected MethodDescriptor[] Methods
        {
            get { return this.descriptors; }
        }

        private void Print(CommandTextWriter writer)
        {
            this.PrintSummary(writer);
            this.PrintUsage(writer);
            this.PrintDescription(writer);
            this.PrintSubcommands(writer);
        }

        private void Print(CommandTextWriter writer, string methodName)
        {
            var descriptor = this.Methods.FirstOrDefault(item => item.Name == methodName);
            if (descriptor == null)
                throw new ArgumentException(string.Format("'{0}'은(는) 존재하지 않는 메소드입니다."));
            this.PrintSummary(writer, descriptor);
            this.PrintUsage(writer, descriptor);
            this.PrintDescription(writer, descriptor);
            this.PrintRequirements(writer, descriptor);
            this.PrintOptions(writer, descriptor);
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

        private void PrintSummary(CommandTextWriter writer, MethodDescriptor descriptor)
        {
            if (this.Description == string.Empty)
                return;

            writer.WriteLine(Resources.Summary);
            writer.Indent++;
            writer.WriteLine(descriptor.Summary);
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

        private void PrintDescription(CommandTextWriter writer, MethodDescriptor descriptor)
        {
            if (descriptor.Description == string.Empty)
                return;

            writer.WriteLine(Resources.Description);
            writer.Indent++;
            writer.WriteMultiline(descriptor.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintSubcommands(CommandTextWriter writer)
        {
            writer.WriteLine(Resources.SubCommands);
            writer.Indent++;

            foreach (var item in this.Methods)
            {
                writer.WriteLine(item.Name);
                writer.Indent++;
                if (item.Summary == string.Empty)
                    writer.WriteMultiline("*요약이 정의되지 않았습니다.*");
                else
                    writer.WriteMultiline(item.Summary);
                writer.Indent--;
            }

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintUsage(CommandTextWriter writer)
        {
            writer.WriteLine(Resources.Usage);
            writer.Indent++;

            writer.WriteLine("{0} <sub-command> [options...]", this.Name);

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintUsage(CommandTextWriter writer, MethodDescriptor descriptor)
        {
            writer.WriteLine(Resources.Usage);
            writer.Indent++;

            this.PrintMethodUsage(writer, descriptor);
            //var switches = this.GetSwitchesString(descriptor.Switches.Where(i => i.Required));
            //var options = this.GetOptionsString(descriptor.Switches.Where(i => i.Required == false));
            //writer.WriteLine("{0} {1} {2} {3}", this.Name, descriptor.Name, switches, options);

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintMethodUsage(CommandTextWriter writer, MethodDescriptor descriptor)
        {
            var indent = writer.Indent;
            var query = from item in descriptor.Switches
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

        private void PrintRequirements(CommandTextWriter writer, MethodDescriptor descriptor)
        {
            var switches = descriptor.Switches.Where(i => i.Required == true).ToArray();
            if (switches.Any() == false)
                return;

            writer.WriteLine(Resources.Requirements);
            writer.Indent++;
            for (var i = 0; i < switches.Length; i++)
            {
                var item = switches[i];
                this.PrintRequirement(writer, item);
                if (i + 1 < switches.Length)
                    writer.WriteLine();
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintOptions(CommandTextWriter writer, MethodDescriptor descriptor)
        {
            var switches = descriptor.Switches.Where(i => i.Required == false).ToArray();

            if (switches.Any() == false)
                return;

            writer.WriteLine(Resources.Options);
            writer.Indent++;

            for (var i = 0; i < switches.Length; i++)
            {
                var item = switches[i];
                this.PrintOption(writer, item);
                if (i + 1 < switches.Length)
                    writer.WriteLine();
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintRequirement(CommandTextWriter textWriter, SwitchDescriptor descriptor)
        {
            if (descriptor.SwitchType == SwitchTypes.Parameter)
            {
                textWriter.WriteLine(descriptor.DisplayName);
            }
            else
            {
                if (descriptor.ShortNamePattern != string.Empty)
                    textWriter.WriteLine(descriptor.ShortNamePattern);
                if (descriptor.NamePattern != string.Empty)
                    textWriter.WriteLine(descriptor.NamePattern);
            }

            if (descriptor.Description != string.Empty)
            {
                textWriter.Indent++;
                textWriter.WriteMultiline(descriptor.Description);
                textWriter.Indent--;
            }
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
        }

        //private string GetOptionString(SwitchDescriptor descriptor)
        //{
        //    var patternItems = new string[] { descriptor.ShortNamePattern, descriptor.NamePattern, };
        //    var patternText = string.Join(" | ", patternItems.Where(i => i != string.Empty));
        //    return string.Format("[{0}]", patternText);
        //}

        //private string GetOptionsString(IEnumerable<SwitchDescriptor> switches)
        //{
        //    var query = from item in switches
        //                let patternItems = new string[] { item.ShortNamePattern, item.NamePattern, }
        //                select string.Join(" | ", patternItems.Where(i => i != string.Empty));

        //    return string.Join(" ", query.Select(item => "[" + item + "]"));
        //}

        private string GetString(SwitchDescriptor descriptor)
        {
            if (descriptor.Required == true)
            {
                var text = string.Empty;
                if (descriptor.SwitchType == SwitchTypes.Parameter)
                {
                    text = descriptor.DisplayName;
                }
                else
                {
                    var patternItems = new string[] { descriptor.ShortNamePattern, descriptor.NamePattern, };
                    text = string.Join(" | ", patternItems.Where(i => i != string.Empty));
                }
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

        //private string GetSwitchString(SwitchDescriptor descriptor)
        //{
        //    var text = descriptor.SwitchType == SwitchTypes.Parameter ? descriptor.DisplayName : this.GetOptionString(descriptor);
        //    if (descriptor.DefaultValue == DBNull.Value)
        //        return string.Format("<{0}>", text);
        //    return string.Format("<{0} = {1}>", text, descriptor.DefaultValue ?? "null");
        //}

        //private string GetSwitchesString(IEnumerable<SwitchDescriptor> switches)
        //{
        //    return string.Join(" ", switches.Select(item =>
        //    {
        //        var text = item.SwitchType == SwitchTypes.Parameter ? item.DisplayName : this.GetOptionString(item);
        //        if (item.DefaultValue == DBNull.Value)
        //            return string.Format("<{0}>", text);
        //        return string.Format("<{0}={1}>", text, item.DefaultValue ?? "null");
        //    }));
        //}
    }
}
