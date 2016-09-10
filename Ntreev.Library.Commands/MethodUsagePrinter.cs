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
        private readonly string description;
        private readonly MethodDescriptor[] descriptors;

        public MethodUsagePrinter(string name, object instance)
        {
            this.name = name;
            this.instance = instance;
            this.description = instance.GetType().GetDescription();
            this.descriptors = CommandDescriptor.GetMethodDescriptors(instance.GetType()).ToArray();
        }

        public virtual void Print(TextWriter writer)
        {
            using (var tw = new IndentedTextWriter(writer))
            {
                this.Print(tw);
            }
        }

        public virtual void Print(TextWriter writer, string methodName)
        {
            using (var tw = new IndentedTextWriter(writer))
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

        protected string Description
        {
            get { return this.description; }
        }

        protected MethodDescriptor[] Methods
        {
            get { return this.descriptors; }
        }

        private void PrintSummary(IndentedTextWriter writer)
        {
            var summary = this.Instance.GetType().GetSummary();
            writer.WriteLine("Name");
            writer.Indent++;
            writer.WriteLine("{0} - {1}", this.Name, summary);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintSummary(IndentedTextWriter writer, MethodDescriptor descriptor)
        {
            var summary = descriptor.Summary;
            writer.WriteLine("Name");
            writer.Indent++;
            writer.WriteLine("{0} {1} - {2}", this.Name, descriptor.Name, summary);
            writer.Indent--;
            writer.WriteLine();
        }

        private void Print(IndentedTextWriter writer)
        {
            this.PrintSummary(writer);
            this.PrintUsage(writer);
            this.PrintDescription(writer);
            this.PrintRequirements(writer);
            this.PrintOptions(writer);
        }

        private void Print(IndentedTextWriter writer, string methodName)
        {
            var descriptor = this.Methods.First(item => item.Name == methodName);
            this.PrintSummary(writer, descriptor);
            this.PrintUsage(writer, descriptor);
            this.PrintDescription(writer, descriptor);
            this.PrintRequirements(writer, descriptor);
            this.PrintOptions(writer, descriptor);
        }

        private void PrintDescription(IndentedTextWriter writer)
        {
            writer.WriteLine("Description");
            writer.Indent++;
            writer.WriteMultiline(this.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintDescription(IndentedTextWriter writer, MethodDescriptor descriptor)
        {
            writer.WriteLine("Description");
            writer.Indent++;
            writer.WriteMultiline(descriptor.Description);
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintUsage(IndentedTextWriter writer)
        {
            writer.WriteLine("Usage");
            writer.Indent++;

            foreach (var item in this.Methods)
            {
                var switches = this.GetSwitchesString(item.Switches.Where(i => i.Required));
                var options = this.GetOptionsString(item.Switches.Where(i => i.Required == false));
                writer.WriteLine("{0} {1} {2} {3}", this.Name, item.Name, switches, options);
            }

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintUsage(IndentedTextWriter writer, MethodDescriptor descriptor)
        {
            writer.WriteLine("Usage");
            writer.Indent++;

            var switches = this.GetSwitchesString(descriptor.Switches.Where(i => i.Required));
            var options = this.GetOptionsString(descriptor.Switches.Where(i => i.Required == false));
            writer.WriteLine("{0} {1} {2} {3}", this.Name, descriptor.Name, switches, options);

            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintRequirements(IndentedTextWriter writer)
        {
            writer.WriteLine("Requirements");
            writer.Indent++;
            foreach (var item in this.Methods)
            {
                var switches = this.GetSwitchesString(item.Switches.Where(i => i.Required));
                var options = this.GetOptionsString(item.Switches.Where(i => i.Required == false));
                writer.WriteLine("{0} {1} {2} {3}", this.Name, item.Name, switches, options);

                writer.Indent++;
                writer.WriteMultiline(item.Description);
                writer.Indent--;
                writer.WriteLine();
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintOptions(IndentedTextWriter writer)
        {
            writer.WriteLine("Options");
            writer.Indent++;
            foreach (var item in this.Methods)
            {
                var switches = this.GetSwitchesString(item.Switches.Where(i => i.Required));
                var options = this.GetOptionsString(item.Switches.Where(i => i.Required == false));
                writer.WriteLine("{0} {1} {2} {3}", this.Name, item.Name, switches, options);

                writer.Indent++;
                writer.WriteMultiline(item.Description);
                writer.Indent--;
                writer.WriteLine();
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintRequirements(IndentedTextWriter writer, MethodDescriptor descriptor)
        {
            var switches = descriptor.Switches.Where(i => i.Required == true).ToArray();
            if (switches.Any() == false)
                return;

            writer.WriteLine("Requirements");
            writer.Indent++;
            foreach (var item in switches)
            {
                this.PrintRequirement(writer, item);
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintOptions(IndentedTextWriter writer, MethodDescriptor descriptor)
        {
            writer.WriteLine("Options");
            writer.Indent++;

            foreach (var item in descriptor.Switches.Where(i => i.Required == false))
            {
                this.PrintOption(writer, item);
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintRequirement(IndentedTextWriter textWriter, SwitchDescriptor descriptor)
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
            textWriter.WriteLine();
        }

        private void PrintOption(IndentedTextWriter writer, SwitchDescriptor descriptor)
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
                var text = item.SwitchType == SwitchTypes.Parameter ? item.DisplayName : this.GetOptionString(item);
                if (item.DefaultValue == DBNull.Value)
                    return string.Format("<{0}>", text);
                return string.Format("<{0} = {1}>", text, item.DefaultValue ?? "null");
            }));
        }
    }
}
