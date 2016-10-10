using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public static class TextWriterExtensions
    {
        public static void WriteLine<T>(this TextWriter writer, IEnumerable<T> items)
        {
            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public);

            var query = from property in properties
                        from item in items
                        group property.GetValue(item).ToString() by property into g
                        select g;

            var texts = new List<string>(items.Count());

            foreach (var item in query)
            {
                var length = item.Select(i => CharWidth.mk_wcswidth_cjk(i)).Max(i => i + (4 - (i % 4)));


            }
        }

        public static void WriteLine(this TextWriter writer, string[][] itemsArray, bool hasHeader)
        {
            var count = itemsArray.First().Length;

            var lengths = new int[count];

            for (var x = 0; x < count; x++)
            {
                var len = 0;
                for (var y = 0; y < itemsArray.Length; y++)
                {
                    len = Math.Max(CharWidth.mk_wcswidth_cjk(itemsArray[y][x]), len);
                }
                lengths[x] = len + (4 - (len % 4));
            }

            
            for (var y = 0; y < itemsArray.Length; y++)
            {
                for (var x = 0; x < itemsArray[y].Length; x++)
                {
                    var pad = lengths[x] - CharWidth.mk_wcswidth_cjk(itemsArray[y][x]);
                    writer.Write(itemsArray[y][x]);
                    writer.Write(string.Empty.PadRight(pad));
                    writer.Write(" ");
                }
                writer.WriteLine();

                if (y != 0 || hasHeader == false)
                    continue;

                for (var x = 0; x < itemsArray[y].Length; x++)
                {
                    var pad = lengths[x];
                    writer.Write(string.Empty.PadRight(pad, '-'));
                    writer.Write(" ");
                }
                writer.WriteLine();
            }
        }
    }
}
