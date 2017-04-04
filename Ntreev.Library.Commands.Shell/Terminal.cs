using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    class Terminal
    {
        private static object lockobj = new object();

        private List<string> histories = new List<string>();
        private int currentHistory = -1;

        private int y = 0;
        private int height = 1;
        private List<ConsoleChar> chars = new List<ConsoleChar>();
        private int index;
        private int start = 0;
        private bool isHidden;

        public Terminal()
        {

        }

        public string ReadLine(string prompt)
        {
            return ReadLine(prompt, string.Empty);
        }

        public string ReadLine(string prompt, bool isHidden)
        {
            return this.ReadLine(prompt, string.Empty, isHidden);
        }

        public string ReadLine(string prompt, string defaultText)
        {
            return this.ReadLine(prompt, defaultText, false);
        }

        public string ReadLine(string prompt, string defaultText, bool isHidden)
        {
            var cout = Console.Out;
            Console.SetOut(new ConsoleTextWriter(Console.Out, this));

            this.y = Console.CursorTop;
            this.Insert(prompt);
            this.start = this.Index;
            this.isHidden = isHidden;
            this.Insert(defaultText);
            while (true)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        {
                            var text = this.Text;
                            var space = this.Space;
                            this.Index = this.start;
                            this.chars.Clear();
                            this.start = 0;
                            this.index = 0;
                            Console.SetOut(cout);
                            Console.WriteLine();

                            if (this.histories.LastOrDefault() != text)
                                this.histories.Add(text);
                            return text;
                        }
                    case ConsoleKey.Escape:
                        {
                            this.Clear();
                        }
                        break;
                    case ConsoleKey.Backspace:
                        {
                            this.Backspace();
                        }
                        break;
                    case ConsoleKey.Delete:
                        {
                            this.Delete();
                        }
                        break;
                    case ConsoleKey.Home:
                        {
                            this.Home();
                        }
                        break;
                    case ConsoleKey.End:
                        {
                            this.End();
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        {
                            this.PrevHistory();
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        {
                            this.NextHistory();
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        {
                            this.Left();
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        {
                            this.Right();
                        }
                        break;
                    case ConsoleKey.Tab:
                        {
                            var text = this.OnCompletion();
                            if (text != null)
                            {
                                Console.CursorVisible = false;
                                this.Clear();
                                this.Insert(text);
                                Console.CursorVisible = true;
                            }
                        }
                        break;
                    default:
                        {
                            if (key.KeyChar != '\0')
                                this.Insert(key.KeyChar);
                        }
                        break;
                }
            }
        }

        public void NextHistory()
        {

        }

        public void PrevHistory()
        {

        }

        public IList<string> Histories
        {
            get { return this.histories; }
        }

        public void Enter()
        {

        }

        public void Clear()
        {
            lock (lockobj)
            {
                var space = this.Space;
                this.Index = this.start;
                this.Replace(space);
                while (this.Length > this.start)
                {
                    this.chars.RemoveAt(this.Length - 1);
                }
            }
        }

        public void Delete()
        {
            lock (lockobj)
            {
                if (this.Index < this.Length)
                {
                    this.Index++;
                    this.Backspace();
                }
            }
        }

        public void Home()
        {
            lock (lockobj)
            {
                this.Index = this.start;
            }
        }

        public void End()
        {
            lock (lockobj)
            {
                this.Index = this.Length;
            }
        }

        public void Left()
        {
            lock (lockobj)
            {
                if (this.Index > this.start)
                    this.Index--;
            }
        }

        public void Right()
        {
            lock (lockobj)
            {
                this.Index++;
            }
        }

        public void Backspace()
        {
            lock (lockobj)
            {
                if (this.Index > this.start)
                {
                    var text = this.RightText;
                    this.Index--;
                    var space = this.RightSpace;
                    this.Replace(space);
                    this.chars.RemoveAt(this.Index);
                    this.Replace(text);
                }
            }
        }

        public void DeleteToEnd()
        {
            var sapce = this.RightSpace;
            this.Replace(sapce);
            var index = this.Index;
            while (this.chars.Count > index)
            {
                this.chars.RemoveAt(this.chars.Count - 1);
            }
        }

        public void DeleteToHome()
        {

        }

        protected virtual string OnCompletion()
        {
            return null;
        }

        private void ShiftDown()
        {
            Console.MoveBufferArea(0, this.y, Console.BufferWidth, this.height, 0, this.y + 1);
            this.y++;
            this.Index = this.Length;
        }

        public int Index
        {
            get
            {
                return this.index;
            }
            private set
            {
                if (value < 0 || value > this.Length)
                    return;
                var x = 0;
                for (var i = 0; i < value; i++)
                {
                    x += this.chars[i].Slot;
                }
                Console.CursorLeft = x % Console.BufferWidth;
                Console.CursorTop = x / Console.BufferWidth + y;
                this.index = value;
            }
        }

        private int Length
        {
            get { return this.chars.Count; }
        }

        private string RightText
        {
            get
            {
                var text = string.Empty;
                for (var i = this.index; i < this.Length; i++)
                {
                    text += this.chars[i].Char;
                }
                return text;
            }
        }

        public string Text
        {
            get
            {
                var text = string.Empty;
                for (var i = this.start; i < this.Length; i++)
                {
                    text += this.chars[i].Char;
                }
                return text;
            }
        }

        private string RightSpace
        {
            get
            {
                var count = 0;
                for (var i = this.index; i < this.Length; i++)
                {
                    count += this.chars[i].Slot;
                }
                if (count == 0)
                    return string.Empty;
                return " ".PadRight(count);
            }
        }

        private string Space
        {
            get
            {
                var count = 0;
                for (var i = 0; i < this.Length; i++)
                {
                    count += this.chars[i].Slot;
                }
                if (count == 0)
                    return string.Empty;
                return " ".PadRight(count);
            }
        }

        private void Replace(string text)
        {
            var index = this.Index;
            using (var stream = Console.OpenStandardOutput())
            {
                var bytes = Console.OutputEncoding.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
            }
            this.Index = index;
        }

        private void Insert(string text)
        {
            lock (lockobj)
            {
                var rightText = this.RightText;
                foreach (var item in text)
                {
                    this.InsertChar(item);
                }
                this.Replace(rightText);
            }
        }

        private void Insert(char ch)
        {
            lock (lockobj)
            {
                var rightText = this.RightText;
                this.InsertChar(ch);
                this.Replace(rightText);
            }
        }

        private void InsertChar(char ch)
        {
            var x1 = Console.CursorLeft;
            var y1 = Console.CursorTop;

            if (this.isHidden == false)
            {
                using (var stream = Console.OpenStandardOutput())
                {
                    var bytes = Console.OutputEncoding.GetBytes(ch.ToString());
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            var x2 = Console.CursorLeft;
            var y2 = Console.CursorTop;

            if (y1 != y2)
            {
                this.chars.Insert(this.index++, new ConsoleChar()
                {
                    Slot = Console.BufferWidth - x1,
                    Char = ch,
                });
                this.height++;
            }
            else if (x1 > x2)
            {
                this.y--;
                this.chars.Insert(this.index++, new ConsoleChar()
                {
                    Slot = Console.BufferWidth - x1,
                    Char = ch,
                });
                this.height++;
            }
            else
            {
                this.chars.Insert(this.index++, new ConsoleChar()
                {
                    Slot = x2 - x1,
                    Char = ch,
                });
            }

        }

        private System.Tuple<int, int> Insert(int x, int y, string text)
        {
            var oldIndex = this.Index;

            foreach (var item in text)
            {
                var re = this.WriteToStream(x, y, item);
                x = re.Item1;
                y = re.Item2;
            }

            try
            {
                return new Tuple<int, int>(Console.CursorLeft, Console.CursorTop);
            }
            finally
            {
                this.Index = oldIndex;
            }
        }

        private System.Tuple<int, int> Insert(int x, int y, char ch)
        {
            var oldIndex = this.Index;

            var re = this.WriteToStream(x, y, ch);
            x = re.Item1;
            y = re.Item2;
            try
            {
                return new Tuple<int, int>(Console.CursorLeft, Console.CursorTop);
            }
            finally
            {
                this.Index = oldIndex;
            }
        }

        private System.Tuple<int, int> WriteToStream(int x, int y, char ch)
        {
            var x1 = Console.CursorLeft;
            var y1 = Console.CursorTop;


            if (y == this.y)
            {
                if (this.y + this.height == Console.BufferHeight)
                {
                    Console.MoveBufferArea(0, 1, Console.BufferWidth, this.y - 1, 0, 0);
                    y -= this.height;
                }
                else
                {
                    var x2 = Console.CursorLeft;
                    var y2 = Console.CursorTop;
                    this.ShiftDown();
                    Console.CursorLeft = x2;
                    Console.CursorTop = y2;
                }
            }

            Console.CursorLeft = x;
            Console.CursorTop = y;
            this.WriteToStream(ch);

            return new Tuple<int, int>(Console.CursorLeft, Console.CursorTop);
        }

        private void WriteToStream(char ch)
        {
            using (var stream = Console.OpenStandardOutput())
            {
                var bytes = Console.OutputEncoding.GetBytes(ch.ToString());
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        #region classes

        struct ConsoleChar
        {
            public int Slot { get; set; }

            public char Char { get; set; }
        }

        class ConsoleTextWriter : TextWriter
        {
            private readonly Terminal prompter;
            private int x;
            private int y;

            public ConsoleTextWriter(TextWriter writer, Terminal prompter)
            {
                this.prompter = prompter;
                this.x = Console.CursorLeft;
                this.y = Console.CursorTop;
            }

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }

            public override void Write(char value)
            {
                lock (lockobj)
                {
                    Console.CursorVisible = false;
                    var re = this.prompter.Insert(this.x, this.y, value);
                    this.x = re.Item1;
                    this.y = re.Item2;
                    Console.CursorVisible = true;
                }
            }

            public override void Write(string value)
            {
                lock (lockobj)
                {
                    Console.CursorVisible = false;
                    var re = this.prompter.Insert(this.x, this.y, value);
                    this.x = re.Item1;
                    this.y = re.Item2;
                    Console.CursorVisible = true;
                }
            }

            public override void WriteLine(string value)
            {
                lock (lockobj)
                {
                    Console.CursorVisible = false;
                    var re = this.prompter.Insert(this.x, this.y, value + Environment.NewLine);
                    this.x = re.Item1;
                    this.y = re.Item2;
                    Console.CursorVisible = true;
                }
            }
        }

        #endregion
    }
}
