using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    class Terminal
    {
        private static object lockobj = new object();

        private readonly Dictionary<ConsoleKeyInfo, Action> actionMaps = new Dictionary<ConsoleKeyInfo, Action>();
        private readonly List<ConsoleChar> chars = new List<ConsoleChar>();

        private List<string> histories = new List<string>();

        private int y = 0;
        private int height = 1;
        private int index;
        private int start = 0;
        private bool isHidden;
        private string inputText;
        private int historyIndex;

        public Terminal()
        {
            this.actionMaps.Add(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false), this.Clear);
            this.actionMaps.Add(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false), this.Backspace);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false), this.Delete);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false), this.Home);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, true), this.DeleteToHome);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false), this.End);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, true), this.DeleteToEnd);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), this.PrevHistory);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), this.NextHistory);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false), this.Left);
            this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false), this.Right);
            this.actionMaps.Add(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false), this.NextCompletion);
            this.actionMaps.Add(new ConsoleKeyInfo('\t', ConsoleKey.Tab, true, false, false), this.PrevCompletion);
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

        private TextWriter w;
        public string ReadLine(string prompt, string defaultText, bool isHidden)
        {
            var oldOut = Console.Out;
            this.w = oldOut;
            var oldTreatControlCAsInput = Console.TreatControlCAsInput;
            Console.TreatControlCAsInput = true;
            Console.SetOut(new ConsoleTextWriter(Console.Out, this));

            try
            {
                return ReadLineImpl(prompt, defaultText, isHidden);
            }
            finally
            {
                Console.TreatControlCAsInput = oldTreatControlCAsInput;
                Console.SetOut(oldOut);
                Console.WriteLine();
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                var text = this.histories[this.historyIndex + 1];
                this.ClearText();
                this.InsertText(text);
                this.inputText = this.LeftText;
                this.historyIndex++;
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                var text = this.histories[this.historyIndex - 1];
                this.ClearText();
                this.InsertText(text);
                this.inputText = this.LeftText;
                this.historyIndex--;
            }
        }

        public IList<string> Histories
        {
            get { return this.histories; }
        }

        public void Clear()
        {
            lock (lockobj)
            {
                this.ClearText();
                this.inputText = this.LeftText;
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
            this.inputText = this.LeftText;
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
                {
                    this.Index--;
                    this.inputText = this.LeftText;
                }
            }
        }

        public void Right()
        {
            lock (lockobj)
            {
                this.Index++;
                this.inputText = this.LeftText;
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
                    this.ReplaceText(space);
                    this.chars.RemoveAt(this.Index);
                    this.ReplaceText(text);
                    this.inputText = this.LeftText;
                }
            }
        }

        public void DeleteToEnd()
        {
            lock (lockobj)
            {
                var sapce = this.RightSpace;
                this.ReplaceText(sapce);
                this.chars.RemoveRange(this.Index, this.Length - this.Index);
                this.inputText = this.LeftText;
            }
        }

        public void DeleteToHome()
        {
            lock (lockobj)
            {
                var text = this.RightText;
                this.ClearText();
                this.InsertText(text);
                this.Index = start;
            }
        }

        public void NextCompletion()
        {
            lock (lockobj)
            {
                var inputArgs = SplitAll(this.inputText);
                var args = SplitAll(this.LeftText);
                var find = (inputArgs.LastOrDefault() ?? string.Empty).Trim();
                var leftText = this.inputText.Substring(0, this.inputText.Length - find.Length);
                var text = args.LastOrDefault() ?? string.Empty;
                var items = SplitAll(leftText).Select(i => i.Trim()).Where(i => i != string.Empty).ToArray();
                var result = this.OnNextCompletion(items, text, find);
                if (result != null)
                {
                    Console.CursorVisible = false;
                    this.ClearText();
                    this.InsertText(leftText + result);
                    Console.CursorVisible = true;
                }
            }
        }

        public void PrevCompletion()
        {
            lock (lockobj)
            {
                var inputArgs = SplitAll(this.inputText);
                var args = SplitAll(this.LeftText);
                var find = (inputArgs.LastOrDefault() ?? string.Empty).Trim();
                var leftText = this.inputText.Substring(0, this.inputText.Length - find.Length);
                var text = args.LastOrDefault() ?? string.Empty;
                var items = SplitAll(leftText).Select(i => i.Trim()).Where(i => i != string.Empty).ToArray();
                var result = this.OnPrevCompletion(items, text, find);
                if (result != null)
                {
                    Console.CursorVisible = false;
                    this.ClearText();
                    this.InsertText(leftText + result);
                    Console.CursorVisible = true;
                }
            }
        }

        protected virtual string OnNextCompletion(string[] items, string text, string find)
        {
            return null;
        }

        protected virtual string OnPrevCompletion(string[] items, string text, string find)
        {
            return null;
        }

        private void ShiftDown()
        {
            Console.MoveBufferArea(0, this.y, Console.BufferWidth, this.height, 0, this.y + 1);
            this.y++;
            this.Index = this.Length;
        }

        private int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                if (value < 0 || value > this.Length)
                    return;
                var x = 0;
                for (var i = 0; i < value; i++)
                {
                    x += this.chars[i].Slot;
                }
                Console.SetCursorPosition(x % Console.BufferWidth, x / Console.BufferWidth + y);
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

        private string LeftText
        {
            get
            {
                var text = string.Empty;
                for (var i = this.start; i < this.index; i++)
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
                return "\0".PadRight(count);
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
                return "\0".PadRight(count);
            }
        }

        private void ClearText()
        {
            var space = this.Space;
            this.Index = this.start;
            this.ReplaceText(space);
            while (this.Length > this.start)
            {
                this.chars.RemoveAt(this.Length - 1);
            }
        }

        private void ReplaceText(string text)
        {
            var index = this.Index;
            using (var stream = Console.OpenStandardOutput())
            {
                var bytes = Console.OutputEncoding.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
            }
            this.Index = index;
        }

        private void InsertText(string text)
        {
            var rightText = this.RightText;
            foreach (var item in text)
            {
                this.InsertChar(item);
            }
            this.ReplaceText(rightText);
        }

        private void InsertText(char ch)
        {
            lock (lockobj)
            {
                var rightText = this.RightText;
                this.InsertChar(ch);
                this.ReplaceText(rightText);
            }
        }

        private void InsertChar(char ch)
        {
            var x1 = Console.CursorLeft;
            var y1 = Console.CursorTop;

            if (this.isHidden == false)
            {
                //using (var stream = Console.OpenStandardOutput())
                //{
                //    var bytes = Console.OutputEncoding.GetBytes(ch.ToString());
                //    stream.Write(bytes, 0, bytes.Length);
                //}
                this.w.Write(ch);
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
        
        private string ReadLineImpl(string prompt, string defaultText, bool isHidden)
        {
            lock (lockobj)
            {
                this.y = Console.CursorTop;
                this.InsertText(prompt);
                this.start = this.Index;
                this.isHidden = isHidden;
                this.InsertText(defaultText);
                this.inputText = string.Empty;
            }

            while (true)
            {
                var key = Console.ReadKey(true);

                if (this.actionMaps.ContainsKey(key) == true)
                {
                    this.actionMaps[key]();

                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    var text = this.Text;
                    this.Index = this.Length;
                    var x = Console.CursorLeft;
                    var y = Console.CursorTop;
                    this.chars.Clear();
                    this.start = 0;
                    this.index = 0;

                    if (this.isHidden == false && text != string.Empty)
                    {
                        if (this.histories.LastOrDefault() != text)
                            this.histories.Add(text);
                        if (this.historyIndex != this.histories.Count)
                            this.historyIndex++;
                    }
                    Console.SetCursorPosition(x, y);
                    return text;
                }
                else if (key.KeyChar != '\0')
                {
                    this.InsertText(key.KeyChar);
                    this.inputText = this.LeftText;
                }
            }
        }

        private static IList<string> SplitAll(string text)
        {
            var pattern = @"^((""[^""]*"")|(\S+)|(\s+))";
            var match = Regex.Match(text, pattern);
            var argList = new List<string>();

            while (match.Success)
            {
                text = text.Substring(match.Length);
                argList.Add(match.Value);
                match = Regex.Match(text, pattern);
            }

            return argList;
        }

        private static string TrimQuot(string text)
        {
            if (text.StartsWith("\"") == true && text.EndsWith("\"") == true)
            {
                text = text.Substring(1);
                text = text.Remove(text.Length - 1);
            }
            return text;
        }

        #region classes

        struct ConsoleChar
        {
            public int Slot { get; set; }

            public char Char { get; set; }
        }

        class ConsoleTextWriter : TextWriter
        {
            private readonly TextWriter writer;
            private readonly Terminal prompter;
            private int x;
            private int y;

            public ConsoleTextWriter(TextWriter writer, Terminal prompter)
            {
                this.writer = writer;
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
                    var oldIndex = this.prompter.Index;

                    try
                    {
                        this.WriteToStream(value);
                    }
                    finally
                    {
                        this.prompter.Index = oldIndex;
                    }

                    this.x = Console.CursorLeft;
                    this.y = Console.CursorTop;
                    Console.CursorVisible = true;
                }
            }

            public override void Write(string value)
            {
                lock (lockobj)
                {
                    Console.CursorVisible = false;
                    var oldIndex = this.prompter.Index;

                    try
                    {
                        foreach (var item in value)
                        {
                            this.WriteToStream(item);
                        }
                    }
                    finally
                    {
                        this.prompter.Index = oldIndex;
                    }
                    this.x = Console.CursorLeft;
                    this.y = Console.CursorTop;
                    Console.CursorVisible = true;
                }
            }

            public override void WriteLine(string value)
            {
                lock (lockobj)
                {
                    Console.CursorVisible = false;
                    var oldIndex = this.prompter.Index;

                    try
                    {
                        var text = value + Environment.NewLine;
                        foreach (var item in text)
                        {
                            this.WriteToStream(item);
                        }
                    }
                    finally
                    {
                        this.prompter.Index = oldIndex;
                    }
                    this.x = Console.CursorLeft;
                    this.y = Console.CursorTop;
                    Console.CursorVisible = true;
                }
            }

            //private void WriteImpl(char ch)
            //{
            //    this.writer.Write(ch);
            //}

            //private System.Tuple<int, int> Insert(int x, int y, string text, TextWriter writer)
            //{
            //    var oldIndex = this.prompter.Index;

            //    foreach (var item in text)
            //    {
            //        var re = this.WriteToStream(x, y, item, writer);
            //        x = re.Item1;
            //        y = re.Item2;
            //    }

            //    try
            //    {
            //        return new Tuple<int, int>(Console.CursorLeft, Console.CursorTop);
            //    }
            //    finally
            //    {
            //        this.prompter.Index = oldIndex;
            //    }
            //}

            //private System.Tuple<int, int> Insert(int x, int y, char ch, TextWriter writer)
            //{
            //    var oldIndex = this.prompter.Index;

            //    var re = this.WriteToStream(x, y, ch, writer);
            //    x = re.Item1;
            //    y = re.Item2;
            //    try
            //    {
            //        return new Tuple<int, int>(Console.CursorLeft, Console.CursorTop);
            //    }
            //    finally
            //    {
            //        this.prompter.Index = oldIndex;
            //    }
            //}

            private void WriteToStream(char ch)
            {
                var x1 = Console.CursorLeft;
                var y1 = Console.CursorTop;

                if (this.y == this.prompter.y)
                {
                    if (this.prompter.y + this.prompter.height == Console.BufferHeight)
                    {
                        Console.MoveBufferArea(0, 1, Console.BufferWidth, this.prompter.y - 1, 0, 0);
                        this.y -= this.prompter.height;
                    }
                    else
                    {
                        var x2 = Console.CursorLeft;
                        var y2 = Console.CursorTop;
                        this.prompter.ShiftDown();
                        Console.SetCursorPosition(x2, y2);
                    }
                }

                Console.SetCursorPosition(this.x, this.y);
                this.writer.Write(ch);
            }
        }

        static class ConsoleCursor
        {
            public static void Push()
            {

            }

            public static void Pop()
            {

            }
        }

        #endregion
    }
}
