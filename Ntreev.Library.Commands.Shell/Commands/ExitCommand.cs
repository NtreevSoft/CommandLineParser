using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell.Commands
{
    [Export(typeof(ICommand))]
    class ExitCommand : Command
    {
        [Import]
        private Lazy<IShell> shell = null;

        public ExitCommand()
            : base("exit", CommandTypes.AllowEmptyArgument)
        {

        }

        public override void Execute()
        {
            this.shell.Value.Cancel();
        }
    }
}
