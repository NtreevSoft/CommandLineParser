using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Invoke
{
    [ConsoleModeOnly]
    static class StaticCommand
    {
        [CommandMethod]
        [CommandMethodProperty("Value")]
        public static void List()
        {
            Console.WriteLine("list invoked.");
        }

        [CommandProperty]
        public static int Value
        {
            get; set;
        }
    }
}
