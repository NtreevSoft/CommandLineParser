using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library
{
    public static class IndentedTextWriterExtensions
    {
        public static void WriteMultiline(this IndentedTextWriter textWriter, string s)
        {
            foreach (var item in s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (item == string.Empty)
                    textWriter.WriteLine();
                else
                    WriteMultilineCore(textWriter, item);
            }
        }

        private static void WriteMultilineCore(this IndentedTextWriter textWriter, string s)
        {
            var emptyCount = IndentedTextWriter.DefaultTabString.Length * textWriter.Indent + 1;
            var width = Console.WindowWidth - emptyCount;

            while (s != string.Empty)
            {
                var line = string.Empty;
                if (s.Length <= width)
                {
                    line = s;
                    s = string.Empty;
                }
                else
                {
                    line = s.Remove(width);
                    s = s.Substring(width);
                }
                textWriter.WriteLine(line.TrimStart());
            }
        }
    }
}
