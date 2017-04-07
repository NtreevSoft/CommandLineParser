using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    public class Terminal
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
                throw new Exception("Terminal cannot use. Console.IsInputRedirected must be false");
            this.actionMaps.Add(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false), this.Clear);
            if (Environment.OSVersion.Platform == PlatformID.Unix)
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

        public string ReadString(string prompt)
        {
            return ReadString(prompt, string.Empty);
        }

        public string ReadString(string prompt, bool isHidden)
        {
            return this.ReadString(prompt, string.Empty, isHidden);
        }

        public string ReadString(string prompt, string defaultText)
        {
            return this.ReadString(prompt, defaultText, false);
        }

        public string ReadString(string prompt, string defaultText, bool isHidden)
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

        public SecureString ReadSecureString(string prompt)
        {
            var text = this.ReadString(prompt, true);
            var secureString = new SecureString();
            foreach (var item in text)
            {
                secureString.AppendChar(item);
            }
            return secureString;
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
                using (TerminalCursorVisible.Set(false))
                {
                    this.ClearText();
                    this.inputText = this.LeftText;
                }
            }
        }

        public void Delete()
        {
            lock (lockobj)
            {
                if (this.Index < this.Length)
                {
                    using (TerminalCursorVisible.Set(false))
                    {
                        this.Index++;
                        this.Backspace();
                        this.inputText = this.LeftText;
                    }
                }
            }

        }

        public void Home()
        {
            lock (lockobj)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    this.Index = 0;
                }
            }
        }

        public void End()
        {
            lock (lockobj)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    this.Index = this.Length;
                }
            }
        }

        public void Left()
        {
            lock (lockobj)
            {
                if (this.Index > 0)
                {
                    using (TerminalCursorVisible.Set(false))
                    {
                        this.Index--;
                        this.inputText = this.LeftText;
                    }
                }
            }
        }

        public void Right()
        {
            lock (lockobj)
            {
                if (this.Index + 1 < this.Length)
                {
                    using (TerminalCursorVisible.Set(false))
                    {
                        this.Index++;
                        this.inputText = this.LeftText;
                    }
                }
            }
        }

        public void Backspace()
        {
            lock (lockobj)
            {
                if (this.Index > 0)
                {
                    using (TerminalCursorVisible.Set(false))
                    {
                        this.BackspaceImpl();
                        this.inputText = this.LeftText;
                    }
                }
            }
        }

        public void DeleteToEnd()
        {
            lock (lockobj)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    var index = this.Index;
                    this.Index = this.Length;
                    while (this.Index > index)
                    {
                        this.BackspaceImpl();
                    }
                    this.inputText = this.LeftText;
                }
            }
        }

        public void DeleteToHome()
        {
            lock (lockobj)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    while (this.Index > 0)
                    {
                        this.BackspaceImpl();
                    }
                    this.inputText = this.LeftText;
                }
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
                var completions = this.GetCompletion(items, find);
                if (completions != null && completions.Any())
                {
                    var completion = NextCompletion(completions, text);
                    using (TerminalCursorVisible.Set(false))
                    {
                        this.ClearText();
                        this.InsertText(leftText + completion);
                    }
                }
                //var result = this.OnNextCompletion(items, text, find);
                //if (result != null)
                //{
                //    using (TerminalCursorVisible.Set(false))
                //    {
                //        this.ClearText();
                //        this.InsertText(leftText + result);
                //    }
                //}
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
                var completions = this.GetCompletion(items, find);
                if (completions != null && completions.Any())
                {
                    var completion = PrevCompletion(completions, text);
                    using (TerminalCursorVisible.Set(false))
                    {
                        this.ClearText();
                        this.InsertText(leftText + completion);
                    }
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

        public static string NextCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item)
                         .ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i + 1 < completions.Length)
                            return completions[i + 1];
                        else
                            return completions.First();
                    }
                }
            }
            else
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        public static string PrevCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item)
                         .ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i - 1 >= 0)
                            return completions[i - 1];
                        else
                            return completions.Last();
                    }
                }
            }
            else
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            return null;
        }

        //protected virtual string OnNextCompletion(string[] items, string text, string find)
        //{
        //    return null;
        //}

        //protected virtual string OnPrevCompletion(string[] items, string text, string find)
        //{
        //    return null;
        //}

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
            while (this.Index > 0)
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
            if (this.isHidden == false)
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
                    using (TerminalCursorVisible.Set(false))
                    {
                        var oldIndex = this.prompter.Index;

                        try
                        {
                            this.WriteToStream(value);
                        }
                        finally
                        {
                            this.prompter.Index = oldIndex;
                        }
                    }
                }
            }

            public override void Write(string value)
            {
                lock (lockobj)
                {
                    using (TerminalCursorVisible.Set(false))
                    {
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
                    }
                }
            }

            public override void WriteLine(string value)
            {
                lock (lockobj)
                {
                    using (TerminalCursorVisible.Set(false))
                    {
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
                    }
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
                this.x = Console.CursorLeft;
                this.y = Console.CursorTop;
            }
        }

        #endregion
    }
}
