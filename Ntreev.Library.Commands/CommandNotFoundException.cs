using Ntreev.Library.Commands.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException(string methodName)
            : base(string.Format(Resources.MethodDoesNotExist_Format, methodName))
        {

        }
    }
}
