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
        private static ConsoleKeyInfo cancelKeyInfo = new ConsoleKeyInfo('\u0003', ConsoleKey.C, false, false, true);
        private static object lockobj = new object();

        private readonly Dictionary<ConsoleKeyInfo, Action> actionMaps = new Dictionary<ConsoleKeyInfo, Action>();
        private readonly List<TerminalChar> chars = new List<TerminalChar>();

        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();

        private int y = Console.CursorTop;
        private int width = Console.BufferWidth;
        private int index;
        private int start = 0;
        private int historyIndex;
        private bool isHidden;
        private string inputText;
        private string completion = string.Empty;
        private TextWriter writer;
        //private TerminalTextWriter t;

        public Terminal()
        {
            if (Console.IsInputRedirected == true)
                throw new Exception("Terminal cannot use. Console.IsInputRedirected must be false");
            this.actionMaps.Add(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false), this.Clear);
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                this.actionMaps.Add(new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false), this.Backspace);
            }
            else
            {
                this.actionMaps.Add(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false), this.Backspace);
                this.actionMaps.Add(new ConsoleKeyInfo('\u001b', ConsoleKey.V, false, false, true), this.Paste);
            }
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

        public long? ReadLong(string prompt)
        {
            long v;
            var result = this.ReadNumber(prompt, null, i => long.TryParse(i, out v));
            if (result is long)
            {
                return (long)result;
            }
            return null;
        }

        public long? ReadLong(string prompt, long defaultValue)
        {
            long v;
            var result = this.ReadNumber(prompt, defaultValue, i => long.TryParse(i, out v));
            if (result is long)
            {
                return (long)result;
            }
            return null;
        }

        public double? ReadDouble(string prompt)
        {
            double v;
            var result = this.ReadNumber(prompt, null, i => double.TryParse(i, out v));
            if (result is double)
            {
                return (double)result;
            }
            return null;
        }

        public double? ReadDouble(string prompt, double defaultValue)
        {
            double v;
            var result = this.ReadNumber(prompt, defaultValue, i => double.TryParse(i, out v));
            if (result is double)
            {
                return (double)result;
            }
            return null;
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
            Console.SetOut(new TerminalTextWriter(Console.Out, this, Console.OutputEncoding));

            try
            {
                return ReadLineImpl(prompt, defaultText, isHidden, i => true);
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

        public ConsoleKey ReadKey(string prompt, params ConsoleKey[] filters)
        {
            this.writer = Console.Out;
            var oldTreatControlCAsInput = Console.TreatControlCAsInput;
            Console.TreatControlCAsInput = true;
            Console.SetOut(new TerminalTextWriter(Console.Out, this, Console.OutputEncoding));

            try
            {
                return ReadKeyImpl(prompt, filters);
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
                this.SetInputText();
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
                this.SetInputText();
                this.historyIndex--;
            }
            else if (this.histories.Count == 1)
            {
                var text = this.histories[0];
                this.ClearText();
                this.InsertText(text);
                this.SetInputText();
                this.historyIndex = 0;
            }
        }

        public IList<string> Histories
        {
            get { return this.histories; }
        }

        public IList<string> Completions
        {
            get { return this.completions; }
        }

        public void Clear()
        {
            lock (lockobj)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    this.ClearText();
                    this.SetInputText();
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
                        this.SetInputText();
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
                        this.SetInputText();
                    }
                }
            }
        }

        public void Right()
        {
            lock (lockobj)
            {
                if (this.Index + 1 <= this.Length)
                {
                    using (TerminalCursorVisible.Set(false))
                    {
                        this.Index++;
                        this.SetInputText();
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
                        this.SetInputText();
                    }
                }
            }
        }

        public void Paste()
        {
            lock (lockobj)
            {

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
                    this.SetInputText();
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
                    this.SetInputText();
                }
            }
        }

        public void NextCompletion()
        {
            lock (lockobj)
            {
                this.CompletionImpl(NextCompletion);
            }
        }

        public void PrevCompletion()
        {
            lock (lockobj)
            {
                this.CompletionImpl(PrevCompletion);
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
                Console.SetCursorPosition(x % Console.BufferWidth, x / Console.BufferWidth + this.Top);
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

        public string Prompt
        {
            get
            {
                var text = string.Empty;
                for (var i = 0; i < this.start; i++)
                {
                    text += this.chars[i + this.start].Char;
                }
                return text;
            }
        }

        public string FullText
        {
            get
            {
                var text = string.Empty;
                for (var i = 0; i < this.chars.Count; i++)
                {
                    text += this.chars[i].Char;
                }
                return text;
            }
        }

        public bool IsReading
        {
            get { return this.writer != null; }
        }

        public int Top
        {
            get
            {
                if (this.width != Console.BufferWidth)
                {
                    this.y = Console.CursorTop - this.index / Console.BufferWidth;
                    this.width = Console.BufferWidth;
                }
                return this.y;
            }
            internal set
            {
                this.y = value;
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

        public void SetPrompt(string prompt)
        {
            if (this.writer == null)
                throw new Exception("prompt can set only on read mode.");

            lock (lockobj)
            {
                using (TerminalCursorVisible.Set(false))
                {
                    var text = this.Text;
                    var index = this.Index;
                    this.start = 0;
                    this.Clear();
                    this.InsertText(prompt);
                    this.start = this.Index;
                    this.InsertText(text);
                    this.Index = index;
                }
            }
        }

        public event TerminalCancelEventHandler CancelKeyPress;

        public event EventHandler Cancelled;

        protected virtual void OnCancelKeyPress(TerminalCancelEventArgs e)
        {
            this.CancelKeyPress?.Invoke(this, e);
        }

        protected virtual void OnCancelled(EventArgs e)
        {
            this.Cancelled?.Invoke(this, e);
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            var query = from item in this.completions
                        where item.StartsWith(find)
                        select item;
            return query.ToArray();
        }

        internal void Erase()
        {
            var x = Console.CursorLeft;
            var y = Console.CursorTop;

            var length = 0;
            for (var i = 0; i < this.chars.Count; i++)
            {
                length += this.chars[i].Slot;
            }

            for (var i = 0; i < this.Height; i++)
            {
                if (length == 0)
                    continue;
                Console.SetCursorPosition(0, this.Top + i);
                if (Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    Console.MoveBufferArea(Console.WindowWidth - 1, this.Top + i, 1, 1, 0, this.Top + i);
                    this.writer.Write(new string(' ', Console.WindowWidth - 1));
                }
                else
                {
                    this.writer.Write("\r" + new string(' ', Console.WindowWidth));
                }
                length -= Console.BufferWidth;
            }
            Console.SetCursorPosition(x, y);
        }

        internal void Draw()
        {
            var x1 = Console.CursorLeft;
            var y1 = Console.CursorTop;
            var index = this.Index;
            var text = this.FullText;
            var y = Console.CursorTop;
            Console.SetCursorPosition(0, this.y);
            this.writer.Write(this.FullText);
            if (text.Length > 0 && text.Length % Console.BufferWidth == 0 && Console.CursorLeft == 0)
            {
                if (y == Console.CursorTop)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Unix && y == Console.BufferHeight - 1)
                    {
                        this.writer.WriteLine();
                    }
                    this.y--;
                    //this.t.y--;
                }
            }

            this.y = Console.CursorTop - (this.Height - 1);
            this.Index = index;
        }

        private int Length
        {
            get { return this.chars.Count - this.start; }
        }

        private int Height
        {
            get
            {
                var length = 0;
                for (var i = 0; i < this.chars.Count; i++)
                {
                    length += this.chars[i].Slot;
                }
                return length / Console.BufferWidth + 1;
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
            var y = Console.CursorTop;
            this.writer.Write(text);
            if (text.Length > 0 && Console.CursorLeft == 0)
            {
                if (y == Console.CursorTop)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Unix && y != Console.BufferHeight - 1)
                    {
                        this.writer.WriteLine();
                    }
                    this.y--;
                    //this.t.y--;
                }
            }
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
            var height = this.Height;

            if (this.isHidden == false)
            {
                this.writer.Write(ch);
            }

            var x2 = Console.CursorLeft;
            var y2 = Console.CursorTop;

            if (y1 != y2)
            {
                this.chars.Insert(this.index++, new TerminalChar()
                {
                    Slot = Console.BufferWidth - x1,
                    Char = ch,
                });
            }
            else if (x1 > x2)
            {
                this.y--;
                //this.t.y--;
                this.chars.Insert(this.index++, new TerminalChar()
                {
                    Slot = Console.BufferWidth - x1,
                    Char = ch,
                });
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    this.writer.WriteLine();
                }
            }
            else
            {
                this.chars.Insert(this.index++, new TerminalChar()
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
            {
                if (Console.CursorLeft == 0)
                {
                    var i = this.Index;
                    this.Index--;
                    if (Environment.OSVersion.Platform == PlatformID.Unix)
                        this.writer.Write(" ");
                    else
                        this.writer.Write("\0");
                    this.Index--;
                }
                else
                {
                    this.writer.Write("\b\0");
                }
            }

            this.Index = inputIndex;
            this.Index--;
            this.chars.RemoveAt(this.index);
            this.ReplaceText(text);
        }

        private void CompletionImpl(Func<string[], string, string> func)
        {
            var matches = new List<Match>(CommandLineParser.MatchCompletion(this.inputText));
            var find = string.Empty;
            var prefix = false;
            var postfix = false;
            var leftText = this.inputText;
            if (matches.Count > 0)
            {
                var match = matches.Last();
                var matchText = match.Value;
                if (matchText.Length > 0 && matchText.First() == '\"')
                {
                    prefix = true;
                    matchText = matchText.Substring(1);
                }
                if (matchText.Length > 1 && matchText.Last() == '\"')
                {
                    postfix = true;
                    matchText = matchText.Remove(matchText.Length - 1);
                }
                if (matchText == string.Empty || matchText.Trim() != string.Empty)
                {
                    find = matchText;
                    matches.RemoveAt(matches.Count - 1);
                    leftText = this.inputText.Remove(match.Index);
                }
            }

            var argList = new List<string>();
            for (var i = 0; i < matches.Count; i++)
            {
                var matchText = CommandLineParser.RemoveQuot(matches[i].Value).Trim();
                if (matchText != string.Empty)
                    argList.Add(matchText);
            }

            var completions = this.GetCompletion(argList.ToArray(), find);
            if (completions != null && completions.Any())
            {
                this.completion = func(completions, this.completion);
                using (TerminalCursorVisible.Set(false))
                {
                    this.ClearText();
                    if (prefix == true || postfix == true)
                    {
                        this.InsertText(leftText + "\"" + this.completion + "\"");
                    }
                    else
                    {
                        this.InsertText(leftText + this.completion);
                    }
                }
            }
        }

        private void SetInputText()
        {
            this.inputText = this.Text.Remove(this.Index, this.Text.Length - this.Index);
            this.completion = string.Empty;
        }

        private string ReadLineImpl(string prompt, string defaultText, bool isHidden, Func<string, bool> validation)
        {
            lock (lockobj)
            {
                this.y = Console.CursorTop;
                this.width = Console.BufferWidth;
                this.index = 0;
                this.start = 0;
                this.chars.Clear();
                this.isHidden = false;
                this.InsertText(prompt);
                this.start = this.Index;
                this.isHidden = isHidden;
                this.InsertText(defaultText);
                this.inputText = string.Empty;
            }

            while (true)
            {
                var key = Console.ReadKey(true);

                //if (width != Console.BufferWidth)
                //{
                //    this.y = Console.CursorTop - this.index / Console.BufferWidth;
                //}

                if (key == cancelKeyInfo)
                {
                    var args = new TerminalCancelEventArgs(ConsoleSpecialKey.ControlC);
                    this.OnCancelKeyPress(args);
                    if (args.Cancel == false)
                    {
                        this.OnCancelled(EventArgs.Empty);
                        throw new OperationCanceledException($"ReadLine is cancelled.");
                    }
                }
                else if (this.actionMaps.ContainsKey(key) == true)
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
                        if (this.histories.Contains(text) == false)
                        {
                            this.histories.Add(text);
                            this.historyIndex = this.histories.Count;
                        }
                        else
                        {
                            this.historyIndex = this.histories.LastIndexOf(text) + 1;
                        }

                    }
                    Console.SetCursorPosition(x, y);
                    return text;
                }
                else if (key.KeyChar != '\0')
                {
                    if (validation(this.Text + key.KeyChar) == true)
                    {
                        this.InsertText(key.KeyChar);
                        this.SetInputText();
                    }
                }
            }
        }

        private object ReadNumber(string prompt, object defaultValue, Func<string, bool> validation)
        {
            this.writer = Console.Out;
            var oldTreatControlCAsInput = Console.TreatControlCAsInput;
            Console.TreatControlCAsInput = true;
            Console.SetOut(new TerminalTextWriter(Console.Out, this, Console.OutputEncoding));

            try
            {
                return ReadLineImpl(prompt, $"{defaultValue}", false, validation);
            }
            finally
            {
                Console.TreatControlCAsInput = oldTreatControlCAsInput;
                Console.SetOut(this.writer);
                Console.WriteLine();
                this.writer = null;
            }
        }

        private ConsoleKey ReadKeyImpl(string prompt, params ConsoleKey[] filters)
        {
            lock (lockobj)
            {
                this.y = Console.CursorTop;
                this.isHidden = false;
                this.InsertText(prompt);
                this.start = this.Index;
                this.isHidden = false;
                this.inputText = string.Empty;
            }

            while (true)
            {
                var key = Console.ReadKey(true);

                if ((int)key.Modifiers != 0)
                    continue;

                if (filters.Any() == false || filters.Any(item => item == key.Key) == true)
                {
                    this.InsertText(key.Key.ToString());
                    return key.Key;
                }
            }
        }

        internal static object LockedObject
        {
            get { return lockobj; }
        }

        #region classes

        struct TerminalChar
        {
            public int Slot { get; set; }

            public char Char { get; set; }
        }

        #endregion
    }
}
