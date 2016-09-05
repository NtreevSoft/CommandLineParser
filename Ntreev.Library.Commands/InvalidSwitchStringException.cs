using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    public class InvalidSwitchStringException : SwitchException
    {
        public InvalidSwitchStringException(string switchName)
            : base(Resources.InvalidSwitchName, switchName)
        {

        }
    }
}
