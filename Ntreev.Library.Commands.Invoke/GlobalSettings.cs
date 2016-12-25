using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Invoke
{
    static class GlobalSettings
    {
        [CommandSwitch]
        public static string ID
        {
            get; set;
        }

        [CommandSwitch]
        public static string Password
        {
            get; set;
        }
    }
}
