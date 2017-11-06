using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class CommandCompletionContext
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();
        private readonly string[] arguments;

        public CommandCompletionContext(object command, IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args, string find)
        {
            var parser = new ParseDescriptor(typeof(CommandPropertyDescriptor), members, args);
            this.Command = command;
            this.MemberDescriptor = parser.UnparsedDescriptors.FirstOrDefault();
            this.Find = find;
            foreach (var item in parser.ParsedDescriptors)
            {
                this.properties.Add(item.Key.DescriptorName, item.Value);
            }
            if (this.MemberDescriptor == null)
            {
                this.MemberDescriptor = members.FirstOrDefault(item => item is CommandMemberArrayDescriptor);
            }
            this.arguments = args.ToArray();
        }

        public CommandCompletionContext(object command, CommandMethodDescriptor methodDescriptor, IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args, string find)
        {
            var parser = new ParseDescriptor(typeof(CommandParameterDescriptor), members, args);
            this.Command = command;
            this.MethodDescriptor = methodDescriptor;
            this.MemberDescriptor = parser.UnparsedDescriptors.FirstOrDefault();
            this.Find = find;
            foreach (var item in parser.ParsedDescriptors)
            {
                this.properties.Add(item.Key.DescriptorName, item.Value);
            }
            if (this.MemberDescriptor == null)
            {
                this.MemberDescriptor = members.FirstOrDefault(item => item is CommandMemberArrayDescriptor);
            }
            this.arguments = args.ToArray();
        }

        public object Command
        {
            get;
            private set;
        }

        public CommandMethodDescriptor MethodDescriptor
        {
            get;
            private set;
        }

        public CommandMemberDescriptor MemberDescriptor
        {
            get;
            private set;
        }

        public string Find
        {
            get;
            private set;
        }

        public string[] Arguments
        {
            get { return this.arguments; }
        }

        public IReadOnlyDictionary<string, object> Properties
        {
            get { return this.properties; }
        }
    }
}
