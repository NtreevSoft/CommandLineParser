//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
