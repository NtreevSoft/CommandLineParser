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

        public MethodUsagePrinter(object instance, string name)
        {
            MethodDescriptorCollection descriptors = null;
            if (instance is Type)
            {
                descriptors = CommandDescriptor.GetMethodDescriptors(instance as Type);
            }
            else
            {
                descriptors = CommandDescriptor.GetMethodDescriptors(instance);
            }
            this.descriptors = descriptors.Where(item => item.Name != CommandLineInvoker.defaultMethod).ToArray();

            this.name = name;
        }

        public void PrintUsage(TextWriter textWriter)
        {
            this.PrintUsage(textWriter, 0);
        }

        public void PrintUsage(TextWriter textWriter, string methodName)
        {
            this.PrintUsage(textWriter, methodName, 0);
        }

        public virtual void PrintUsage(TextWriter textWriter, int indentLevel)
        {
            using (IndentedTextWriter tw = new IndentedTextWriter(textWriter))
            {
                tw.Indent = indentLevel;
                //tw.WriteLine("{0} {1}", this.Title, this.Version);
                tw.WriteLine("{0}: {1} subcommand [args | ...]", Resources.Usage, this.name);
                tw.WriteLine();

                tw.WriteLine("subcommands:");

                tw.Indent++;

                int maxLength = 0;


                foreach (MethodDescriptor item in this.descriptors)
                {
                    maxLength = Math.Max(item.Name.Length, maxLength);
                }

                foreach (MethodDescriptor item in this.descriptors)
                {
                    string name = item.Name;
                    name = name.PadRight(maxLength + 4);
                    tw.Write(name);
                    tw.WriteLine(item.Description);
                }
                tw.Indent--;
            }
        }

        public virtual void PrintUsage(TextWriter textWriter, string methodName, int indentLevel)
        {
            using (IndentedTextWriter tw = new IndentedTextWriter(textWriter))
            {
                tw.Indent = indentLevel;

                MethodDescriptor methodDescriptor = this.GetMethodDescriptor(methodName);

                if (methodDescriptor == null)
                {
                    tw.WriteLine("not found method : {0}", methodName);
                    return;
                }

                string args = methodDescriptor.Switches.Where(item => item.Required).Aggregate("", (l, n) => l += "[" + n.UsageProvider.Usage + "] ", item => item);

                tw.WriteLine("{0}: {1}", Resources.Description, methodDescriptor.Description);
                tw.WriteLine("{0}: {1} {2} {3}", Resources.Usage, this.name, methodName, args);
                tw.WriteLine();


                SwitchDescriptor[] requiredSwitched = methodDescriptor.Switches.Where(item => item.Required == true).ToArray();
                if (requiredSwitched.Length > 0)
                {
                    tw.WriteLine("required : ");
                    tw.Indent++;
                    string[] usages = new string[requiredSwitched.Length];
                    string[] descriptions = new string[requiredSwitched.Length];
                    for(int i=0;i< requiredSwitched.Length; i++)
                    {
                        SwitchDescriptor item = requiredSwitched[i];
                        usages[i] = item.UsageProvider.Usage;
                        descriptions[i] = item.UsageProvider.Description;
                    }
                    this.PrintUsages(tw, usages, descriptions);
                    tw.Indent--;
                    tw.WriteLine();
                }

                SwitchDescriptor[] optionSwitched = methodDescriptor.Switches.Where(item => item.Required == false).ToArray();
                if (optionSwitched.Length > 0)
                {
                    tw.WriteLine("options : ");
                    tw.Indent++;
                    string[] usages = new string[optionSwitched.Length];
                    string[] descriptions = new string[optionSwitched.Length];
                    for (int i = 0; i < optionSwitched.Length; i++)
                    {
                        SwitchDescriptor item = optionSwitched[i];
                        usages[i] = item.UsageProvider.Usage;
                        descriptions[i] = item.UsageProvider.Description;
                    }
                    this.PrintUsages(tw, usages, descriptions);
                    tw.Indent--;
                }
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
            int maxLength = 0;
            foreach(string item in usages)
            {
                maxLength = Math.Max(maxLength, item.Length);
            }
            maxLength += maxLength % 4;

            for (int i = 0; i < usages.Length; i++)
            {
                string usage = usages[i].PadRight(maxLength);
                string description = descriptions[i];
                textWriter.WriteLine("{0} {1}", usage, description);
            }
        }
    }
}
