using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    class ConsoleWriter : IndentedTextWriter
    {
        public ConsoleWriter()
            : base(Console.Out)
        {

        }
    }
}
