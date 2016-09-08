using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [Flags]
    public enum CommandTypes
    {
        None = 0,

        HasSubCommand = 1,

        AllowEmptyArgument = 2,
    }
}
