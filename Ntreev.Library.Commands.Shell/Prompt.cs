using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    static class Prompt
    {
        //private PromptWriter writer;
        private static CommandText command = new CommandText(Console.Out, "> ");

        static Prompt()
        {

        }

        public static void Use()
        {
            Console.SetOut(command);
        }

        public static void Start()
        {
            while (true)
            {
                Prompt.ProcessKey(Console.ReadKey());
            }
        }

        public static event EventHandler<ExecutionEventArgs> Execution;

        private static void ProcessKey(ConsoleKeyInfo consoleKeyInfo)
        {
            switch (consoleKeyInfo.Key)
            {
                case ConsoleKey.Enter:
                    {
                        string text = Prompt.command.Text;
                        if (Execution != null)
                        {
                            Execution(null, new ExecutionEventArgs(text));
                        }
                        Prompt.command.Clear();
                    }
                    break;
                case ConsoleKey.Escape:
                    {
                        Prompt.command.Clear();
                    }
                    break;
                case ConsoleKey.Backspace:
                    {
                        Prompt.command.Backspace();
                    }
                    break;
                case ConsoleKey.Delete:
                    {
                        Prompt.command.Delete(Prompt.command.Position);
                    }
                    break;
                case ConsoleKey.Home:
                    {
                        Prompt.command.Home();
                    }
                    break;
                case ConsoleKey.End:
                    {
                        Prompt.command.End();
                    }
                    break;
                case ConsoleKey.RightArrow:
                    {
                        Prompt.command.NextPosition();
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    {
                        Prompt.command.PrevPosition();
                    }
                    break;
                default:
                    {
                        char keyChar = consoleKeyInfo.KeyChar;
                        Prompt.command.Insert(Prompt.command.Position, keyChar);
                    }
                    break;
            }
        }

        class CommandText : TextWriter
        {
            private int outTop = 0;
            private int outLeft = 0;

            private int top = 0;
            private List<byte> command = new List<byte>();
            private readonly byte[] prompt;
            private readonly TextWriter writer;

            public CommandText(TextWriter writer, string prompt)
            {
                this.outTop = Console.CursorTop;
                this.outLeft = Console.CursorLeft;

                this.top = Console.CursorTop;

                this.writer = writer;
                this.prompt = writer.Encoding.GetBytes(prompt);
                this.Clear();
            }

            public override Encoding Encoding
            {
                get { return this.writer.Encoding; }
            }

            public override void Write(char value)
            {
                if (value == '\0' || value == '\b' || value == '')
                    return;
                this.writer.Write(value);
            }

            public override void Write(string value)
            {
                this.WriteCore(value, false);
            }

            public override void WriteLine(string value)
            {
                this.WriteCore(value, true);
            }

            private void WriteCore(string value, bool newLine)
            {
                int x = this.outLeft;
                int y = this.outTop;
                this.ComputeNextPosition(value, ref x, ref y);
                int pos = this.Position;
                int width = Math.Min(this.command.Count, Console.BufferWidth);
                int height = this.command.Count / Console.BufferWidth + 1;

                Console.MoveBufferArea(0, this.top, width, height, 0, y + 1);

                Console.SetCursorPosition(this.outLeft, this.outTop);

                if (newLine == true)
                    this.writer.WriteLine(value);
                else
                    this.writer.Write(value);

                this.outLeft = Console.CursorLeft;
                this.outTop = Console.CursorTop;

                this.top = y + 1;
                this.Position = pos;
            }

            private void ComputeNextPosition(string value, ref int left, ref int top)
            {
                int x = left;
                int y = top;
                foreach (byte item in this.Encoding.GetBytes(value))
                {
                    if (item == '\n')
                    {
                        y++;
                        x = 0;
                    }
                    else if (item == '\r')
                    {
                        x = 0;
                    }
                    else
                    {
                        x++;

                        if (x == Console.BufferWidth)
                        {
                            x = 0;
                            y++;
                        }
                    }
                }

                left = x;
                top = y;
            }

            private int ComputeLinefeeds(string value)
            {
                int x = 0;
                int y = 1;
                foreach (byte item in this.Encoding.GetBytes(value))
                {
                    if (item == '\n')
                    {
                        y++;
                        x = 0;
                    }
                    else if (item == '\r')
                    {
                        x = 0;
                    }
                    else
                    {
                        x++;

                        if (x == Console.BufferWidth)
                        {
                            x = 0;
                            y++;
                        }
                    }
                }

                return y;
            }


            public override string ToString()
            {
                return this.ToString(0);
            }

            public string ToString(int index)
            {
                return this.writer.Encoding.GetString(this.command.ToArray(), index, this.command.Count - index);
            }

            public void Clear()
            {
                this.Position = 0;
                this.WriteEmpty(this.command.Count);

                this.command.Clear();
                this.command.AddRange(this.prompt);
                this.Position = 0;
                this.WriteText(0);
                this.Home();
            }

            public void Insert(int index, char keyChar)
            {
                if (keyChar == '\0')
                    return;

                byte[] bytes = this.writer.Encoding.GetBytes(keyChar.ToString());

                int pos = this.Position;

                this.command.InsertRange(pos - bytes.Length, bytes);
                this.WriteText(pos);

                this.Position = pos;
            }

            public void Home()
            {
                this.Position = this.prompt.Length;
            }

            public void End()
            {
                this.Position = this.command.Count;
            }

            public void Backspace()
            {
                if (this.Position > this.prompt.Length)
                {
                    int pos = this.PrevPosition();
                    this.Remove(pos);
                }
            }

            public void Delete(int pos)
            {
                if (this.Position < this.command.Count)
                {
                    this.Remove(this.Position);
                }
            }

            private void Remove(int pos)
            {
                int len = this.IsTwoBytes(pos) == true ? 2 : 1;

                this.command.RemoveRange(pos, len);
                this.WriteText(pos);
                this.WriteEmpty(len);
                this.Position = pos;
            }

            public void WriteText(int index)
            {
                this.writer.Write(this.ToString(index));
            }

            private void WriteEmpty(int len)
            {
                this.writer.Write(string.Empty.PadRight(len));
            }

            public void NextPosition()
            {
                this.NextPosition(this.Position);
            }

            public void NextPosition(int pos)
            {
                if (pos >= this.command.Count)
                    return;
                if (this.IsTwoBytes(pos) == true)
                    this.Position = pos + 2;
                else
                    this.Position = pos + 1;
            }

            public int PrevPosition()
            {
                return this.PrevPosition(this.Position);
            }

            public int PrevPosition(int pos)
            {
                if (pos <= this.prompt.Length)
                    return this.prompt.Length;
                if (this.IsTwoBytes(pos - 2) == true)
                    this.Position = pos - 2;
                else
                    this.Position = pos - 1;
                return this.Position;
            }

            public int Position
            {
                get
                {
                    return Console.CursorLeft + (Console.CursorTop - this.top) * Console.BufferWidth;
                }
                set
                {
                    int x = value % Console.BufferWidth;
                    int y = value / Console.BufferWidth + this.top;
                    Console.SetCursorPosition(x, y);
                }
            }

            public string Text
            {
                get { return this.ToString(this.prompt.Length); }
            }

            private bool IsSecondByte(int pos)
            {
                byte[] bytes = this.command.ToArray();
                string text = this.Encoding.GetString(bytes, pos - 1, 2);
                return text.Length == 1;
            }

            private bool IsTwoBytes(int pos)
            {
                if (pos + 2 > this.command.Count)
                    return false;
                byte[] bytes = this.command.ToArray();
                string text = this.Encoding.GetString(bytes, pos, 2);
                return text.Length == 1;
            }

            private void MoveNext()
            {
                int pos = this.Position;
                int width = Math.Min(this.command.Count, Console.BufferWidth);
                int height = this.command.Count / Console.BufferWidth + 1;

                Console.MoveBufferArea(0, this.top, width, height, 0, this.top + 1);

                Console.CursorLeft = 0;
                Console.CursorTop = this.top;
                //this.writer.Write(value);

                this.top++;
                this.Position = pos;
            }
        }
    }

    public class ExecutionEventArgs : EventArgs
    {
        private readonly string command;

        public ExecutionEventArgs(string command)
        {
            this.command = command;
        }

        public string Command
        {
            get { return this.command; }
        }
    }
}
