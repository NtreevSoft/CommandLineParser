using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.Commands
{
    [Export(typeof(ICommand))]
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class ListDirectoryCommand : ICommand
    {
        [Import]
        private Lazy<CommandContext> commandContext = null;

        public string Name
        {
            get { return "ls"; }
        }

        public CommandTypes Types
        {
            get { return CommandTypes.None | CommandTypes.AllowEmptyArgument; }
        }

        public void Execute()
        {
            var dir = Directory.GetCurrentDirectory();
            this.PrintItems(dir);
        }

        private void PrintItems(string dir)
        {
            foreach (var item in Directory.GetDirectories(dir))
            {
                var itemInfo = new DirectoryInfo(item);

                this.Out.Write(itemInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh:mm"));
                this.Out.Write("\t");
                this.Out.Write("<DIR>");
                this.Out.Write("\t");
                this.Out.Write(itemInfo.Name);
                this.Out.WriteLine();
            }

            foreach (var item in Directory.GetFiles(dir))
            {
                var itemInfo = new FileInfo(item);

                this.Out.Write(itemInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh:mm"));
                this.Out.Write("\t");
                this.Out.Write("    ");
                this.Out.Write("\t");
                this.Out.Write(itemInfo.Name);
                this.Out.WriteLine();
            }

            if (this.IsRecursive == true)
            {
                foreach (var item in Directory.GetDirectories(dir))
                {
                    this.PrintItems(item);
                }
            }
        }

        [CommandSwitch(ShortName = 's', NameType = SwitchNameTypes.ShortName)]
        public bool IsRecursive
        {
            get; set;
        }

        public TextWriter Out
        {
            get { return this.commandContext.Value.Out; }
        }
    }
}
