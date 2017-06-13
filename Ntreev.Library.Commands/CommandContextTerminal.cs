using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class CommandContextTerminal : Terminal
    {
        private readonly CommandContextBase commandContext;
        private bool isCancellationRequested;
        private string prompt;
        private string prefix;
        private string postfix;

        public CommandContextTerminal(CommandContextBase commandContext)
        {
            this.commandContext = commandContext;
        }

        public new string Prompt
        {
            get { return this.prompt ?? string.Empty; }
            set
            {
                this.prompt = value;
                if (this.IsReading == true)
                {
                    this.SetPrompt(this.Prefix + this.Prompt + this.Postfix);
                }
            }
        }

        public string Prefix
        {
            get { return this.prefix ?? string.Empty; }
            set
            {
                this.prefix = value;
                if (this.IsReading == true)
                {
                    this.SetPrompt(this.Prefix + this.Prompt + this.Postfix);
                }
            }
        }

        public string Postfix
        {
            get { return this.postfix ?? string.Empty; }
            set
            {
                this.postfix = value;
                if (this.IsReading == true)
                {
                    this.SetPrompt(this.Prefix + this.Prompt + this.Postfix);
                }
            }
        }

        public void Cancel()
        {
            this.isCancellationRequested = true;
        }

        public void Start()
        {
            string line;
            while ((line = this.ReadString(this.Prefix + this.Prompt + this.Postfix)) != null)
            {
                try
                {
                    this.commandContext.Execute(this.commandContext.Name + " " + line);
                }
                catch (TargetInvocationException e)
                {
                    if (this.DetailErrorMessage == true)
                    {
                        this.commandContext.Error.WriteLine(e);
                    }
                    else
                    {
                        if (e.InnerException != null)
                            this.commandContext.Error.WriteLine(e.InnerException.Message);
                        else
                            this.commandContext.Error.WriteLine(e.Message);
                    }
                }
                catch (Exception e)
                {
                    if (this.DetailErrorMessage == true)
                    {
                        this.commandContext.Error.WriteLine(e);
                    }
                    else
                    {
                        this.commandContext.Error.WriteLine(e.Message);
                    }
                }
                if (this.IsCancellationRequested == true)
                    break;
            }
        }

        public bool IsCancellationRequested
        {
            get { return this.isCancellationRequested; }
        }

        public bool DetailErrorMessage
        {
            get; set;
        }

        protected override string[] GetCompletion(string[] items, string find)
        {
            if (items.Length == 0)
            {
                var query = from item in this.commandContext.Commands
                            let name = item.Name
                            where this.commandContext.IsCommandVisible(item)
                            where name.StartsWith(find)
                            select name;
                return query.ToArray();
            }
            else if (items.Length == 1)
            {
                var commandName = items[0];
                var command = this.GetCommand(commandName);
                if (command == null)
                    return null;
                if (command is IExecutable == false)
                {
                    var query = from item in CommandDescriptor.GetMethodDescriptors(command)
                                let name = item.Name
                                where this.commandContext.IsMethodVisible(command, item)
                                where name.StartsWith(find)
                                select name;
                    return query.ToArray();
                }
                else
                {
                    var memberList = new List<CommandMemberDescriptor>(CommandDescriptor.GetMemberDescriptors(command));
                    var argList = new List<string>(items.Skip(1));
                    var completionContext = new CommandCompletionContext(command, memberList, argList, find);
                    return this.GetCompletions(completionContext);
                    //if (argList.Any())
                    //{
                    //    var arg = argList.Last();
                    //    var descriptor = this.FindMemberDescriptor(memberList, arg);
                    //    if (descriptor != null)
                    //    {
                    //        return this.GetCompletions(command, descriptor, find);
                    //    }
                    //}

                    //for (var i = 0; i < argList.Count; i++)
                    //{
                    //    if (memberList.Any() == false)
                    //        break;
                    //    var arg = argList[i];
                    //    var member = memberList.First();
                    //    if (member.IsRequired == true)
                    //        memberList.RemoveAt(0);
                    //}

                    //if (memberList.Any() == true && memberList.First().IsRequired == true)
                    //{
                    //    int qwer = 0;
                    //    //var methodInfo = methodDescriptor.MethodInfo;
                    //    //if (methodInfo.DeclaringType == command.GetType())
                    //    //    return this.GetCompletions(command, methodDescriptor, memberList.First(), find);
                    //    //var commandProvider = this.commandContext.CommandProviders.First(item => item.CommandName == command.Name && item.GetType() == methodInfo.DeclaringType);
                    //    //return this.GetCompletions(commandProvider, methodDescriptor, memberList.First(), find);
                    //}
                }
            }
            else if (items.Length >= 2)
            {
                var commandName = items[0];
                var command = this.GetCommand(commandName);
                if (command == null)
                    return null;

                if (command is IExecutable == true)
                {
                    var memberList = new List<CommandMemberDescriptor>(CommandDescriptor.GetMemberDescriptors(command));
                    var argList = new List<string>(items.Skip(1));
                    var completionContext = new CommandCompletionContext(command, memberList, argList, find);
                    //completionContext.MemberDescriptor = this.FindMemberDescriptor(argList, memberList);
                    //var parser = new ParseDescriptor(memberList, argList.ToArray());
                    //completionContext.MemberDescriptor = parser.UnparsedDescriptors.First();
                    //completionContext.Properties = parser.p
                    return this.GetCompletions(completionContext);
                }
                else
                {
                    var methodName = items[1];
                    var methodDescriptor = this.GetMethodDescriptor(command, methodName);
                    if (methodDescriptor == null)
                        return null;

                    if (methodDescriptor.Members.Length == 0)
                        return null;

                    var commandTarget = this.GetCommandTarget(command, methodDescriptor);
                    var memberList = new List<CommandMemberDescriptor>(methodDescriptor.Members);
                    var argList = new List<string>(items.Skip(2));
                    var completionContext = new CommandCompletionContext(command, methodDescriptor, memberList, argList, find);
                    //completionContext.MemberDescriptor = this.FindMemberDescriptor(argList, memberList);
                    return this.GetCompletions(completionContext);

                    //if (argList.Any())
                    //{
                    //    var arg = argList.Last();
                    //    var descriptor = this.FindMemberDescriptor(memberList, arg);
                    //    if (descriptor != null)
                    //    {
                    //        var methodInfo = methodDescriptor.MethodInfo;
                    //        if (methodInfo.DeclaringType == command.GetType())
                    //            return this.GetCompletions(command, methodDescriptor, descriptor, find);
                    //        var commandProvider = this.commandContext.CommandProviders.First(item => item.CommandName == command.Name && item.GetType() == methodInfo.DeclaringType);
                    //        return this.GetCompletions(commandProvider, methodDescriptor, descriptor, find);
                    //    }
                    //}

                    //for (var i = 0; i < argList.Count; i++)
                    //{
                    //    if (memberList.Any() == false)
                    //        break;
                    //    var arg = argList[i];
                    //    var member = memberList.First();
                    //    if (member.IsRequired == true)
                    //        memberList.RemoveAt(0);
                    //}

                    //if (memberList.Any() == true && memberList.First().IsRequired == true)
                    //{
                    //    var methodInfo = methodDescriptor.MethodInfo;
                    //    if (methodInfo.DeclaringType == command.GetType())
                    //        return this.GetCompletions(command, methodDescriptor, memberList.First(), find);
                    //    var commandProvider = this.commandContext.CommandProviders.First(item => item.CommandName == command.Name && item.GetType() == methodInfo.DeclaringType);
                    //    return this.GetCompletions(commandProvider, methodDescriptor, memberList.First(), find);
                    //}
                }
            }

            return null;
        }

        //protected virtual string[] GetCompletions(ICommand command, string methodName, string find)
        //{
        //    return null;
        //}

        //protected virtual string[] GetCompletions(ICommand command, CommandMemberDescriptor memberDescriptor, string find)
        //{
        //    return null;
        //}

        //protected virtual string[] GetCompletions(ICommand command, CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        //{
        //    return null;
        //}

        //protected virtual string[] GetCompletions(ICommandProvider commandProvider, CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        //{
        //    return null;
        //}

        protected virtual string[] GetCompletions(CommandCompletionContext completionContext)
        {
            return null;
        }

        private CommandMemberDescriptor FindMemberDescriptor(List<string> argList, List<CommandMemberDescriptor> memberList)
        {
            if (argList.Any())
            {
                var arg = argList.Last();
                var descriptor = this.FindMemberDescriptor(memberList, arg);
                if (descriptor != null)
                {
                    return descriptor;
                }
            }

            for (var i = 0; i < argList.Count; i++)
            {
                if (memberList.Any() == false)
                    break;
                var arg = argList[i];
                var member = memberList.First();
                if (member.IsRequired == true)
                    memberList.RemoveAt(0);
            }

            if (memberList.Any() == true && memberList.First().IsRequired == true)
            {
                return memberList.First();
            }
            return null;
        }

        private ICommand GetCommand(string commandName)
        {
            var commandNames = this.commandContext.Commands.Select(item => item.Name).ToArray();
            if (commandNames.Contains(commandName) == true)
            {
                var command = this.commandContext.Commands[commandName];
                if (this.commandContext.IsCommandVisible(command) == true)
                    return command;
            }
            return null;
        }

        private CommandMethodDescriptor GetMethodDescriptor(ICommand command, string methodName)
        {
            if (command is IExecutable == true)
                return null;
            var descriptors = CommandDescriptor.GetMethodDescriptors(command);
            if (descriptors.Contains(methodName) == false)
                return null;
            var descriptor = descriptors[methodName];
            if (this.commandContext.IsMethodVisible(command, descriptor) == false)
                return null;
            return descriptor;
        }

        private CommandMemberDescriptor FindMemberDescriptor(IEnumerable<CommandMemberDescriptor> descriptors, string argument)
        {
            foreach (var item in descriptors)
            {
                if (item.NamePattern == argument || item.ShortNamePattern == argument)
                {
                    return item;
                }
            }
            return null;
        }

        private object GetCommandTarget(ICommand command, CommandMethodDescriptor methodDescriptor)
        {
            var methodInfo = methodDescriptor.MethodInfo;
            if (methodInfo.DeclaringType == command.GetType())
                return command;
            var query = from item in this.commandContext.CommandProviders
                        where item.CommandName == command.Name
                        where item.GetType() == methodInfo.DeclaringType
                        select item;

            return query.First();
        }
    }
}
