using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    [Export(typeof(CommandContext))]
    class ShellCommandContext : CommandContext
    {
        [ImportingConstructor]
        public ShellCommandContext([ImportMany]IEnumerable<ICommand> commands)
            : base(commands)
        {

        }

        protected override CommandLineParser CreateInstance(ICommand command)
        {
            return new ShellCommandLineParser(command.Name, command);
        }
    }
}
