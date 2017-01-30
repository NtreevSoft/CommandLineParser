#region License
//Ntreev CommandLineParser for .Net 1.0.4548.25168
//https://github.com/NtreevSoft/CommandLineParser

//Released under the MIT License.

//Copyright (c) 2010 Ntreev Soft co., Ltd.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using Ntreev.Library.Commands.Properties;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace Ntreev.Library.Commands
{
    public partial class CommandLineParser
    {
        private readonly string name;
        private readonly object instance;
        private Version version;
        private TextWriter writer;
        private CommandMemberUsagePrinter commandUsagePrinter;
        private CommandMethodUsagePrinter methodUsagePrinter;

        public CommandLineParser(object instance)
            : this(string.Empty, instance)
        {

        }

        public CommandLineParser(string name, object instance)
        {
            this.HelpName = "help";
            this.VersionName = "--version";
            this.instance = instance;
            this.name = string.IsNullOrEmpty(name) == true ? Process.GetCurrentProcess().ProcessName : name;
            this.Out = Console.Out;
        }

        public bool Parse(string commandLine)
        {
            var match = Regex.Match(commandLine, @"^((""[^""]*"")|(\S+))");
            var name = match.Value.Trim(new char[] { '\"', });

            if (File.Exists(name) == true)
                name = Process.GetCurrentProcess().ProcessName;

            if (this.name != name)
                throw new ArgumentException(string.Format(Resources.InvalidCommandName_Format, name));

            var arguments = commandLine.Substring(match.Length).Trim();
            var items = CommandLineParser.Split(arguments);

            if (items[0] == this.HelpName)
            {
                if (items[1] == string.Empty)
                    this.PrintUsage();
                else
                    this.PrintUsage(items[1]);
                return false;
            }
            else if (items[0] == this.VersionName)
            {
                this.PrintVersion();
                return false;
            }
            else
            {
                var descriptors = CommandDescriptor.GetMemberDescriptors(this.instance).Where(item => this.IsMemberVisible(item));
                var helper = new ParseDescriptor(descriptors);
                helper.Parse(this.instance, arguments);
                return true;
            }
        }

        public bool Invoke(string commandLine)
        {
            var cmdLine = commandLine;

            var regex = new Regex(@"^((""[^""]*"")|(\S+))");
            var match = regex.Match(cmdLine);
            var name = match.Value.Trim(new char[] { '\"', });

            if (File.Exists(name) == true)
                name = Process.GetCurrentProcess().ProcessName;

            if (this.name != name)
                throw new ArgumentException(string.Format(Resources.InvalidCommandName_Format, name));

            cmdLine = cmdLine.Substring(match.Length).Trim();
            match = regex.Match(cmdLine);
            var method = match.Value;

            var arguments = cmdLine.Substring(match.Length).Trim();
            var items = CommandLineParser.Split(arguments);

            if (string.IsNullOrEmpty(method) == true)
            {
                this.PrintSummary();
                return false;
            }
            else if (method == this.HelpName)
            {
                if (arguments == string.Empty)
                    this.PrintMethodUsage();
                else if(items[1] == string.Empty)
                    this.PrintMethodUsage(arguments);
                else 
                    this.PrintMethodUsage(items[0], items[1]);
                return false;
            }
            else if (method == this.VersionName)
            {
                this.PrintVersion();
                return false;
            }
            else
            {
                var descriptor = CommandDescriptor.GetMethodDescriptor(this.instance, method);

                if (descriptor == null || this.IsMethodVisible(descriptor) == false)
                    throw new CommandNotFoundException(method);

                var visibleDescriptors = descriptor.Members.Where(item => this.IsMemberVisible(item));

                Invoke(this.instance, arguments, descriptor.MethodInfo, visibleDescriptors);
                return true;
            }
        }

        public virtual void PrintSummary()
        {
            this.Out.WriteLine("Type '{0} {1}' for usage.", this.name, this.HelpName);
        }

        public virtual void PrintUsage()
        {
            var visibleDescriptors = CommandDescriptor.GetMemberDescriptors(this.instance).Where(item => this.IsMemberVisible(item));
            this.MemberUsagePrinter.Print(this.Out, visibleDescriptors.ToArray());
        }

        public virtual void PrintUsage(string memberName)
        {
            var descriptor = CommandDescriptor.GetMemberDescriptors(this.instance)
                                              .Where(item => this.IsMemberVisible(item))
                                              .FirstOrDefault(item => (item.Required == true && memberName == item.Name) || 
                                                                       memberName == item.NamePattern || 
                                                                       memberName == item.ShortNamePattern);
            if (descriptor == null)
                throw new InvalidOperationException(string.Format(Resources.MemberDoesNotExist_Format, memberName));
            this.MemberUsagePrinter.Print(this.Out, descriptor);
        }

        public virtual void PrintVersion()
        {
            var info = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            this.Out.WriteLine("{0} {1}", this.Name, this.Version);
            this.Out.WriteLine(info.LegalCopyright);
        }

        public virtual void PrintMethodUsage()
        {
            var descriptors = CommandDescriptor.GetMethodDescriptors(this.instance).Where(item => this.IsMethodVisible(item));
            this.MethodUsagePrinter.Print(this.Out, descriptors.ToArray());
        }

        public virtual void PrintMethodUsage(string methodName)
        {
            var descriptors = CommandDescriptor.GetMethodDescriptors(this.instance);
            var descriptor = descriptors.FirstOrDefault(item => item.Name == methodName);
            if (descriptor == null || this.IsMethodVisible(descriptor) == false)
                throw new CommandNotFoundException(methodName);

            var visibleDescriptros = descriptor.Members.Where(item => this.IsMemberVisible(item)).ToArray();

            this.MethodUsagePrinter.Print(this.Out, descriptor, visibleDescriptros);
        }

        public virtual void PrintMethodUsage(string methodName, string memberName)
        {
            var descriptors = CommandDescriptor.GetMethodDescriptors(this.instance);
            var descriptor = descriptors.FirstOrDefault(item => item.Name == methodName);
            if (descriptor == null || this.IsMethodVisible(descriptor) == false)
                throw new CommandNotFoundException(methodName);

            var visibleDescriptor = descriptor.Members.Where(item => this.IsMemberVisible(item))
                                                      .FirstOrDefault(item => (item.Required == true && memberName == item.Name) ||
                                                                               memberName == item.NamePattern ||
                                                                               memberName == item.ShortNamePattern);

            if (visibleDescriptor == null)
                throw new InvalidOperationException(string.Format(Resources.MemberDoesNotExist_Format, memberName));

            this.MethodUsagePrinter.Print(this.Out, descriptor, visibleDescriptor);
        }

        public static string[] Split(string commandLine)
        {
            var match = Regex.Match(commandLine, @"^((""[^""]*"")|(\S+))");
            var name = match.Value.Trim(new char[] { '\"', });

            if (File.Exists(name) == true)
                name = Path.GetFileNameWithoutExtension(name);

            var arguments = commandLine.Substring(match.Length).Trim();

            return new string[] { name, arguments, };
        }

        internal static Queue<string> SplitAll(string commandLine)
        {
            var pattern = @"^((""[^""]*"")|(\S+))";
            var match = Regex.Match(commandLine, pattern);
            var argList = new Queue<string>();

            while (match.Success)
            {
                commandLine = commandLine.Substring(match.Length).Trim();
                argList.Enqueue(match.Value);
                match = Regex.Match(commandLine, pattern);
            }

            return argList;
        }

        public TextWriter Out
        {
            get { return this.writer ?? Console.Out; }
            set { this.writer = value; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public object Instance
        {
            get { return this.instance; }
        }

        public string HelpName
        {
            get; set;
        }

        public string VersionName
        {
            get; set;
        }

        public Version Version
        {
            get
            {
                if (this.version == null)
                {
                    return new Version(FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion);
                }
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }

        protected virtual bool IsMethodVisible(CommandMethodDescriptor descriptor)
        {
            var attr = descriptor.Attributes.FirstOrDefault(item => item is BrowsableAttribute) as BrowsableAttribute;
            if (attr == null)
                return true;
            return attr.Browsable;
        }

        protected virtual bool IsMemberVisible(CommandMemberDescriptor descriptor)
        {
            var attr = descriptor.Attributes.FirstOrDefault(item => item is BrowsableAttribute) as BrowsableAttribute;
            if (attr == null)
                return true;
            return attr.Browsable;
        }

        protected virtual CommandMemberUsagePrinter CreateMemberUsagePrinter(string name, object instance)
        {
            return new CommandMemberUsagePrinter(name, instance);
        }

        protected virtual CommandMethodUsagePrinter CreateMethodUsagePrinter(string name, object instance)
        {
            return new CommandMethodUsagePrinter(name, instance);
        }

        private static void Invoke(object instance, string arguments, MethodInfo methodInfo, IEnumerable<CommandMemberDescriptor> descriptors)
        {
            var helper = new ParseDescriptor(descriptors);
            helper.Parse(instance, arguments);

            var values = new ArrayList();
            var nameToDescriptors = descriptors.ToDictionary(item => item.DescriptorName);

            foreach (var item in methodInfo.GetParameters())
            {
                var descriptor = nameToDescriptors[item.Name];

                var value = descriptor.GetValue(instance);
                values.Add(value);
            }

            methodInfo.Invoke(instance, values.ToArray());
        }

        private CommandMemberUsagePrinter MemberUsagePrinter
        {
            get
            {
                if (this.commandUsagePrinter == null)
                    this.commandUsagePrinter = this.CreateMemberUsagePrinter(this.name, this.instance);
                return this.commandUsagePrinter;
            }
        }

        private CommandMethodUsagePrinter MethodUsagePrinter
        {
            get
            {
                if (this.methodUsagePrinter == null)
                    this.methodUsagePrinter = this.CreateMethodUsagePrinter(this.name, this.instance);
                return this.methodUsagePrinter;
            }
        }
    }
}