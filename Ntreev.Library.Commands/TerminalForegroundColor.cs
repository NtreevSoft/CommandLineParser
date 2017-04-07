using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    public class TerminalForegroundColor : IDisposable
    {
        private readonly Stack<ConsoleColor> stack = new Stack<ConsoleColor>();

        private TerminalForegroundColor()
        {

        }

        public void Dispose()
        {
            Console.ForegroundColor = this.stack.Pop();
        }

        public static TerminalForegroundColor Set(ConsoleColor color)
        {
            Default.stack.Push(Console.ForegroundColor);
            Console.ForegroundColor = color;
            return Default;
        }

        private static TerminalForegroundColor Default = new TerminalForegroundColor();
    }
}
