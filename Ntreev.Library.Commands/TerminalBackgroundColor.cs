using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    public class TerminalBackgroundColor : IDisposable
    {
        private readonly Stack<ConsoleColor> stack = new Stack<ConsoleColor>();

        private TerminalBackgroundColor()
        {

        }

        public void Dispose()
        {
            Console.BackgroundColor = this.stack.Pop();
        }

        public static TerminalBackgroundColor Set(ConsoleColor color)
        {
            Default.stack.Push(Console.BackgroundColor);
            Console.BackgroundColor = color;
            return Default;
        }

        private static TerminalBackgroundColor Default = new TerminalBackgroundColor();
    }
}
