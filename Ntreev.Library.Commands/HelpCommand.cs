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
                        throw new NotFoundMethodException(string.Format("'{0}' 은(는) 존재하지 않는 명령입니다."));

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
            finally
            {
                this.CommandName = string.Empty;
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
        [DefaultValue("")]
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
