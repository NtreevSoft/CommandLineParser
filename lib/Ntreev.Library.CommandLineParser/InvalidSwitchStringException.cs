using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Library.Properties;

namespace Ntreev.Library
{
    public class InvalidSwitchStringException : SwitchException
    {
        public InvalidSwitchStringException(string switchName)
            : base(Resources.InvalidSwitchName, switchName)
        {

        }
    }
}
