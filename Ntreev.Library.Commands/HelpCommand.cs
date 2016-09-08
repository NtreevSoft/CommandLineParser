using Ntreev.Library;
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
    [Summary("사용할 수 있는 명령을 나열하고 사용법을 알려줍니다.")]
    [SRDescription("HelpDescription")]
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
                using (var writer = new IndentedTextWriter(this.commandContext.Out))
                {
                    this.PrintList(writer);
                }
            }
            else
            {
                var parser = this.commandContext.Parsers[this.CommandName];
                var command = parser.Instance as ICommand;
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
        [Description("사용법을 표시할 명령의 이름을 나타냅니다. 사용 가능한 명령의 목록은 AvaliableCommands에 표시됩니다.")]
        public string CommandName
        {
            get; set;
        }

        [CommandSwitch(Name = "sub-command", ShortName = 's')]
        [Description("사용법을 표시할 하위 명령을 설정합니다.")]
        public string SubCommandName
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'd', NameType = SwitchNameTypes.ShortName)]
        [Description("사용법을 표시할 하위 명령을 설정합니다.")]
        public bool Detail
        {
            get; set;
        }

        private void PrintList(IndentedTextWriter writer)
        {
            this.commandContext.Parsers[this.Name].PrintUsage();

            writer.WriteLine("AvaliableCommands");
            writer.Indent++;
            foreach (var item in this.commandContext.Parsers)
            {
                var instance = item.Value.Instance;
                var summary = instance.GetType().GetSummary();
                if (item.Key == this.Name)
                    continue;
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
