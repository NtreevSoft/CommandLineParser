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

        private void WriteMultilineCore(string s)
        {
            var indent = this.Indent;
            var emptyCount = this.TabString.Length * this.Indent;
            var width = Console.WindowWidth - emptyCount;

            var i = emptyCount;
            this.Indent = 0;

            var x = 0;
            foreach (var item in s)
            {
                if (x == 0)
                {
                    this.Write(string.Empty.PadRight(emptyCount));
                    x += emptyCount;
                    if (item == ' ')
                        continue;
                }
                this.Write(item);
                x += CharWidth.mk_wcwidth_cjk(item);
                if (x == this.width || Console.CursorLeft == 0)
                {
                    x = 0;
                }

            }
            this.WriteLine();
            this.Indent = indent;
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
    }
}