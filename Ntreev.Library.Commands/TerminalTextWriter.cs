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
        private int length;

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
            Console.SetCursorPosition(this.length % Console.BufferWidth, this.terminal.Top + this.offsetY);

            //var t = text;
            //while (t != string.Empty)
            //{
            //    var c = null as char?;
            //    var pre = string.Empty;
            //    for (var i = 0; i < t.Length; i++)
            //    {
            //        var ch = t[i];
            //        var w = Terminal.GetWidth(ch);
            //        if (w < 0)
            //        {
            //            pre = t.Substring(0, i);
            //            c = ch;
            //            t = t.Substring(i + 1);
            //            break;
            //        }
            //        else
            //        {
            //            pre += ch;
            //        }
            //        if (i + 1 == t.Length)
            //        {
            //            t = string.Empty;
            //        }
            //    }
            //    if (pre != string.Empty)
            //    {
            //        this.writer.Write(pre);
            //    }
            //    if (c != null)
            //    {
            //        Terminal.InsertChar(this.writer, c.Value, 0);
            //    }
            //}

            this.writer.Write(text);

            if (text == string.Empty || text.Last() != '\n')
            {
                this.length += Terminal.GetLength(text);
                this.offsetY = -1;
                this.writer.WriteLine();
            }
            else
            {
                this.length = 0;
            }

            this.terminal.Top = Console.CursorTop;
            this.terminal.Draw();
        }
    }
}
