using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class TableDataBuilder
    {
        private readonly object[] headers;
        private readonly List<string[]> rows = new List<string[]>();
        private string[][] data;

        public TableDataBuilder(params object[] headers)
        {
            this.headers = headers;
        }

        public void Add(params object[] items)
        {
            if (this.headers != null && this.headers.Length != items.Length)
                throw new ArgumentOutOfRangeException();

            this.rows.Add(items.Select(item => item == null ? string.Empty : item.ToString().Replace(Environment.NewLine, string.Empty)).ToArray());
        }

        public string[][] Data
        {
            get
            {
                if (this.data == null)
                {
                    if (this.headers != null)
                        this.rows.Insert(0, this.headers.Select(item => item.ToString()).ToArray());
                    this.data = this.rows.ToArray();
                }
                return this.data;
            }
        }
    }
}
