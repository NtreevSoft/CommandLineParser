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

        public virtual void PrintUsage(TextWriter writer)
        {
            using (var tw = new IndentedTextWriter(writer))
            {
                this.PrintUsage(tw);
            }
        }

        public virtual void PrintUsage(TextWriter writer, string methodName)
        {
            using (var tw = new IndentedTextWriter(writer))
            {
                this.PrintUsage(tw, methodName);
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

        private void PrintUsage(IndentedTextWriter writer)
        {
            this.PrintSummary(writer);
            this.PrintSynopsis(writer);
            this.PrintDescription(writer);
            this.PrintOptions(writer);
        }

        private void PrintUsage(IndentedTextWriter writer, string methodName)
        {
            var descriptor = this.Methods.First(item => item.Name == methodName);
            this.PrintSummary(writer, descriptor);
            this.PrintSynopsis(writer, descriptor);
            this.PrintDescription(writer, descriptor);
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

        private void PrintSynopsis(IndentedTextWriter writer)
        {
            writer.WriteLine("Synopsis");
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

        private void PrintSynopsis(IndentedTextWriter writer, MethodDescriptor descriptor)
        {
            writer.WriteLine("Synopsis");
            writer.Indent++;

            var switches = this.GetSwitchesString(descriptor.Switches.Where(i => i.Required));
            var options = this.GetOptionsString(descriptor.Switches.Where(i => i.Required == false));
            writer.WriteLine("{0} {1} {2} {3}", this.Name, descriptor.Name, switches, options);

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

        private void PrintOptions(IndentedTextWriter writer, MethodDescriptor descriptor)
        {
            writer.WriteLine("Options");
            writer.Indent++;
            foreach (var item in descriptor.Switches.Where(i => i.Required == true))
            {
                this.PrintSwitch(writer, item);
            }
            foreach (var item in descriptor.Switches.Where(i => i.Required == false))
            {
                this.PrintOption(writer, item);
            }
            writer.Indent--;
            writer.WriteLine();
        }

        private void PrintSwitch(IndentedTextWriter writer, SwitchDescriptor descriptor)
        {
            if (descriptor.Description == string.Empty)
                return;

            writer.WriteLine(descriptor.DisplayName);
            writer.Indent++;
            writer.WriteMultiline(descriptor.Description);
            writer.Indent--;
            writer.WriteLine();
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

        private string GetOptionsString(IEnumerable<SwitchDescriptor> switches)
        {
            var query = from item in switches
                        let patternItems = new string[] { item.ShortNamePattern, item.NamePattern, }
                        select string.Join(" | ", patternItems.Where(i => i != string.Empty));

            return string.Join(" ", query.Select(item => "[" + item + "]"));
        }

        private string GetSwitchesString(IEnumerable<SwitchDescriptor> switches)
        {
            return string.Join(" ", switches.Select(item => "<" + item.DisplayName + ">"));
        }
    }
}
