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

namespace Ntreev.Library.Commands
{
    public abstract class CommandContext
    {
        private readonly Dictionary<string, CommandLineParser> commands = new Dictionary<string, CommandLineParser>();
        private string name;
        private Version version;
        private TextWriter writer;

        protected CommandContext(IEnumerable<ICommand> commands)
        {
            this.VerifyName = true;
            this.Out = Console.Out;

            foreach (var item in commands)
            {
                this.commands.Add(item.Name, this.CreateInstance(item));
            }

            if (this.commands.ContainsKey("help") == false)
                this.commands.Add("help", this.CreateInstance(new HelpCommand(this)));
        }

        public void Execute(string commandLine)
        {
            var segments = CommandLineParser.Split(commandLine);

            var name = segments[0];
            var arguments = segments[1];

            if (this.VerifyName == true && this.Name != name)
                throw new ArgumentException(string.Format("'{0}' 은 잘못된 명령입니다.", name));

            this.Execute(CommandLineParser.Split(arguments));
        }

        public virtual void PrintVersion()
        {
            var info = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            using (var writer = new IndentedTextWriter(this.Out))
            {
                writer.WriteLine("{0} {1}", this.Name, this.Version);
                writer.WriteLine(info.LegalCopyright);
            }
        }

        public virtual void PrintHelp()
        {
            using (var writer = new IndentedTextWriter(this.Out))
            {

            }
        }

        public TextWriter Out
        {
            get { return this.writer ?? Console.Out; }
            set { this.writer = value; }
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

        public bool VerifyName { get; set; }

        public TextWriter Writer
        {
            get
            {
                return writer;
            }

            set
            {
                writer = value;
            }
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
                this.Out.WriteLine("type '{0} help' for usage.", this.name);
                return false;
            }
            else if (this.commands.ContainsKey(commandName) == true)
            {
                var parser = this.commands[commandName];
                var command = parser.Instance as ICommand;
                if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true)
                {
                    parser.Invoke(arguments);
                }
                else
                {
                    if(arguments == string.Empty && command.Types.HasFlag(CommandTypes.AllowEmptyArgument))
                    {
                        command.Execute();
                        return true;
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

                    if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true)
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

        private void PrintHel(IndentedTextWriter textWriter)
        {

            textWriter.WriteLine("Commands");

            textWriter.Indent++;
            foreach (var item in this.commands)
            {
                textWriter.WriteLine(item.Value.Name);
            }
            textWriter.Indent--;
        }
    }
}
