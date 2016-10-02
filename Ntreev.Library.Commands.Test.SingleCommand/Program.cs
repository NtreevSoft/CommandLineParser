using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.SingleCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();
            var parser = new CommandLineParser(settings);

            try
            {
                if (parser.Parse(Environment.CommandLine) == false)
                {
                    Environment.Exit(1);
                }

                if (Directory.Exists(settings.DirectoryName) == false)
                    throw new DirectoryNotFoundException();

                if (settings.OnlyName == true)
                    PrintItems(Console.Out, settings.DirectoryName, settings.IsRecursive);
                else
                    PrintItemsDetail(Console.Out, settings.DirectoryName, settings.IsRecursive);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(2);
            }
        }

        private static void PrintItems(TextWriter writer, string dir, bool isRecursive)
        {
            foreach (var item in Directory.GetDirectories(dir))
            {
                var itemInfo = new DirectoryInfo(item);
                writer.WriteLine(itemInfo.Name);
            }

            foreach (var item in Directory.GetFiles(dir))
            {
                var itemInfo = new FileInfo(item);
                writer.WriteLine(itemInfo.Name);
            }

            if (isRecursive == true)
            {
                foreach (var item in Directory.GetDirectories(dir))
                {
                    PrintItems(writer, item, isRecursive);
                }
            }
        }

        private static void PrintItemsDetail(TextWriter writer, string dir, bool isRecursive)
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

            writer.WriteLine();
            writer.WriteLine(items.ToArray(), true);
            writer.WriteLine();

            if (isRecursive == true)
            {
                foreach (var item in Directory.GetDirectories(dir))
                {
                    PrintItemsDetail(writer, item, isRecursive);
                }
            }
        }
    }
}
