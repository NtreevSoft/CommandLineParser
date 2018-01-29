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
        private readonly Dictionary<string, string> unparsedArguments = new Dictionary<string, string>();

        /// <param name="type">
        /// 스위치를 직접 명시하지 않아도 되는 타입
        /// </param>
        /// <param name="members"></param>
        /// <param name="commandLine"></param>
        /// <param name="isInitializable"></param>
        public ParseDescriptor(Type type, IEnumerable<CommandMemberDescriptor> members, string commandLine, bool isInitializable)
            : this(type, members, CommandStringUtility.SplitAll(commandLine), isInitializable)
        {


        }

        public ParseDescriptor(Type type, IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args)
            : this(type, members, args, true)
        {

        }

        public ParseDescriptor(Type type, IEnumerable<CommandMemberDescriptor> members, IEnumerable<string> args, bool isInitializable)
        {
            this.unparsedDescriptors = new List<CommandMemberDescriptor>(members.Where(item => item.GetType() == type && item.IsRequired == true));

            var descriptors = new Dictionary<string, CommandMemberDescriptor>();
            foreach (var item in members)
            {
                if ((item.GetType() == type && item.IsRequired == true) || item is CommandMemberArrayDescriptor)
                    continue;
                if (item.NamePattern != string.Empty)
                    descriptors.Add(item.NamePattern, item);
                if (item.ShortNamePattern != string.Empty)
                    descriptors.Add(item.ShortNamePattern, item);
            }
            var variableList = new List<string>();
            var requirements = members.Where(item => item.GetType() == type && item.IsRequired == true).ToList();
            var options = members.Where(item => (item.IsRequired == false || item.GetType() != type) && item is CommandMemberArrayDescriptor == false).ToList();
            var variables = members.Where(item => item is CommandMemberArrayDescriptor).FirstOrDefault();

            var arguments = new Queue<string>(args);

            while (arguments.Any())
            {
                var arg = arguments.Dequeue();

                if (descriptors.ContainsKey(arg) == true)
                {
                    var descriptor = descriptors[arg];
                    var nextArg = arguments.FirstOrDefault();
                    var isValue = CommandStringUtility.IsSwitch(nextArg) == false;

                    if (nextArg != null && isValue == true)
                    {
                        var textValue = arguments.Dequeue();
                        if (CommandStringUtility.IsWrappedOfQuote(textValue) == true)
                            textValue = Regex.Unescape(textValue);
                        this.parsedDescriptors.Add(descriptor, Parser.Parse(descriptor, textValue));
                    }
                    //else if (descriptor.MemberType == typeof(bool))
                    //{
                    //    this.parsedDescriptors.Add(descriptor, true);
                    //}
                    else if (descriptor.DefaultValue != DBNull.Value)
                    {
                        this.parsedDescriptors.Add(descriptor, descriptor.DefaultValue);
                    }
                    else if (descriptor.IsImplicit == true)
                    {
                        if (descriptor.MemberType.IsValueType)
                        {
                            this.parsedDescriptors.Add(descriptor, Activator.CreateInstance(descriptor.MemberType));
                        }
                        else
                        {
                            this.parsedDescriptors.Add(descriptor, null);
                        }
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
                        var nextArg = arguments.FirstOrDefault();
                        if (nextArg != null && CommandStringUtility.IsSwitch(nextArg) == false)
                            this.unparsedArguments.Add(arg, arguments.Dequeue());
                        else
                            this.unparsedArguments.Add(arg, null);
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
                if (isInitializable == false || item.IsImplicit == true)
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
                var items = new Dictionary<string, string>(this.unparsedArguments);
                if (instance is IUnknownArgument parser)
                {
                    foreach (var item in this.unparsedArguments)
                    {
                        if (parser.Parse(item.Key, item.Value) == true)
                        {
                            items.Remove(item.Key);
                        }
                    }
                }

                if (items.Any() == true)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("처리되지 않은 인자가 포함되어 있습니다.");
                    foreach (var item in items)
                    {
                        if (item.Value != null)
                        {
                            sb.AppendLine($"    {item.Key} {item.Value}");
                        }
                        else
                        {
                            sb.AppendLine($"    {item.Key}");
                        }
                    }
                    throw new ArgumentException(sb.ToString());
                }
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
            get { return this.unparsedArguments.Keys.ToArray(); }
        }
    }
}
