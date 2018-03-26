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
using System.Threading;

namespace Ntreev.Library.Commands
{
    public class Terminal
    {
        private static ConsoleKeyInfo cancelKeyInfo = new ConsoleKeyInfo('\u0003', ConsoleKey.C, false, false, true);
        private static object lockobj = new object();
        private static readonly Dictionary<char, int> charToWidth = new Dictionary<char, int>(char.MaxValue);
        private static int bufferWidth = 80;

        private readonly Dictionary<ConsoleKeyInfo, Action> actionMaps = new Dictionary<ConsoleKeyInfo, Action>();
        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();

        private int y = Console.CursorTop;
        private int width = Console.BufferWidth;
        private int fullIndex;
        private int start = 0;
        private int historyIndex;
        private string fullText;
        private string inputText;
        private string completion = string.Empty;
        private TextWriter writer;
        private bool isHidden;
        private bool treatControlCAsInput;
        private bool isCancellationRequested;

        static Terminal()
        {
            {
                var name = $"{typeof(Terminal).Namespace}.bin.{PlatformID.Win32NT}.dat";
                using (var stream = typeof(Terminal).Assembly.GetManifestResourceStream(name))
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);

                    for (var i = char.MinValue; i < char.MaxValue; i++)
                    {
                        charToWidth.Add(i, buffer[i]);
                    }
                }
            }
        }

        public static int GetLength(string text)
        {
            var length = 0;
            foreach (var item in text)
            {
                length += charToWidth[item];
            }
            return length;
        }

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
                //this.actionMaps.Add(new ConsoleKeyInfo('\u0016', ConsoleKey.V, false, false, true), this.Paste);
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
            this.Initialize(prompt, defaultText, isHidden);

            try
            {
                return ReadLineImpl(i => true);
            }
            finally
            {
                this.Release();
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
            this.Initialize(prompt, string.Empty, false);
            try
            {
                return ReadKeyImpl(filters);
            }
            finally
            {
                this.Release();
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

        public void Cancel()
        {
            this.isCancellationRequested = true;
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
            get { return this.FullIndex - this.start; }
            set
            {
                this.FullIndex = value + this.start;
            }
        }

        internal int FullIndex
        {
            get { return this.fullIndex; }
            set
            {
                if (value < 0 || value > this.fullText.Length)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.fullIndex = value;
                if (this.isHidden == false)
                {
                    var x = 0;
                    var y = this.Top;
                    NextPosition(this.fullText.Substring(0, value), ref x, ref y);
                    Console.SetCursorPosition(x, Math.Min(y, Console.BufferHeight - 1));
                }
            }
        }

        public string Text
        {
            get { return this.fullText.Substring(this.start); }
        }

        public string Prompt
        {
            get { return this.fullText.Substring(0, this.start); }
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
                    this.y = Console.CursorTop - this.fullIndex / Console.BufferWidth;
                    this.width = Console.BufferWidth;
                }
                return this.y;
            }
            internal set
            {
                this.y = value;
            }
        }

        public static int BufferWidth
        {
            get
            {
                if (Console.IsOutputRedirected == false)
                    return Console.BufferWidth;
                return bufferWidth;
            }
            set
            {
                if (Console.IsOutputRedirected == false)
                    throw new InvalidOperationException("console output is redirected.");
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                bufferWidth = value;
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

        protected virtual void OnDrawPrompt(TextWriter writer, string prompt)
        {
            writer.Write(prompt);
        }

        protected virtual void OnDrawText(TextWriter writer, string text)
        {
            writer.Write(text);
        }

        internal static void NextPosition(string text, ref int x, ref int y)
        {
            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                if (ch == '\r')
                {
                    x = 0;
                    continue;
                }
                else if (ch == '\n')
                {
                    x = 0;
                    y++;
                    continue;
                }

                var w = charToWidth[ch];
                if (x + w >= Console.BufferWidth)
                {
                    x = x + w - Console.BufferWidth;
                    y++;
                }
                else
                {
                    x += w;
                }
            }
        }

        internal void Erase()
        {
            var x1 = Console.CursorLeft;
            var y1 = Console.CursorTop;

            var prompt = this.fullText.Substring(0, this.start);
            var text = this.isHidden == true ? string.Empty : this.fullText.Substring(this.start);
            var x = 0;
            var y = this.y;
            NextPosition(prompt, ref x, ref y);
            NextPosition(text, ref x, ref y);

            for (var i = this.y; i <= y; i++)
            {
                Console.SetCursorPosition(0, i);
                if (Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    Console.MoveBufferArea(Console.BufferWidth - 1, i, 1, 1, 0, i);
                    this.writer.Write($"\r{new string('\0', Console.BufferWidth - 1)}\r");
                }
                else
                {
                    this.writer.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
                }
            }
            Console.SetCursorPosition(x1, y1);
        }

        internal void Draw()
        {
            Console.SetCursorPosition(0, this.y);
            var index = this.FullIndex;
            var y = Console.CursorTop;
            var x = 0;
            var prompt = this.fullText.Substring(0, this.start);
            var text = this.isHidden == true ? string.Empty : this.fullText.Substring(this.start);
            this.OnDrawPrompt(this.writer, prompt);
            this.OnDrawText(this.writer, text);
            NextPosition(prompt, ref x, ref y);
            NextPosition(text, ref x, ref y);

            if (y >= Console.BufferHeight)
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix && x == 0)
                {
                    this.writer.WriteLine();
                }
                this.y--;
            }
            this.FullIndex = index;
        }

        private int Length
        {
            get { return this.fullText.Length - this.start; }
        }

        private void ClearText()
        {
            var x = 0;
            var y = this.Top;
            this.Erase();
            this.fullText = this.fullText.Substring(0, this.start);
            this.start = this.fullText.Length;
            this.fullIndex = this.start;
            this.inputText = string.Empty;
            Console.SetCursorPosition(x, y);
            this.Draw();
        }

        private void InsertText(string text)
        {
            if (text == string.Empty)
                return;
            lock (lockobj)
            {
                this.fullText = this.fullText.Insert(this.fullIndex, text);
                this.fullIndex += text.Length;

                if (this.isHidden == true)
                    return;

                var index = this.fullIndex;
                var text1 = this.fullText.Substring(this.start);

                using (TerminalCursorVisible.Set(false))
                {
                    this.FullIndex = this.start;
                    this.OnDrawText(this.writer, text1);


                    var x = 0;
                    var y = this.Top;
                    NextPosition(this.fullText, ref x, ref y);

                    if (y >= Console.BufferHeight)
                    {
                        if (Environment.OSVersion.Platform == PlatformID.Unix && x == 0)
                        {
                            this.writer.WriteLine();
                        }
                        this.y--;
                    }
                    this.FullIndex = index;
                }
            }
        }

        private void BackspaceImpl()
        {
            var extraText = this.fullText.Substring(this.fullIndex);
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
            this.fullText = this.fullText.Remove(this.fullIndex, 1);
            if (this.isHidden == false)
            {
                this.writer.Write(extraText);
            }
            this.Index = this.Index;
        }

        private void CompletionImpl(Func<string[], string, string> func)
        {
            var matches = new List<Match>(CommandStringUtility.MatchCompletion(this.inputText));
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
                var matchText = CommandStringUtility.TrimQuot(matches[i].Value).Trim();
                if (matchText != string.Empty)
                    argList.Add(matchText);
            }

            var completions = this.GetCompletion(argList.ToArray(), find);
            if (completions != null && completions.Any())
            {
                this.completion = func(completions, this.completion);
                using (TerminalCursorVisible.Set(false))
                {
                    var inputText = this.inputText;
                    this.ClearText();
                    if (prefix == true || postfix == true)
                    {
                        this.InsertText(leftText + "\"" + this.completion + "\"");
                    }
                    else
                    {
                        this.InsertText(leftText + this.completion);
                    }
                    this.inputText = inputText;
                }
            }
        }

        private void SetInputText()
        {
            this.inputText = this.Text.Remove(this.Index, this.Text.Length - this.Index);
            this.completion = string.Empty;
        }

        private object ReadNumber(string prompt, object defaultValue, Func<string, bool> validation)
        {
            this.Initialize(prompt, $"{defaultValue}", false);
            try
            {
                return ReadLineImpl(validation);
            }
            finally
            {
                this.Release();
            }
        }

        private string ReadLineImpl(Func<string, bool> validation)
        {
            while (true)
            {
                var keys = this.ReadKeys().ToArray();
                if (this.isCancellationRequested == true)
                    return null;

                var keyChars = string.Empty;
                foreach (var key in keys)
                {
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
                        this.fullText = string.Empty;
                        this.start = 0;
                        this.fullIndex = 0;

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
                        return text;
                    }
                    else if (key.KeyChar != '\0')
                    {
                        keyChars += key.KeyChar;
                    }
                }

                if (keyChars != string.Empty && validation(this.Text + keyChars) == true)
                {
                    this.InsertText(keyChars);
                    this.SetInputText();
                }
            }
        }

        private IEnumerable<ConsoleKeyInfo> ReadKeys()
        {
            while (this.isCancellationRequested == false)
            {
                var count = 0;
                while (true)
                {
                    if (Console.KeyAvailable == true)
                    {
                        yield return Console.ReadKey(true);
                    }
                    else if (count > 1)
                    {
                        yield break;
                    }
                    else
                    {
                        Thread.Sleep(1);
                        count++;
                    }
                }
            }
        }

        private ConsoleKey ReadKeyImpl(params ConsoleKey[] filters)
        {
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

        private void Initialize(string prompt, string defaultText, bool isHidden)
        {
            lock (lockobj)
            {
                this.writer = Console.Out;
                Console.SetOut(new TerminalTextWriter(Console.Out, this, Console.OutputEncoding));
                this.treatControlCAsInput = Console.TreatControlCAsInput;
                Console.TreatControlCAsInput = true;

                this.y = Console.CursorTop;
                this.width = Console.BufferWidth;
                this.start = 0;
                this.isHidden = false;
                this.fullText = prompt;
                this.fullIndex = prompt.Length;
                this.OnDrawPrompt(this.writer, this.fullText);
                this.FullIndex = this.fullIndex;
                this.start = this.fullIndex;
                this.isHidden = isHidden;
                this.InsertText(defaultText);
                this.inputText = defaultText;
            }
        }

        private void Release()
        {
            lock (lockobj)
            {
                Console.TreatControlCAsInput = this.treatControlCAsInput;
                Console.SetOut(this.writer);
                Console.WriteLine();
                this.writer = null;
                this.isHidden = false;
            }
        }

        internal static object LockedObject
        {
            get { return lockobj; }
        }
    }
}
