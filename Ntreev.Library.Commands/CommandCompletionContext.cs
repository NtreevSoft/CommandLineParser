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

        public CommandCompletionContext(object command, IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args, string find)
        {
            var parser = new ParseDescriptor(members, args);
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
        }

        public CommandCompletionContext(object command, CommandMethodDescriptor MethodDescriptor, IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args, string find)
            : this(command, members, args, find)
        {
            this.MethodDescriptor = MethodDescriptor;
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

        public IReadOnlyDictionary<string, object> Properties
        {
            get { return this.properties; }
        }
    }
}
