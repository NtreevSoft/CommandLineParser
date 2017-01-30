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
        [CommandMethodProperty("Value")]
        public static void List()
        {

        }

        [CommandProperty]
        public static int Value
        {
            get; set;
        }
    }
}
