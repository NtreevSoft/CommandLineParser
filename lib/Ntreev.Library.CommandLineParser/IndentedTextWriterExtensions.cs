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
                WriteMultilineCore(textWriter, item);
            }
        }

        private static void WriteMultilineCore(this IndentedTextWriter textWriter, string s)
        {
            var emptyCount = IndentedTextWriter.DefaultTabString.Length * textWriter.Indent + 1;
            var width = Console.WindowWidth - emptyCount;

            while (s != string.Empty)
            {
                if (s.Length <= width)
                {
                    textWriter.WriteLine(s);
                    break;
                }
                else
                {
                    textWriter.WriteLine(s.Remove(width));
                    s = s.Substring(width);
                }
            }
        }
    }
}
