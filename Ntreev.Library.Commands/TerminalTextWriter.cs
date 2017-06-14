using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    class TerminalTextWriter : TextWriter
    {
        private readonly TextWriter writer;
        private readonly Terminal terminal;
        private Encoding encoding;
        //private int x;
        //private int y;
        private int offsetY;
        private int length;

        public TerminalTextWriter(TextWriter writer, Terminal terminal, Encoding encoding)
        {
            this.writer = writer;
            this.terminal = terminal;
            this.encoding = encoding;
            //this.x = Console.CursorLeft;
            //this.y = Console.CursorTop;
        }

        public override Encoding Encoding
        {
            get { return this.encoding; }
        }

        public override void Write(char value)
        {
            lock (Terminal.LockedObject)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    this.WriteToStream(value.ToString());
                }
            }
        }

        public override void Write(string value)
        {
            lock (Terminal.LockedObject)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    this.WriteToStream(value);
                }
            }
        }

        public override void WriteLine(string value)
        {
            lock (Terminal.LockedObject)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    this.WriteToStream(value + Environment.NewLine);
                }
            }
        }
        private void WriteToStream(string text)
        {
            this.terminal.Erase();
            Console.SetCursorPosition(this.length % Console.BufferWidth, this.terminal.Top + this.offsetY);
            //if (this.offsetY == 0)
            //{
            //    this.terminal.Top++;
            //    this.offsetY = -1;
            //}

            var x1 = Console.CursorLeft;
            var y1 = Console.CursorTop;

            var ss = text.Split('\n');
            for (var i = 0; i < ss.Length; i++)
            {
                if (i != 0)
                {
                    this.WriteLineCore();
                    x1 = Console.CursorLeft;
                    y1 = Console.CursorTop;
                }

                this.WriteCore(ss[i]);
            }

            var x2 = Console.CursorLeft;
            var y2 = Console.CursorTop;

            this.length += (y2 - y1) * Console.BufferWidth - x1 + x2;

            if (Console.CursorLeft != 0)
            {
                this.offsetY = -1;
                this.WriteLineCore();
            }
            else
            {
                this.offsetY = 0;
            }
            this.terminal.Top = Console.CursorTop;
            this.terminal.Draw();
            //this.offsetY = y2 - this.terminal.Top;
        }

        private void WriteCore(string text)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                using (var stream = Console.OpenStandardOutput())
                using (var writer = new StreamWriter(stream, this.encoding))
                {
                    writer.Write(text);
                }
            }
            else
            {
                this.writer.Write(text);
            }
        }

        private void WriteLineCore()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                using (var stream = Console.OpenStandardOutput())
                using (var writer = new StreamWriter(stream, this.encoding))
                {
                    writer.WriteLine();
                }
            }
            else
            {
                this.writer.WriteLine();
            }
        }
    }
}
