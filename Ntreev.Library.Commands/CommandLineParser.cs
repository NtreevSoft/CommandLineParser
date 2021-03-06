﻿//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
    public class CommandLineParser
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
            return this.Parse(commandLine, CommandParsingTypes.None);
        }

        public bool Parse(string commandLine, CommandParsingTypes types)
        {
            if ((types & CommandParsingTypes.OmitCommandName) == CommandParsingTypes.OmitCommandName)
            {
                commandLine = $"{this.Name} {commandLine}";
            }

            var arguments = CommandStringUtility.Split(commandLine);
            var name = arguments[0];

            if (File.Exists(name) == true)
                name = Process.GetCurrentProcess().ProcessName;
            if (this.name != name)
                throw new ArgumentException(string.Format(Resources.InvalidCommandName_Format, name));

            var items = CommandStringUtility.Split(arguments[1]);

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
                var descriptors = CommandDescriptor.GetMemberDescriptors(this.instance).Where(item => this.IsMemberEnabled(item));
                var omitInitialize = (types & CommandParsingTypes.OmitInitialize) == CommandParsingTypes.OmitInitialize;
                var parser = new ParseDescriptor(typeof(CommandPropertyDescriptor), descriptors, arguments[1], omitInitialize == false);
                parser.SetValue(this.instance);
                return true;
            }
        }

        public bool Invoke(string commandLine)
        {
            return this.Invoke(commandLine, CommandParsingTypes.None);
        }

        public bool Invoke(string commandLine, CommandParsingTypes types)
        {
            if ((types & CommandParsingTypes.OmitCommandName) == CommandParsingTypes.OmitCommandName)
            {
                commandLine = $"{this.Name} {commandLine}";
            }

            var arguments = CommandStringUtility.Split(commandLine);
            var name = arguments[0];

            if (File.Exists(name) == true)
                name = Process.GetCurrentProcess().ProcessName;
            if (this.name != name)
                throw new ArgumentException(string.Format(Resources.InvalidCommandName_Format, name));

            var arguments1 = CommandStringUtility.Split(arguments[1]);
            var method = arguments1[0];

            if (string.IsNullOrEmpty(method) == true)
            {
                this.PrintSummary();
                return false;
            }
            else if (method == this.HelpName)
            {
                var items = CommandStringUtility.Split(arguments1[1]);
                if (arguments1[1] == string.Empty)
                    this.PrintMethodUsage();
                else if (items[1] == string.Empty)
                    this.PrintMethodUsage(arguments1[1]);
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
                if (descriptor == null || this.IsMethodEnabled(descriptor) == false)
                    throw new CommandNotFoundException(method);
                var enabledDescriptors = descriptor.Members.Where(item => this.IsMemberEnabled(item));
                var omitInitialize = (types & CommandParsingTypes.OmitInitialize) == CommandParsingTypes.OmitInitialize;
                descriptor.Invoke(this.instance, arguments1[1], enabledDescriptors, omitInitialize == false);
                return true;
            }
        }

        public virtual void PrintSummary()
        {
            if (this.CommandContext != null)
                this.Out.WriteLine("Type '{0} {1}' for usage.", this.CommandContext.HelpCommand.Name, this.name);
            else
                this.Out.WriteLine("Type '{0} {1}' for usage.", this.name, this.HelpName);
        }

        public virtual void PrintUsage()
        {
            var enabledDescriptors = CommandDescriptor.GetMemberDescriptors(this.instance).Where(item => this.IsMemberEnabled(item));
            this.MemberUsagePrinter.Print(this.Out, enabledDescriptors.ToArray());
        }

        public virtual void PrintUsage(string memberName)
        {
            var descriptor = CommandDescriptor.GetMemberDescriptors(this.instance)
                                              .Where(item => this.IsMemberEnabled(item))
                                              .FirstOrDefault(item => (item.IsRequired == true && memberName == item.Name) ||
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
            var descriptors = CommandDescriptor.GetMethodDescriptors(this.instance).Where(item => this.IsMethodEnabled(item));
            this.MethodUsagePrinter.Print(this.Out, descriptors.ToArray());
        }

        public virtual void PrintMethodUsage(string methodName)
        {
            var descriptors = CommandDescriptor.GetMethodDescriptors(this.instance);
            var descriptor = descriptors.FirstOrDefault(item => item.Name == methodName);
            if (descriptor == null || this.IsMethodEnabled(descriptor) == false)
                throw new CommandNotFoundException(methodName);

            var enabledDescriptors = descriptor.Members.Where(item => this.IsMemberEnabled(item)).ToArray();

            this.MethodUsagePrinter.Print(this.Out, descriptor, enabledDescriptors);
        }

        public virtual void PrintMethodUsage(string methodName, string memberName)
        {
            var descriptors = CommandDescriptor.GetMethodDescriptors(this.instance);
            var descriptor = descriptors.FirstOrDefault(item => item.Name == methodName);
            if (descriptor == null || this.IsMethodEnabled(descriptor) == false)
                throw new CommandNotFoundException(methodName);

            var visibleDescriptor = descriptor.Members.Where(item => this.IsMemberEnabled(item))
                                                      .FirstOrDefault(item => (item.IsRequired == true && memberName == item.Name) ||
                                                                               memberName == item.NamePattern ||
                                                                               memberName == item.ShortNamePattern);

            if (visibleDescriptor == null)
                throw new InvalidOperationException(string.Format(Resources.MemberDoesNotExist_Format, memberName));

            this.MethodUsagePrinter.Print(this.Out, descriptor, visibleDescriptor);
        }

        [Obsolete("use CommandStringUtility")]
        public static string[] Split(string text)
        {
            return CommandStringUtility.Split(text);
        }

        [Obsolete("use CommandStringUtility")]
        public static string[] SplitAll(string text)
        {
            return CommandStringUtility.SplitAll(text);
        }

        [Obsolete("use CommandStringUtility")]
        public static string[] SplitAll(string text, bool removeQuot)
        {
            return CommandStringUtility.SplitAll(text, removeQuot);
        }

        [Obsolete("use CommandStringUtility")]
        public static Match[] MatchAll(string text)
        {
            return CommandStringUtility.MatchAll(text);
        }

        [Obsolete("use CommandStringUtility")]
        public static Match[] MatchCompletion(string text)
        {
            return CommandStringUtility.MatchCompletion(text);
        }

        [Obsolete("use CommandStringUtility")]
        public static string RemoveQuot(string text)
        {
            return CommandStringUtility.TrimQuot(text);
        }

        [Obsolete("use CommandStringUtility")]
        public static bool IsSwitch(string argument)
        {
            return CommandStringUtility.IsSwitch(argument);
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
                    return new Version(FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion);
                }
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }

        protected virtual bool IsMethodEnabled(CommandMethodDescriptor descriptor)
        {
            if (descriptor.Attributes.FirstOrDefault(item => item is BrowsableAttribute) is BrowsableAttribute attr && attr.Browsable == false)
                return false;
            if (this.CommandContext != null)
                return this.CommandContext.IsMethodEnabled(this.Instance as ICommand, descriptor);
            return true;
        }

        protected virtual bool IsMemberEnabled(CommandMemberDescriptor descriptor)
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

        private static void Invoke(object instance, string arguments, MethodInfo methodInfo, IEnumerable<CommandMemberDescriptor> descriptors, bool init)
        {
            var parser = new ParseDescriptor(typeof(CommandParameterDescriptor), descriptors, arguments, init);
            parser.SetValue(instance);

            var values = new ArrayList();
            var nameToDescriptors = descriptors.ToDictionary(item => item.DescriptorName);

            foreach (var item in methodInfo.GetParameters())
            {
                var descriptor = nameToDescriptors[item.Name];
                var value = descriptor.GetValueInternal(instance);
                values.Add(value);
            }

            if (methodInfo.DeclaringType.IsAbstract && methodInfo.DeclaringType.IsSealed == true)
                methodInfo.Invoke(null, values.ToArray());
            else
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

        internal bool IsMethodVisible(string methodName)
        {
            var descriptor = CommandDescriptor.GetMethodDescriptor(this.instance, methodName);
            if (descriptor == null || this.IsMethodEnabled(descriptor) == false)
                return false;
            return true;
        }

        internal CommandContextBase CommandContext
        {
            get; set;
        }
    }
}