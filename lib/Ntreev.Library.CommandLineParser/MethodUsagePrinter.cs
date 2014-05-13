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
        private readonly string command;

        public MethodUsagePrinter(object target, string command)
        {
            MethodDescriptorCollection descriptors = null;
            if (target is Type)
            {
                descriptors = CommandDescriptor.GetMethodDescriptors(target as Type);
            }
            else
            {
                descriptors = CommandDescriptor.GetMethodDescriptors(target);
            }
            this.descriptors = descriptors.Where(item => item.Name != CommandLineInvoker.defaultMethod).ToArray();

            this.command = command;
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
                tw.WriteLine("{0}: {1} subcommand [args | ...]", Resources.Usage, this.command);
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

                string args = methodDescriptor.Switches.Aggregate("", (l, n) => l += "[" + n.Name + "] ", item => item);

                tw.WriteLine("{0}: {1}", Resources.Description, methodDescriptor.Description);
                tw.WriteLine("{0}: {1} {2} {3}", Resources.Usage, this.command, methodName, args);
                tw.WriteLine();


                if (methodDescriptor.Switches.Length > 0)
                {
                    tw.WriteLine("required : ");
                    tw.Indent++;
                    foreach (SwitchDescriptor item in methodDescriptor.Switches)
                    {
                        tw.WriteLine("{0} {1}", item.UsageProvider.Usage, item.UsageProvider.Description);
                    }
                    tw.Indent--;
                    tw.WriteLine();
                }

                if (methodDescriptor.OptionSwitches.Length > 0)
                {
                    tw.WriteLine("options : ");
                    tw.Indent++;
                    foreach (SwitchDescriptor item in methodDescriptor.OptionSwitches)
                    {
                        tw.WriteLine("{0} {1}", item.UsageProvider.Usage, item.UsageProvider.Description);
                    }
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
            get { return this.command; }
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
    }
}
