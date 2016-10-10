using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.Commands
{
    [Export(typeof(ICommand))]
    class ExitCommand : ICommand
    {
        [Import]
        private Lazy<IShell> shell = null;

        public void Execute()
        {
            this.shell.Value.Cancel();
        }

        public string Name
        {
            get { return "exit"; }
        }

        public CommandTypes Types
        {
            get { return CommandTypes.None | CommandTypes.AllowEmptyArgument; }
        }
    }
}
