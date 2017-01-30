using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell.Commands
{
    [Export(typeof(ICommand))]
    class ExitCommand : CommandBase
    {
        [Import]
        private Lazy<IShell> shell = null;

        public ExitCommand()
            : base("exit")
        {

        }

        [CommandProperty(Required = true)]
        [DefaultValue(0)]
        public int ExitCode
        {
            get; set;
        }

        protected override void OnExecute()
        {
            this.shell.Value.Cancel();
        }
    }
}
