using Ntreev.Library.Commands.Properties;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
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
    [InheritedExport(typeof(CommandContextBase))]
    public abstract class CommandContextBase
    {
        private readonly CommandLineParserCollection parsers = new CommandLineParserCollection();
        private readonly CommandCollection commands = new CommandCollection();
        private string name;
        private Version version;
        private TextWriter writer;
        private ICommand helpCommand;
        private ICommand versionCommand;

        protected CommandContextBase(IEnumerable<ICommand> commands)
        {
            this.VerifyName = true;
            this.Out = Console.Out;
            foreach (var item in commands)
            {
                this.commands.Add(item);
                this.parsers.Add(item, this.CreateInstance(this, item));
            }
            this.parsers.Add(this.HelpCommand, this.CreateInstance(this, this.HelpCommand));
            this.parsers.Add(this.VersionCommand, this.CreateInstance(this, this.versionCommand));

            foreach (var item in this.parsers)
            {
                item.VersionName = null;
                item.HelpName = null;
            }
        }

        public void Execute(string commandLine)
        {
            var segments = CommandLineParser.Split(commandLine);

            var name = segments[0];
            var arguments = segments[1];

            if (File.Exists(name) == true)
                name = Process.GetCurrentProcess().ProcessName;

            if (this.VerifyName == true && this.Name != name)
                throw new ArgumentException(string.Format(Resources.InvalidCommandName_Format, name));

            this.Execute(CommandLineParser.Split(arguments));
        }

        public virtual bool IsCommandVisible(ICommand command)
        {
            if (this.HelpCommand == command)
                return false;
            if (this.VersionCommand == command)
                return false;
            if (this.parsers.Contains(command) == false)
                return false;
            var attr = command.GetType().GetCustomAttribute<BrowsableAttribute>();
            if (attr == null)
                return true;
            return attr.Browsable;
        }

        public virtual bool IsMethodVisible(ICommand command, CommandMethodDescriptor descriptor)
        {
            if (command is IExecutable == true)
                return false;
            if (this.IsCommandVisible(command) == false)
                return false;
            return true;
        }

        public TextWriter Out
        {
            get { return this.writer ?? Console.Out; }
            set
            {
                this.writer = value;
                foreach (var item in this.parsers)
                {
                    item.Out = value;
                }
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.name) == true)
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

        public CommandCollection Commands
        {
            get { return this.commands; }
        }

        public CommandLineParserCollection Parsers
        {
            get { return this.parsers; }
        }

        public virtual ICommand HelpCommand
        {
            get
            {
                if (this.helpCommand == null)
                    this.helpCommand = new HelpCommand(this);
                return this.helpCommand;
            }
        }

        public virtual ICommand VersionCommand
        {
            get
            {
                if (this.versionCommand == null)
                    this.versionCommand = new VersionCommand(this);
                return this.versionCommand;
            }
        }

        public bool VerifyName { get; set; }

        protected virtual CommandLineParser CreateInstance(ICommand command)
        {
            return new CommandLineParser(command.Name, command) { Out = this.Out, };
        }

        protected virtual bool OnExecute(ICommand command, string arguments)
        {
            var parser = this.parsers[command];
            if (command is IExecutable == false)
            {
                if (parser.Parse(command.Name + " " + arguments) == false)
                {
                    return false;
                }

                (command as IExecutable).Execute();
            }
            else
            {
                if (parser.Invoke(parser.Name + " " + arguments) == false)
                {
                    return false;
                }
            }
            return true;
        }

        private bool Execute(string[] args)
        {
            var commandName = args[0];
            var arguments = args[1];

            if (commandName == string.Empty)
            {
                this.Out.WriteLine("type '{0}' for usage.", string.Join(" ", new string[] { this.Name, this.HelpCommand.Name }.Where(i => i != string.Empty)));
                this.Out.WriteLine("type '{0}' to see the version.", string.Join(" ", new string[] { this.Name, this.VersionCommand.Name }.Where(i => i != string.Empty)));
                return false;
            }
            else if (commandName == this.HelpCommand.Name)
            {
                return this.OnExecute(this.HelpCommand, arguments);
            }
            else if (commandName == this.VersionCommand.Name)
            {
                return this.OnExecute(this.VersionCommand, arguments);
            }
            else if (this.commands.Contains(commandName) == true)
            {
                var command = this.commands[commandName];
                if (this.IsCommandVisible(command) == true)
                    return this.OnExecute(command, arguments);
            }

            throw new ArgumentException(string.Format("'{0}' does not exsited command.", commandName));
        }

        private CommandLineParser CreateInstance(CommandContextBase commandContext, ICommand command)
        {
            var parser = this.CreateInstance(command);
            parser.CommandContext = commandContext;
            return parser;
        }
    }
}
