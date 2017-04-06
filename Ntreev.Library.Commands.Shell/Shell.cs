using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    [Export(typeof(IShell))]
    class Shell : CommandContextTerminal, IShell
    {
        [ImportingConstructor]
        public Shell(CommandContextBase commandContext)
           : base(commandContext)
        {
            this.Prompt = "shell";
        }
    }
}