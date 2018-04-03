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
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell.Commands
{
    [Export(typeof(ICommand))]
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class ListDirectoryCommandPartial : CommandBase
    {
        [Import]
        private Lazy<ShellCommandContext> commandContext = null;

        public ListDirectoryCommandPartial()
            : base("ls")
        {

        }

        [CommandProperty('s', true)]
        public bool IsRecursive
        {
            get; set;
        }

        public TextWriter Out
        {
            get { return this.commandContext.Value.Out; }
        }

        protected override void OnExecute()
        {
            var dir = Directory.GetCurrentDirectory();
            this.PrintItems(dir);
        }

        private void PrintItems(string dir)
        {
            var items = new List<string[]>();

            {
                var props = new List<string>();
                props.Add("DateTime");
                props.Add("");
                props.Add("Name");
                items.Add(props.ToArray());
            }

            foreach (var item in Directory.GetDirectories(dir))
            {
                var itemInfo = new DirectoryInfo(item);

                var props = new List<string>();
                props.Add(itemInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh:mm"));
                props.Add("<DIR>");
                props.Add(itemInfo.Name);
                items.Add(props.ToArray());
            }

            foreach (var item in Directory.GetFiles(dir))
            {
                var itemInfo = new FileInfo(item);

                var props = new List<string>();
                props.Add(itemInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh:mm"));
                props.Add(string.Empty);
                props.Add(itemInfo.Name);
                items.Add(props.ToArray());
            }

            this.Out.WriteLine();
            this.Out.PrintTableData(items.ToArray(), true);
            this.Out.WriteLine();

            if (this.IsRecursive == true)
            {
                foreach (var item in Directory.GetDirectories(dir))
                {
                    this.PrintItems(item);
                }
            }
        }
    }
}
