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
        private int offsetY;
        private int x;

        public TerminalTextWriter(TextWriter writer, Terminal terminal, Encoding encoding)
        {
            this.writer = writer;
            this.terminal = terminal;
            this.encoding = encoding;
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
            Console.SetCursorPosition(this.x, this.terminal.Top + this.offsetY);

            var y = Console.CursorTop;
            var x = this.x;
            this.writer.Write(text);

            Terminal.NextPosition(text, ref x, ref y);
            this.x = x;
            if (this.x != 0)
            {
                this.offsetY = -1;
                this.writer.WriteLine();
            }
            //else
            //{
            //    var y = 0;
            //    Terminal.NextPosition(text, ref this.x, ref y);
            //}

            this.terminal.Top = Console.CursorTop;
            this.terminal.Draw();
        }
    }
}
