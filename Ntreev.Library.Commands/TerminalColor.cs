using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    public class TerminalColor : IDisposable
    {
        private readonly Stack<ColorItem> stack = new Stack<ColorItem>();

        private TerminalColor()
        {

        }

        public void Dispose()
        {
            Pop();
        }

        public static TerminalColor Set(ConsoleColor foreground, ConsoleColor background)
        {
            Push(Console.ForegroundColor, Console.BackgroundColor);
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            return Default;
        }

        public static TerminalColor SetForeground(ConsoleColor color)
        {
            Push(Console.ForegroundColor, null);
            Console.ForegroundColor = color;
            return Default;
        }

        public static TerminalColor SetBackground(ConsoleColor color)
        {
            Push(null, Console.BackgroundColor);
            Console.BackgroundColor = color;
            return Default;
        }

        private static void Push(ConsoleColor? foreground, ConsoleColor? background)
        {
            if (Default.stack.Any() == true)
            {
                Default.stack.Push(new ColorItem() { foreground = foreground, background = background });
            }
            else
            {
                Default.stack.Push(new ColorItem() { foreground = null, background = null });
            }
        }

        private static void Pop()
        {
            Console.ResetColor();

            var item = Default.stack.Pop();
            if (item.foreground != null)
                Console.ForegroundColor = item.foreground.Value;
            if (item.background != null)
                Console.BackgroundColor = item.background.Value;
        }

        private static TerminalColor Default = new TerminalColor();

        private struct ColorItem
        {
            public ConsoleColor? foreground;
            public ConsoleColor? background;
        }
    }
}
