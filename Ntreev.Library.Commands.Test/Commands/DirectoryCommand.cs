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
    class DirectoryCommand : ICommand
    {
        public string Name
        {
            get { return "dir"; }
        }

        public CommandTypes Types
        {
            get { return CommandTypes.None; }
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

                Console.Write(itemInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh:mm"));
                Console.Write("\t");
                Console.Write("<DIR>");
                Console.Write("\t");
                Console.Write(itemInfo.Name);
                Console.WriteLine();
            }

            foreach (var item in Directory.GetFiles(dir))
            {
                var itemInfo = new FileInfo(item);

                Console.Write(itemInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh:mm"));
                Console.Write("\t");
                Console.Write("<DIR>");
                Console.Write("\t");
                Console.Write(itemInfo.Name);
                Console.WriteLine();
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
    }
}
