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
        private Lazy<CommandContextBase> commandContext = null;

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
