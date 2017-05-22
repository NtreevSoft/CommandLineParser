using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public sealed class TerminalCancelEventArgs
    {
        private readonly ConsoleSpecialKey specialKey;

        internal TerminalCancelEventArgs(ConsoleSpecialKey specialKey)
        {
            this.specialKey = specialKey;
        }

        public bool Cancel { get; set; }

        public ConsoleSpecialKey SpecialKey
        {
            get { return this.specialKey; }
        }
    }
}
