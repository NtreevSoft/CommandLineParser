using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public static class IndentedTextWriterExtensions
    {
        public static void WriteMultiline(this IndentedTextWriter writer, string s)
        {
            foreach (var item in s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (item == string.Empty)
                    writer.WriteLine();
                else
                    WriteMultilineCore(writer, item);
            }
        }

        private static void WriteMultilineCore(this IndentedTextWriter writer, string s)
        {
            var oldIndent = writer.Indent;
            var emptyCount = IndentedTextWriter.DefaultTabString.Length * writer.Indent;
            if (Console.CursorLeft != 0)
                emptyCount = Console.CursorLeft;
            var width = (Console.WindowWidth - emptyCount) - 1;

            try

            {
                writer.Indent = 0;
                foreach (var i in s)
                {
                    if (Console.CursorLeft == 0)
                        Console.CursorLeft = emptyCount;
                    writer.Write(i);
                }
                writer.WriteLine();
            }
            finally
            {
                writer.Indent = oldIndent;
            }
        }
    }
}
