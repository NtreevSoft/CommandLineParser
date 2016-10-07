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
        private readonly Dictionary<ICommand, CommandLineParser> parsers = new Dictionary<ICommand, CommandLineParser>();
        private readonly Dictionary<string, ICommand> commands;
        private string name;
        private Version version;
        private TextWriter writer;
        private ICommand helpCommand;
        private ICommand versionCommand;

        protected CommandContext(IEnumerable<ICommand> commands)
        {
            this.VerifyName = true;
            this.Out = Console.Out;

            foreach (var item in commands)
            {
                this.parsers.Add(item, this.CreateInstance(item));
            }
            this.commands = commands.ToDictionary(item => item.Name);
            this.helpCommand = new HelpCommand(this);
            this.versionCommand = new VersionCommand(this);
            this.parsers.Add(this.helpCommand, this.CreateInstance(this.helpCommand));
            this.parsers.Add(this.versionCommand, this.CreateInstance(this.versionCommand));

            foreach(var item in this.parsers.Values)
            {
                item.VersionName = string.Empty;
                item.HelpName = string.Empty;
            }
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

        public TextWriter Out
        {
            get { return this.writer ?? Console.Out; }
            set { this.writer = value; }
        }

        public string Name
        {
            get
            {
                if (this.name == null)
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

        public IReadOnlyDictionary<string, ICommand> Commands
        {
            get { return this.commands; }
        }

        public IReadOnlyDictionary<ICommand, CommandLineParser> Parsers
        {
            get { return this.parsers; }
        }

        public virtual ICommand HelpCommand
        {
            get { return this.helpCommand; }
        }

        public virtual ICommand VersionCommand
        {
            get { return this.versionCommand; }
        }

        public bool VerifyName { get; set; }

        protected virtual CommandLineParser CreateInstance(ICommand command)
        {
            return new CommandLineParser(command.Name, command);
        }

        protected virtual bool IsCommandVisible(ICommand command)
        {
            return true;
        }

        private bool Execute(string[] args)
        {
            var commandName = args[0];
            var arguments = args[1];

            if (commandName == string.Empty)
            {
                this.Out.WriteLine("type '{0} {1}' for usage.", this.name, this.HelpCommand.Name);
                this.Out.WriteLine("type '{0} {1}' to see the version.", this.name, this.VersionCommand.Name);
                return false;
            }
            else if (commandName == this.HelpCommand.Name)
            {
                return this.Execute(this.HelpCommand, arguments);
            }
            else if (commandName == this.VersionCommand.Name)
            {
                return this.Execute(this.VersionCommand, arguments);
            }
            else if (this.commands.ContainsKey(commandName) == true)
            {
                return this.Execute(this.commands[commandName], arguments);
            }

            throw new ArgumentException(string.Format("{0} 은(는) 존재하지 않는 명령어입니다", commandName));
        }

        private bool Execute(ICommand command, string arguments)
        {
            var parser = this.parsers[command];
            if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true)
            {
                if (arguments == string.Empty)
                {
                    if (command.Types.HasFlag(CommandTypes.AllowEmptyArgument) == true)
                    {
                        command.Execute();
                    }
                    else
                    {
                        this.Out.WriteLine("type '{0}' for usage.", string.Join(" ", this.Name, this.HelpCommand.Name, command.Name).Trim());
                        return false;
                    }
                }
                else if (parser.Invoke(command.Name + " " + arguments) == false)
                {
                    return false;
                }
            }
            else
            {
                if (arguments == string.Empty)
                {
                    if (command.Types.HasFlag(CommandTypes.AllowEmptyArgument) == true)
                    {
                        command.Execute();
                        return false;
                    }
                    else
                    {
                        this.Out.WriteLine("type '{0}' for usage.", string.Join(" ", this.Name, this.HelpCommand.Name, command.Name).Trim());
                        return false;
                    }
                }
                else if (parser.Parse(command.Name + " " + arguments) == false)
                {
                    return false;
                }

                command.Execute();
            }
            return true;
        }
    }
}
