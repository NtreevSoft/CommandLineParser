using Ntreev.Library;
using System;
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
            var parser = this.commandContext.Parsers[this.CommandName];
            var command = parser.Instance as ICommand;
            if (command.HasSubCommand == true)
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

        public bool HasSubCommand
        {
            get { return false; }
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

        [CommandSwitch(Name="sub-command", ShortName = 's')]
        [Description("사용법을 표시할 하위 명령을 설정합니다.")]
        public string SubCommandName
        {
            get; set;
        }
    }
}
