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
                this.parsers.Add(item, this.CreateInstance(item));
            }
            this.parsers.Add(this.HelpCommand, this.CreateInstance(this.HelpCommand));
            this.parsers.Add(this.VersionCommand, this.CreateInstance(this.versionCommand));

            foreach (var item in this.parsers)
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
                throw new ArgumentException(string.Format(Resources.InvalidCommandName_Format, name));

            this.Execute(CommandLineParser.Split(arguments));
        }

        public virtual bool IsCommandVisible(ICommand command)
        {
            if (this.HelpCommand == command)
                return false;
            if (this.VersionCommand == command)
                return false;
            var attr = command.GetType().GetCustomAttribute<BrowsableAttribute>();
            if (attr == null)
                return true;
            return attr.Browsable;
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
                        this.Out.WriteLine(Resources.TypeForUsage_Format, string.Join(" ", this.Name, this.HelpCommand.Name, command.Name).Trim());
                        return false;
                    }
                }
                else if (parser.Invoke(parser.Name + " " + arguments) == false)
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
                        this.Out.WriteLine(Resources.TypeForUsage_Format, string.Join(" ", this.Name, this.HelpCommand.Name, command.Name).Trim());
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
    }
}
