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
        private ConsoleColor? foregroundColor;
        private ConsoleColor? backgroundColor;

        private TerminalColor()
        {

        }

        public void Dispose()
        {
            Pop();
        }

        public static TerminalColor Set(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Push(Console.ForegroundColor, Console.BackgroundColor);
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Default.foregroundColor = foregroundColor;
            Default.backgroundColor = backgroundColor;
            ForegroundColorChanged?.Invoke(null, EventArgs.Empty);
            BackgroundColorChanged?.Invoke(null, EventArgs.Empty);
            return Default;
        }

        public static TerminalColor SetForeground(ConsoleColor color)
        {
            Push(Console.ForegroundColor, null);
            Console.ForegroundColor = color;
            Default.foregroundColor = color;
            ForegroundColorChanged?.Invoke(null, EventArgs.Empty);
            return Default;
        }

        public static TerminalColor SetBackground(ConsoleColor color)
        {
            Push(null, Console.BackgroundColor);
            Console.BackgroundColor = color;
            Default.backgroundColor = color;
            BackgroundColorChanged?.Invoke(null, EventArgs.Empty);
            return Default;
        }

        public static ConsoleColor? ForegroundColor
        {
            get
            {
                if (Default.foregroundColor.HasValue == true)
                    return Default.foregroundColor.Value;
                return null;
            }
            private set
            {

            }
        }

        public static ConsoleColor? BackgroundColor
        {
            get
            {
                if (Default.backgroundColor.HasValue == true)
                    return Default.backgroundColor.Value;
                return null;
            }
            private set
            {

            }
        }

        public static event EventHandler ForegroundColorChanged;

        public static event EventHandler BackgroundColorChanged;

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
            Default.foregroundColor = item.foreground;
            Default.backgroundColor = item.background;
            ForegroundColorChanged?.Invoke(null, EventArgs.Empty);
            BackgroundColorChanged?.Invoke(null, EventArgs.Empty);
        }

        private static TerminalColor Default = new TerminalColor();

        private struct ColorItem
        {
            public ConsoleColor? foreground;
            public ConsoleColor? background;
        }
    }
}
