using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    [Export(typeof(ShellCommandContext))]
    class ShellCommandContext : CommandContextBase
    {
        [ImportingConstructor]
        public ShellCommandContext([ImportMany]IEnumerable<ICommand> commands, [ImportMany]IEnumerable<ICommandProvider> methods)
            : base(commands, methods)
        {

        }

        protected override CommandLineParser CreateInstance(ICommand command)
        {
            return new ShellCommandLineParser(command.Name, command);
        }
    }
}
