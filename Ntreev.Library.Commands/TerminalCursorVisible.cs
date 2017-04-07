using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    public class TerminalCursorVisible : IDisposable
    {
        private readonly Stack<bool> stack = new Stack<bool>();

        private TerminalCursorVisible()
        {

        }

        public void Dispose()
        {
            Console.CursorVisible = this.stack.Pop();
        }

        public static TerminalCursorVisible Set(bool value)
        {
            Default.stack.Push(Console.CursorVisible);
            Console.CursorVisible = value;
            return Default;
        }

        private static TerminalCursorVisible Default = new TerminalCursorVisible();
    }
}
