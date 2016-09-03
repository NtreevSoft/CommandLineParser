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
        private readonly MethodDescriptor[] descriptors;
        private readonly string name;

        public MethodUsagePrinter(string name, object instance)
        {
            this.name = name;
            var descriptors = CommandDescriptor.GetMethodDescriptors(instance);
            this.descriptors = descriptors.Where(item => item.Name != CommandLineInvoker.defaultMethod).ToArray();
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

        private void PrintUsage(IndentedTextWriter textWriter)
        {
            textWriter.WriteLine("{0}: {1} command [args | ...]", Resources.Usage, this.name);
            textWriter.WriteLine();

            textWriter.WriteLine("commands:");
            textWriter.Indent++;

            var usages = new string[this.descriptors.Length];
            var descriptions = new string[this.descriptors.Length];
            for (var i = 0; i < this.descriptors.Length; i++)
            {
                var item = this.descriptors[i];
                usages[i] = item.Name;
                descriptions[i] = item.Description;
            }
            this.PrintUsages(textWriter, usages, descriptions);

            textWriter.Indent--;
        }

        private void PrintUsage(IndentedTextWriter textWriter, string methodName)
        {
            var methodDescriptor = this.GetMethodDescriptor(methodName);

            if (methodDescriptor == null)
            {
                textWriter.WriteLine("not found method : {0}", methodName);
                return;
            }

            var args = methodDescriptor.Switches.Where(item => item.Required).Aggregate("", (l, n) => l += "[" + n.UsageProvider.Usage + "] ", item => item);

            textWriter.WriteLine("{0}: {1}", Resources.Description, methodDescriptor.Description);
            textWriter.WriteLine("{0}: {1} {2} {3}", Resources.Usage, this.name, methodName, args);
            textWriter.WriteLine();

            var requiredSwitched = methodDescriptor.Switches.Where(item => item.Required == true).ToArray();
            if (requiredSwitched.Length > 0)
            {
                textWriter.WriteLine("required : ");
                textWriter.Indent++;
                var usages = new string[requiredSwitched.Length];
                var descriptions = new string[requiredSwitched.Length];
                for (var i = 0; i < requiredSwitched.Length; i++)
                {
                    var item = requiredSwitched[i];
                    usages[i] = item.UsageProvider.Usage;
                    descriptions[i] = item.UsageProvider.Description;
                }
                this.PrintUsages(textWriter, usages, descriptions);
                textWriter.Indent--;
                textWriter.WriteLine();
            }

            var optionSwitched = methodDescriptor.Switches.Where(item => item.Required == false).ToArray();
            if (optionSwitched.Length > 0)
            {
                textWriter.WriteLine("options : ");
                textWriter.Indent++;
                var usages = new string[optionSwitched.Length];
                var descriptions = new string[optionSwitched.Length];
                for (var i = 0; i < optionSwitched.Length; i++)
                {
                    var item = optionSwitched[i];
                    usages[i] = item.UsageProvider.Usage;
                    descriptions[i] = item.UsageProvider.Description;
                }
                this.PrintUsages(textWriter, usages, descriptions);
                textWriter.Indent--;
            }
        }

        protected MethodDescriptor[] MethodDescriptors
        {
            get { return this.descriptors; }
        }

        protected string Command
        {
            get { return this.name; }
        }

        private MethodDescriptor GetMethodDescriptor(string methodName)
        {
            var query = from item in this.descriptors
                        where string.Compare(item.Name, methodName, true) == 0
                        select item;

            if (query.Count() == 0)
                return null;
            return query.First();
        }

        private void PrintUsages(TextWriter textWriter, string[] usages, string[] descriptions)
        {
            var maxLength = 0;
            foreach(var item in usages)
            {
                maxLength = Math.Max(maxLength, item.Length);
            }
            maxLength += maxLength % 4;

            for (var i = 0; i < usages.Length; i++)
            {
                var usage = usages[i].PadRight(maxLength);
                var description = descriptions[i];
                textWriter.WriteLine("{0} {1}", usage, description);
            }
        }
    }
}
