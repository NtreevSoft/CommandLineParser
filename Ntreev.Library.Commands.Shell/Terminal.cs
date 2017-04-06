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
        private int historyIndex;
        private bool isHidden;
        private string inputText;
        private TextWriter writer;

        public Terminal()
        {
			if (Console.IsInputRedirected == true)
				throw new Exception ("Terminal cannot use. Console.IsInputRedirected must be false");
            this.actionMaps.Add(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false), this.Clear);
			if(Environment.OSVersion.Platform == PlatformID.Unix)
                this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false), this.Backspace);
			else
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
        
        public string ReadLine(string prompt, string defaultText, bool isHidden)
        {
            this.writer = Console.Out;
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
                Console.SetOut(this.writer);
                Console.WriteLine();
                this.writer = null;
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                var text = this.histories[this.historyIndex + 1];
                this.ClearText();
                this.InsertText(text);
                this.inputText = this.Text.Remove(this.Index);
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
                Console.CursorVisible = false;
                this.ClearText();
                this.inputText = this.LeftText;
                Console.CursorVisible = true;
            }
        }
        
        public void Delete()
        {
            lock (lockobj)
            {
                if (this.Index < this.Length)
                {
                    Console.CursorVisible = false;
                    this.Index++;
                    this.Backspace();
                    this.inputText = this.LeftText;
                    Console.CursorVisible = true;
                }
            }
            
        }

        public void Home()
        {
            lock (lockobj)
            {
                Console.CursorVisible = false;
                this.Index = 0;
                Console.CursorVisible = true;
            }
        }

        public void End()
        {
            lock (lockobj)
            {
                Console.CursorVisible = false;
                this.Index = this.Length;
                Console.CursorVisible = true;
            }
        }

        public void Left()
        {
            lock (lockobj)
            {
                if (this.Index > 0)
                {
                    Console.CursorVisible = false;
                    this.Index--;
                    this.inputText = this.LeftText;
                    Console.CursorVisible = true;
                }
            }
        }

        public void Right()
        {
            lock (lockobj)
            {
                if (this.Index + 1 < this.Length)
                {
                    Console.CursorVisible = false;
                    this.Index++;
                    this.inputText = this.LeftText;
                    Console.CursorVisible = true;
                }
            }
        }

        public void Backspace()
        {
            lock (lockobj)
            {
                if (this.Index > 0)
                {
                    Console.CursorVisible = false;
                    this.BackspaceImpl();
                    this.inputText = this.LeftText;
                    Console.CursorVisible = true;
                }
            }
        }
        
        public void DeleteToEnd()
        {
            lock (lockobj)
            {
                Console.CursorVisible = false;
                var index = this.Index;
                this.Index = this.Length;
                while (this.Index > index)
                {
                    this.BackspaceImpl();
                }
                this.inputText = this.LeftText;
                Console.CursorVisible = true;
            }
        }

        public void DeleteToHome()
        {
            lock (lockobj)
            {
                Console.CursorVisible = false;
                while (this.Index > 0)
                {
                    this.BackspaceImpl();
                }
                this.inputText = this.LeftText;
                Console.CursorVisible = true;
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

        public int Index
        {
            get
            {
                return this.index - this.start;
            }
            set
            {
                if (value < 0 || value > this.Length)
                    return;
                var x = 0;
                for (var i = 0; i < value + this.start; i++)
                {
                    x += this.chars[i].Slot;
                }
                Console.SetCursorPosition(x % Console.BufferWidth, x / Console.BufferWidth + y);
                this.index = value + this.start;
            }
        }

        public string Text
        {
            get
            {
                var text = string.Empty;
                for (var i = 0; i < this.Length; i++)
                {
                    text += this.chars[i + this.start].Char;
                }
                return text;
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

        private int Length
        {
            get { return this.chars.Count - this.start; }
        }

        private string LeftText
        {
            get
            {
                return this.Text.Remove(this.Index, this.Text.Length - this.Index);
            }
        }

        private void ClearText()
        {
            this.Index = this.Length;
            while(this.Index > 0)
            {
                this.BackspaceImpl();
            }
        }

        private void ReplaceText(string text)
        {
            var index = this.Index;
			this.writer.Write(text);
            this.Index = index;
        }

        private void InsertText(string text)
        {
            var text2 = this.Text.Substring(this.Index);
            foreach (var item in text)
            {
                this.InsertChar(item);
            }
            this.ReplaceText(text2);
        }

        private void InsertText(char ch)
        {
            lock (lockobj)
            {
                var text = this.Text.Substring(this.Index);
                this.InsertChar(ch);
                this.ReplaceText(text);
            }
        }

        private void InsertChar(char ch)
        {
            var x1 = Console.CursorLeft;
            var y1 = Console.CursorTop;

            if (this.isHidden == false)
            {
                this.writer.Write(ch);
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

        private void BackspaceImpl()
        {
            var text = this.Text.Substring(this.Index);
            var inputIndex = this.Index;
            this.Index = this.Length;
            this.writer.Write("\b\0");
            this.Index = inputIndex;
            this.Index--;
            this.chars.RemoveAt(this.index);
            this.ReplaceText(text);
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
