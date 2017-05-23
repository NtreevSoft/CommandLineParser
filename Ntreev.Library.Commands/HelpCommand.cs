using Ntreev.Library;
using Ntreev.Library.Commands.Properties;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    if (this.commandContext.IsCommandVisible(command) == false)
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
                if (this.commandContext.IsCommandVisible(item) == false)
                    continue;
                var summary = CommandDescriptor.GetUsageDescriptionProvider(item.GetType()).GetSummary(item);

                writer.WriteLine(item.Name);
                writer.Indent++;
                writer.WriteMultiline(summary);
                writer.Indent--;
            }
            writer.Indent--;
        }
    }
}
