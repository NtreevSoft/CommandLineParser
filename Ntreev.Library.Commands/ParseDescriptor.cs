#region License
//Ntreev CommandLineParser for .Net 1.0.4548.25168
//https://github.com/NtreevSoft/CommandLineParser

//Released under the MIT License.

//Copyright (c) 2010 Ntreev Soft co., Ltd.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Text.RegularExpressions;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    class ParseDescriptor
    {
        private readonly Dictionary<CommandMemberDescriptor, object> parsedDescriptors = new Dictionary<CommandMemberDescriptor, object>();
        private readonly List<CommandMemberDescriptor> unparsedDescriptors = new List<CommandMemberDescriptor>();
        private readonly List<string> unparsedArguments = new List<string>();

        public ParseDescriptor(IEnumerable<CommandMemberDescriptor> members, string commandLine, bool isInitializable)
            : this(members, CommandLineParser.SplitAll(commandLine), isInitializable)
        {


        }

        public ParseDescriptor(IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args)
            : this(members, args, true)
        {

        }

        public ParseDescriptor(IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args, bool isInitializable)
        {
            this.unparsedDescriptors = new List<CommandMemberDescriptor>(members.Where(item => item.IsRequired));

            var descriptors = new Dictionary<string, CommandMemberDescriptor>();
            foreach (var item in members)
            {
                if (item.IsRequired == true || item is CommandMemberArrayDescriptor)
                    continue;
                if (item.NamePattern != string.Empty)
                    descriptors.Add(item.NamePattern, item);
                if (item.ShortNamePattern != string.Empty)
                    descriptors.Add(item.ShortNamePattern, item);
            }
            var variableList = new List<string>();
            var requirements = members.Where(item => item.IsRequired == true).ToList();
            var options = members.Where(item => item.IsRequired == false && item is CommandMemberArrayDescriptor == false).ToList();
            var variables = members.Where(item => item is CommandMemberArrayDescriptor).FirstOrDefault();

            var arguments = new Queue<string>(args);

            while (arguments.Any())
            {
                var arg = arguments.Dequeue();

                if (descriptors.ContainsKey(arg) == true)
                {
                    var descriptor = descriptors[arg];
                    var nextArg = arguments.FirstOrDefault();
                    var isValue = CommandLineParser.IsSwitch(nextArg) == false;

                    if (nextArg != null && isValue == true && descriptor.IsToggle == false)
                    {
                        this.parsedDescriptors.Add(descriptor, Parser.Parse(descriptor, arguments.Dequeue()));
                    }
                    else if (descriptor.DefaultValue != DBNull.Value)
                    {
                        this.parsedDescriptors.Add(descriptor, descriptor.DefaultValue);
                    }
                    else if (descriptor.MemberType == typeof(bool))
                    {
                        this.parsedDescriptors.Add(descriptor, true);
                    }
                    else
                    {
                        this.unparsedDescriptors.Insert(0, descriptor);
                        return;
                    }
                    options.Remove(descriptor);

                }
                else if (requirements.Any() == false)
                {
                    if (variables != null)
                    {
                        variableList.Add(arg);
                    }
                    else
                    {
                        this.unparsedArguments.Add(arg);
                        return;
                    }
                }
                else
                {
                    var descriptor = requirements.First();
                    this.parsedDescriptors.Add(descriptor, Parser.Parse(descriptor, arg));
                    requirements.Remove(descriptor);
                    this.unparsedDescriptors.Remove(descriptor);
                }
            }

            foreach (var item in requirements.ToArray())
            {
                if (item.DefaultValue != DBNull.Value)
                {
                    this.parsedDescriptors.Add(item, item.DefaultValue);
                    this.unparsedDescriptors.Remove(item);
                    requirements.Remove(item);
                }
            }

            foreach (var item in options.ToArray())
            {
                if (isInitializable == false)
                    continue;

                if (item.DefaultValue != DBNull.Value)
                {
                    this.parsedDescriptors.Add(item, item.DefaultValue);
                }
                else if (item.MemberType.IsValueType == true)
                {
                    this.parsedDescriptors.Add(item, Activator.CreateInstance(item.MemberType));
                }
                else
                {
                    this.parsedDescriptors.Add(item, null);
                }
            }

            if (variables != null)
            {
                this.parsedDescriptors.Add(variables, Parser.ParseArray(variables, variableList));
                this.unparsedDescriptors.Remove(variables);
            }
        }

        public void SetValue(object instance)
        {
            var descriptor = this.unparsedDescriptors.FirstOrDefault();
            if (descriptor != null)
            {
                if (descriptor.IsRequired == true)
                    throw new ArgumentException($"필수 인자 {descriptor.Name}가 빠져있습니다");
                else
                    throw new ArgumentException("처리되지 않은 인자가 포함되어 있습니다.");
            }

            if (this.unparsedArguments.Any())
            {
                throw new ArgumentException($"처리되지 않은 인자가 포함되어 있습니다. : {this.unparsedArguments.First()}");
            }

            foreach (var item in this.parsedDescriptors)
            {
                item.Key.SetValueInternal(instance, item.Value);
            }
        }

        public IReadOnlyDictionary<CommandMemberDescriptor, object> ParsedDescriptors
        {
            get { return this.parsedDescriptors; }
        }

        public CommandMemberDescriptor[] UnparsedDescriptors
        {
            get { return this.unparsedDescriptors.ToArray(); }
        }

        public string[] UnparsedArguments
        {
            get { return this.unparsedArguments.ToArray(); }
        }
    }
}
