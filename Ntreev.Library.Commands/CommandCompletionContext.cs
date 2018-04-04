//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
            this.Find = find;
            this.arguments = args.ToArray();

            foreach (var item in parser.Descriptors)
            {
                var descriptor = item.Key;
                var parseInfo = item.Value;

                if (parseInfo.IsParsed == true)
                {
                    this.properties.Add(descriptor.DescriptorName, parseInfo.Desiredvalue);
                }
                else if (this.MemberDescriptor == null && descriptor is CommandMemberArrayDescriptor == false)
                {
                    this.MemberDescriptor = descriptor;
                }
            }
        }

        public CommandCompletionContext(object command, CommandMethodDescriptor methodDescriptor, IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args, string find)
        {
            var parser = new ParseDescriptor(typeof(CommandParameterDescriptor), members, args, false);
            this.Command = command;
            this.MethodDescriptor = methodDescriptor;
            this.Find = find;
            this.arguments = args.ToArray();

            foreach (var item in parser.Descriptors)
            {
                var descriptor = item.Key;
                var parseInfo = item.Value;

                if (parseInfo.IsParsed == true)
                {
                    this.properties.Add(descriptor.DescriptorName, parseInfo.Desiredvalue);
                }
                else if (this.MemberDescriptor == null && descriptor is CommandMemberArrayDescriptor == false)
                {
                    this.MemberDescriptor = descriptor;
                }
            }
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
