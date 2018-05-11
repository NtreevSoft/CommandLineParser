//Released under the MIT License.
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

using Ntreev.Library;
using Ntreev.Library.Commands.Properties;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    public class HelpCommand : CommandBase
    {
        private readonly CommandContextBase commandContext;

        public HelpCommand(CommandContextBase commandContext)
            : base("help")
        {
            this.commandContext = commandContext;
            this.CommandName = string.Empty;
            this.MethodName = string.Empty;
        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.Arguments.Length == 0)
            {
                return this.GetCommandNames();
            }
            else if (completionContext.Arguments.Length == 1)
            {
                return this.GetCommandMethodNames(completionContext.Arguments[0]);
            }
            return base.GetCompletions(completionContext);
        }

        [CommandProperty("CommandName", IsRequired = true)]
        [DisplayName("command")]
        [DefaultValue("")]
        public string CommandName
        {
            get; set;
        }

        [CommandProperty("sub-command", IsRequired = true)]
        [DefaultValue("")]
        public string MethodName
        {
            get; set;
        }

        protected override void OnExecute()
        {
            try
            {
                if (this.CommandName == string.Empty)
                {
                    using (var writer = new CommandTextWriter(this.commandContext.Out))
                    {
                        this.PrintList(writer);
                    }
                }
                else
                {
                    var command = this.commandContext.Commands[this.CommandName];
                    if (command == null || this.commandContext.IsCommandEnabled(command) == false || this.IsCommandUsageBrowsable(command) == false)
                        throw new CommandNotFoundException(this.CommandName);

                    var parser = this.commandContext.Parsers[command];
                    parser.Out = this.commandContext.Out;
                    this.PrintUsage(command, parser);
                }
            }
            finally
            {
                this.CommandName = string.Empty;
            }
        }

        protected virtual void PrintUsage(ICommand command, CommandLineParser parser)
        {
            if (command is IExecutable == false)
            {
                if (this.MethodName != string.Empty)
                    parser.PrintMethodUsage(this.MethodName);
                else
                    parser.PrintMethodUsage();
            }
            else
            {
                parser.PrintUsage();
            }
        }

        private void PrintList(CommandTextWriter writer)
        {
            this.commandContext.Parsers[this].PrintUsage();

            writer.WriteLine(Resources.AvaliableCommands);
            writer.Indent++;
            foreach (var item in this.commandContext.Commands)
            {
                if (this.commandContext.IsCommandEnabled(item) == false)
                    continue;
                if (this.IsCommandUsageBrowsable(item) == false)
                    continue;
                var summary = CommandDescriptor.GetUsageDescriptionProvider(item.GetType()).GetSummary(item);

                writer.WriteLine(item.Name);
                writer.Indent++;
                writer.WriteMultiline(summary);
                if (summary != string.Empty)
                    writer.WriteLine();
                writer.Indent--;
            }
            writer.Indent--;
        }

        private string[] GetCommandNames()
        {
            var query = from item in this.commandContext.Commands
                        where item.IsEnabled
                        orderby item.Name
                        select item.Name;
            return query.ToArray();
        }

        private string[] GetCommandMethodNames(string commandName)
        {
            if (this.commandContext.Commands.Contains(commandName) == false)
                return null;
            var command = this.commandContext.Commands[commandName];
            if (command is IExecutable == true)
                return null;

            var descriptors = CommandDescriptor.GetMethodDescriptors(command);
            var query = from item in descriptors
                        where this.commandContext.IsMethodEnabled(command, item)
                        orderby item.Name
                        select item.Name;
            return query.ToArray();
        }

        private bool IsCommandUsageBrowsable(ICommand command)
        {
            var attr = command.GetType().GetCustomAttribute<UsageBrowsableAttribute>();
            if (attr == null)
                return true;
            return attr.IsBrowsable;
        }
    }
}
