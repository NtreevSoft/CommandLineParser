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

        public bool HasHeader
        {
            get { return this.headers != null; }
        }
    }
}
