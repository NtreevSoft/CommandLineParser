using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library
{
    public abstract class CommandContext
    {
        private readonly Dictionary<string, CommandLineParser> commands = new Dictionary<string, CommandLineParser>();
        private string name;
        private Version version;
        private TextWriter textWriter;
        private IndentedTextWriter tw;

        protected CommandContext(IEnumerable<ICommand> commands)
        {
            this.TextWriter = Console.Out;

            foreach (var item in commands)
            {
                this.commands.Add(item.Name, this.CreateInstance(item));
            }
        }

        public void Execute(string commandLine)
        {
            var segments = CommandLineParser.Split(commandLine);

            var name = segments[0];
            var arguments = segments[1];

            if (this.Name != name)
                throw new ArgumentException(string.Format("'{0}' 은 잘못된 명령입니다."));

            this.Execute(CommandLineParser.Split(arguments));
        }

        public virtual void PrintVersion()
        {
            var info = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            this.tw.WriteLine("{0} {1}", this.Name, this.Version);
            this.tw.WriteLine(info.LegalCopyright);
        }

        public virtual void PrintHelp()
        {
            this.tw.WriteLine("사용 가능한 명령들");

            this.tw.Indent++;
            foreach (var item in this.commands)
            {
                this.tw.WriteLine(item.Value.Name);
            }
            this.tw.Indent--;

        }

        public TextWriter TextWriter
        {
            get { return this.textWriter; }
            set
            {
                this.textWriter = value;
                if (this.textWriter == null)
                    this.textWriter = Console.Out;
                this.tw = new IndentedTextWriter(this.textWriter);
            }
        }

        public string Name
        {
            get
            {
                if ((this.name ?? string.Empty) == string.Empty)
                    return System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public Version Version
        {
            get
            {
                if (this.version == null)
                {
                    return new Version(FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion);
                }
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }

        public IReadOnlyDictionary<string, CommandLineParser> Parsers
        {
            get { return this.commands; }
        }

        protected virtual CommandLineParser CreateInstance(ICommand command)
        {
            return new CommandLineParser(command.Name, command);
        }

        private bool Execute(string[] args)
        {
            var commandName = args[0];
            var arguments = args[1];

            if (commandName == string.Empty)
            {
                this.TextWriter.WriteLine("type '{0} help' for usage.", this.name);
                return false;
            }
            else if (this.commands.ContainsKey(commandName) == true)
            {
                var parser = this.commands[commandName];
                var command = parser.Instance as ICommand;
                if (command.HasSubCommand == true)
                {
                    parser.Invoke(arguments);
                }
                else
                {
                    if (arguments == string.Empty)
                    {
                        parser.PrintUsage();
                        return false;
                    }
                    else if (parser.Parse(commandName + " " + arguments) == false)
                    {
                        return false;
                    }

                    command.Execute();
                }
                return true;
            }
            else if (commandName == "help")
            {
                this.PrintHelp(CommandLineParser.Split(arguments));
                return false;

            }
            else if (commandName == "--version")
            {
                this.PrintVersion();
                return false;

            }

            throw new ArgumentException(string.Format("{0} 은(는) 존재하지 않는 명령어입니다", commandName));
        }

        private void PrintHelp(string[] args)
        {
            if (args.Any() == false || args.First() == string.Empty)
            {
                this.PrintHelp();
            }
            else
            {
                var commandName = args.First();
                if (this.commands.ContainsKey(commandName) == true)
                {
                    var parser = this.commands[commandName];
                    var command = parser.Instance as ICommand;

                    if (command.HasSubCommand == true)
                    {
                        var subCommandName = args.Skip(1).FirstOrDefault() ?? string.Empty;
                        if (subCommandName == string.Empty)
                            parser.PrintMethodUsage();
                        else
                            parser.PrintMethodUsage(subCommandName);
                    }
                    else
                    {
                        parser.PrintUsage();
                    }
                }
            }
        }
    }
}
