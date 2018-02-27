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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class CommandTextWriter : IndentedTextWriter
    {
        private readonly int width;

        public CommandTextWriter(TextWriter writer)
            : this(writer, Console.IsOutputRedirected == true ? int.MaxValue : Console.BufferWidth)
        {

        }

        public CommandTextWriter(TextWriter writer, int width)
            : base(writer)
        {
            this.width = width;
        }

        public void WriteMultiline(string s)
        {
            foreach (var item in s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (item == string.Empty)
                    this.WriteLine();
                else if (Console.IsOutputRedirected == true)
                    this.WriteLine(item);
                else
                    this.WriteMultilineCore(item);
            }
        }

        public string TabString
        {
            get { return IndentedTextWriter.DefaultTabString; }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
        }

        private void WriteMultilineCore(string s)
        {
            var indent = this.Indent;
            var emptyCount = this.TabString.Length * this.Indent;
            var width = Console.WindowWidth - emptyCount;

            this.Indent = 0;

            var i = 0;
            foreach (var item in s)
            {
                try
                {
                    if (Console.CursorLeft == 0)
                    {
                        this.Write(string.Empty.PadRight(emptyCount));
                        if (item == ' ' && i != 0)
                            continue;
                    }
                    var x = Console.CursorLeft;
                    this.Write(item);
                    if (item != ' ' && Console.CursorLeft != 0 && Console.CursorLeft < x)
                    {
                        this.Write("\r" + string.Empty.PadRight(emptyCount));
                        this.Write(item);
                    }
                }
                finally
                {
                    i++;
                }
            }
            this.WriteLine();
            this.Indent = indent;
        }
    }
}