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
    public abstract class CommandContextBase
    {
        private readonly static TextWriter defaultWriter = new ConsoleTextWriter();
        private readonly CommandLineParserCollection parsers = new CommandLineParserCollection();
        private readonly CommandCollection commands = new CommandCollection();
        private readonly ICommandProvider[] commandProviders;
        private string name;
        private Version version;
        private TextWriter writer;
        private TextWriter errorWriter;
        private ICommand helpCommand;
        private ICommand versionCommand;

        protected CommandContextBase(IEnumerable<ICommand> commands)
            : this(commands, new ICommandProvider[] { })
        {

        }

        protected CommandContextBase(IEnumerable<ICommand> commands, IEnumerable<ICommandProvider> commandProviders)
        {
            this.VerifyName = true;
            this.Out = defaultWriter;
            this.commandProviders = commandProviders.ToArray();

            foreach (var item in commands)
            {
                if (CommandSettings.IsConsoleMode == false && item.GetType().GetCustomAttribute<ConsoleModeOnlyAttribute>() != null)
                    continue;
                this.commands.Add(item);
                this.parsers.Add(item, this.CreateInstance(this, item));
            }
            this.parsers.Add(this.HelpCommand, this.CreateInstance(this, this.HelpCommand));
            this.parsers.Add(this.VersionCommand, this.CreateInstance(this, this.versionCommand));

            foreach (var item in commandProviders)
            {
                if (CommandSettings.IsConsoleMode == false && item.GetType().GetCustomAttribute<ConsoleModeOnlyAttribute>() != null)
                    continue;
                var command = commands.FirstOrDefault(i => i.Name == item.CommandName);
                if (command == null)
                    throw new CommandNotFoundException(item.CommandName);

                var descriptors = CommandDescriptor.GetMethodDescriptors(command);
                descriptors.AddRange(this.GetExternalMethodDescriptors(item));
            }

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

        public virtual bool IsCommandEnabled(ICommand command)
        {
            if (this.HelpCommand == command)
                return false;
            if (this.VersionCommand == command)
                return false;
            if (this.parsers.Contains(command) == false)
                return false;
            return command.IsEnabled;
        }

        public virtual bool IsMethodEnabled(ICommand command, CommandMethodDescriptor descriptor)
        {
            if (command is IExecutable == true)
                return false;
            if (this.IsCommandEnabled(command) == false)
                return false;
            if (command is CommandMethodBase commandMethod)
                return commandMethod.InvokeIsMethodEnabled(descriptor);
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

        public TextWriter Error
        {
            get { return this.errorWriter ?? Console.Error; }
            set
            {
                this.errorWriter = value;
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

        public ICommandProvider[] CommandProviders
        {
            get { return this.commandProviders; }
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
            if (command is IExecutable == true)
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
                this.Out.WriteLine(Resources.HelpMessage_Format, string.Join(" ", new string[] { this.HelpCommand.Name }.Where(i => i != string.Empty)));
                this.Out.WriteLine(Resources.VersionMessage_Format, string.Join(" ", new string[] { this.VersionCommand.Name }.Where(i => i != string.Empty)));
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
                if (this.IsCommandEnabled(command) == true)
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

        private IEnumerable<CommandMethodDescriptor> GetExternalMethodDescriptors(ICommandProvider commandProvider)
        {
            foreach (var item in CommandDescriptor.GetMethodDescriptors(commandProvider))
            {
                yield return new ExternalCommandMethodDescriptor(commandProvider, item);
            }
        }

        #region classes

        class ConsoleTextWriter : TextWriter
        {
            public override Encoding Encoding
            {
                get { return Console.OutputEncoding; }
            }

            public override void Write(char value)
            {
                Console.Write(value);
            }

            public override void Write(string value)
            {
                Console.Write(value);
            }

            public override void WriteLine(string value)
            {
                Console.WriteLine(value);
            }
        }

        #endregion
    }
}
