using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using Ntreev.Library.Properties;

namespace Ntreev.Library
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
            this.descriptors = CommandDescriptor.GetMethodDescriptors(instance).ToArray();
        }

        public virtual void PrintUsage(TextWriter textWriter)
        {
            using (var tw = new IndentedTextWriter(textWriter))
            {
                this.PrintUsage(tw);
            }
        }

        public virtual void PrintUsage(TextWriter textWriter, string methodName)
        {
            using (var tw = new IndentedTextWriter(textWriter))
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

        private void PrintSummary(IndentedTextWriter textWriter)
        {
            var summary = this.Instance.GetType().GetSummary();
            textWriter.WriteLine("Name");
            textWriter.Indent++;
            textWriter.WriteLine("git-{0} - {1}", this.Name, summary);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintSummary(IndentedTextWriter textWriter, MethodDescriptor descriptor)
        {
            var summary = descriptor.Summary;
            textWriter.WriteLine("Name");
            textWriter.Indent++;
            textWriter.WriteLine("git {0} {1} - {2}", this.Name, descriptor.Name, summary);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintUsage(IndentedTextWriter textWriter)
        {
            this.PrintSummary(textWriter);
            this.PrintSynopsis(textWriter);
            this.PrintDescription(textWriter);
            this.PrintOptions(textWriter);
        }

        private void PrintUsage(IndentedTextWriter textWriter, string methodName)
        {
            var descriptor = this.Methods.First(item => item.Name == methodName);
            this.PrintSummary(textWriter, descriptor);
            this.PrintSynopsis(textWriter, descriptor);
            this.PrintDescription(textWriter, descriptor);
            this.PrintOptions(textWriter, descriptor);
        }

        private void PrintDescription(IndentedTextWriter textWriter)
        {
            textWriter.WriteLine("Description");
            textWriter.Indent++;
            textWriter.WriteMultiline(this.Description);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintDescription(IndentedTextWriter textWriter, MethodDescriptor descriptor)
        {
            textWriter.WriteLine("Description");
            textWriter.Indent++;
            textWriter.WriteMultiline(descriptor.Description);
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintSynopsis(IndentedTextWriter textWriter)
        {
            textWriter.WriteLine("Synopsis");
            textWriter.Indent++;

            foreach (var item in this.Methods)
            {
                var switches = this.GetSwitchesString(item.Switches.Where(i => i.Required));
                var options = this.GetOptionsString(item.Switches.Where(i => i.Required == false));
                textWriter.WriteLine("{0} {1} {2} {3}", this.Name, item.Name, switches, options);
            }

            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintSynopsis(IndentedTextWriter textWriter, MethodDescriptor descriptor)
        {
            textWriter.WriteLine("Synopsis");
            textWriter.Indent++;

            var switches = this.GetSwitchesString(descriptor.Switches.Where(i => i.Required));
            var options = this.GetOptionsString(descriptor.Switches.Where(i => i.Required == false));
            textWriter.WriteLine("{0} {1} {2} {3}", this.Name, descriptor.Name, switches, options);

            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintOptions(IndentedTextWriter textWriter)
        {
            textWriter.WriteLine("Options");
            textWriter.Indent++;
            foreach (var item in this.Methods)
            {
                var switches = this.GetSwitchesString(item.Switches.Where(i => i.Required));
                var options = this.GetOptionsString(item.Switches.Where(i => i.Required == false));
                textWriter.WriteLine("{0} {1} {2} {3}", this.Name, item.Name, switches, options);

                textWriter.Indent++;
                textWriter.WriteMultiline(item.Description);
                textWriter.Indent--;
                textWriter.WriteLine();
            }
            textWriter.Indent--;
            textWriter.WriteLine();
        }

        private void PrintOptions(IndentedTextWriter textWriter, MethodDescriptor descriptor)
        {
            textWriter.WriteLine("Options");
            textWriter.Indent++;
            foreach (var item in descriptor.Switches.Where(i => i.Required == true))
            {
                this.PrintSwitch(textWriter, item);
            }
            foreach (var item in descriptor.Switches.Where(i => i.Required == false))
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
