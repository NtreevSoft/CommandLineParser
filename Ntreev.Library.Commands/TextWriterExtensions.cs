using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public static class TextWriterExtensions
    {
        public static void WriteLine<T>(IEnumerable<T> items)
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
    }
}
