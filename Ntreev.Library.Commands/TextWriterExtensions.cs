using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public static class TextWriterExtensions
    {
        public static void WriteItems<T>(this TextWriter writer, IEnumerable<T> items)
        {
            var properties = typeof(T).GetProperties();

            var query = from item in properties
                        where item.PropertyType.IsArray == false
                        select item;

            var headers = from item in query
                          let displayName = item.GetDisplayName()
                          select displayName != string.Empty ? displayName : item.Name;

            var dataBuilder = new TableDataBuilder(headers.ToArray());

            foreach (var item in items)
            {
                dataBuilder.Add(query.Select(i => i.GetValue(item)).ToArray());
            }

            writer.WriteLine(dataBuilder.Data, true);
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
