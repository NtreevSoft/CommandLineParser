using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Invoke
{
    static class StaticCommand
    {
        [CommandMethod]
        [CommandMethodSwitch("Value")]
        public static void List()
        {

        }

        [CommandSwitch]
        public static int Value
        {
            get; set;
        }
    }
}
