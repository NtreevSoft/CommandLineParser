using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using Ntreev.Library.Properties;

namespace Ntreev.Library
{
    class MethodUsagePrinter : IUsagePrinter
    {
        private readonly Type type;
        private readonly string command;

        public MethodUsagePrinter(Type type, string command)
        {
            this.type = type;
            this.command = command;
        }

        public void PrintUsage(TextWriter textWriter)
        {
            this.PrintUsage(textWriter, 0);
        }

        public void PrintUsage(TextWriter textWriter, int indentLevel)
        {
            using (IndentedTextWriter tw = new IndentedTextWriter(textWriter))
            {
                tw.Indent = indentLevel;
                MethodDescriptorCollection descriptors = CommandDescriptor.GetMethodDescriptors(this.type);

                tw.WriteLine("{0}: {1} subcommand [args | ...]", Resources.Usage, this.command);
                tw.WriteLine();

                tw.WriteLine("sub commands:");

                tw.Indent++;

                int maxLength = 0;
                foreach (MethodDescriptor item in descriptors)
                {
                    maxLength = Math.Max(item.Name.Length, maxLength);
                }

                foreach (MethodDescriptor item in descriptors)
                {
                    string name = item.Name;
                    name = name.PadRight(maxLength + 4);
                    tw.Write(name);
                    tw.WriteLine(item.Description);
                }
                tw.Indent--;
            }
        }

        public void PrintUsage(TextWriter textWriter, string memberName)
        {
            this.PrintUsage(textWriter, memberName, 0);
        }

        public void PrintUsage(TextWriter textWriter, string memberName, int indentLevel)
        {
            using (IndentedTextWriter tw = new IndentedTextWriter(textWriter))
            {
                tw.Indent = indentLevel;
                MethodDescriptor methodDescriptor = CommandDescriptor.GetMethodDescriptors(this.type)[memberName];

                if (methodDescriptor == null)
                {
                    tw.WriteLine("not found method : {0}", memberName);
                    return;
                }

                tw.WriteLine("{0}: {1} {2} [args | ...]", Resources.Usage, this.command, memberName);
                tw.WriteLine("subcommand: {0}", methodDescriptor.UsageProvider.Usage);

                tw.Indent++;
                tw.WriteLine("args : ");

                tw.Indent++;
                foreach (SwitchDescriptor item in methodDescriptor.Switches)
                {
                    tw.WriteLine("{0} {1}", item.UsageProvider.Usage, item.UsageProvider.Description);
                }
                tw.Indent--;

                tw.Indent--;
            }
        }


        protected Type Type
        {
            get { return this.type; }
        }
    }
}
