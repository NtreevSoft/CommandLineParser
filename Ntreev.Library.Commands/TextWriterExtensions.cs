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

            writer.PrintTableData(dataBuilder.Data, true);
        }

        public static void PrintTableData(this TextWriter writer, string[][] itemsArray, bool hasHeader)
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

        public static void Print<T>(this TextWriter writer, T[] items)
        {
            Print<T>(writer, items, (o, a) => a(), item => item.ToString());
        }

        public static void Print<T>(this TextWriter writer, T[] items, Action<T, Action> action)
        {
            Print<T>(writer, items, action, item => item.ToString());
        }

        /// <summary>
        /// linux에 ls 명령처럼 단순 문자열을 위에서 아래로 좌에서 우로 정렬해서 출력하는 기능
        /// </summary>
        public static void Print<T>(this TextWriter writer, T[] items, Action<T, Action> action, Func<T, string> selector)
        {
            var maxWidth = Console.IsOutputRedirected == false ? Console.BufferWidth : 80;
            var lineCount = 4;

            while (true)
            {
                var lines = new List<string>[lineCount];
                var objs = new List<T>[lineCount];
                var lengths = new int[lineCount];
                var columns = new List<int>();

                for (var i = 0; i < items.Length; i++)
                {
                    var y = i % lineCount;
                    var item = selector(items[i]);
                    if (lines[y] == null)
                    {
                        lines[y] = new List<string>();
                        objs[y] = new List<T>();
                        lengths[y] = item.Length + 2;
                    }
                    else
                    {
                        lengths[y] += item.Length + 2;
                    }

                    var c = lines[y].Count;
                    lines[y].Add(item);
                    objs[y].Add(items[i]);

                    if (columns.Count < lines[y].Count)
                        columns.Add(0);

                    columns[c] = Math.Max(columns[c], item.Length + 2);
                }

                var canPrint = true;
                for (var i = 0; i < lines.Length; i++)
                {
                    if (lines[i] == null)
                        continue;
                    var c = 0;
                    for (var j = 0; j < lines[i].Count; j++)
                    {
                        c += columns[j];
                    }
                    if (c >= maxWidth)
                        canPrint = false;
                }

                if (canPrint == false)
                {
                    lineCount++;
                    continue;
                }

                for (var i = 0; i < lines.Length; i++)
                {
                    if (lines[i] == null)
                        continue;
                    for (var j = 0; j < lines[i].Count; j++)
                    {
                        var obj = objs[i][j];
                        var text = lines[i][j].PadRight(columns[j]);
                        action(obj, () => writer.Write(text));
                    }
                    writer.WriteLine();
                }

                break;
            }
        }

        public static void Print<T>(this TextWriter writer, IDictionary<string, T> items)
        {
            Print<T>(writer, items, (o, a) => a(), item => $"{item}");
        }

        public static void Print<T>(this TextWriter writer, IDictionary<string, T> items, Action<T, Action> action)
        {
            Print<T>(writer, items, action, item => $"{item}");
        }

        /// <summary>
        /// 라벨과 값이 존재하는 아이템을 {0} : {1} 형태로 출력하는 기능
        /// </summary>
        public static void Print<T>(this TextWriter writer, IDictionary<string, T> items, Action<T, Action> action, Func<T, string> selector)
        {
            var maxWidth = items.Keys.Max(item => item.Length);

            foreach (var item in items)
            {
                var text = $"{item.Key.PadRight(maxWidth)} : {selector(item.Value)}";
                action(item.Value, () => writer.WriteLine(text));
            }
        }
    }
}
