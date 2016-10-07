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
    class HelpCommand : ICommand
    {
        private readonly CommandContext commandContext;

        public HelpCommand(CommandContext commandContext)
        {
            this.commandContext = commandContext;
            this.CommandName = string.Empty;
            this.SubCommandName = string.Empty;
        }

        public void Execute()
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
                var parser = this.commandContext.Parsers[command];
                parser.Out = this.commandContext.Out;

                if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true)
                {
                    if (this.SubCommandName != string.Empty)
                        parser.PrintMethodUsage(this.SubCommandName);
                    else
                        parser.PrintMethodUsage();
                }
                else
                {
                    parser.PrintUsage();
                }
            }
        }

        public CommandTypes Types
        {
            get { return CommandTypes.AllowEmptyArgument; }
        }

        public string Name
        {
            get { return "help"; }
        }

        [CommandSwitch(Name = "CommandName", Required = true)]
        [DisplayName("command")]
        public string CommandName
        {
            get; set;
        }

        [CommandSwitch(Name = "sub-command", Required = true)]
        [DefaultValue(null)]
        public string SubCommandName
        {
            get; set;
        }

        private void PrintList(CommandTextWriter writer)
        {
            this.commandContext.Parsers[this].PrintUsage();

            writer.WriteLine(Resources.AvaliableCommands);
            writer.Indent++;
            foreach (var item in this.commandContext.Commands)
            {
                var command = item.Value;
                var summary = CommandDescriptor.GetUsageDescriptionProvider(command.GetType()).GetSummary(command);

                writer.WriteLine(item.Key);
                writer.Indent++;
                writer.WriteMultiline(summary);
                writer.Indent--;
            }
            writer.Indent--;
            writer.WriteLine();
        }
    }
}
