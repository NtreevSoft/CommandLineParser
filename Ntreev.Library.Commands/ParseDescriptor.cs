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
        private readonly CommandMemberDescriptor[] members;
        private readonly Dictionary<CommandMemberDescriptor, object> args = new Dictionary<CommandMemberDescriptor, object>();
        private readonly Dictionary<string, CommandMemberDescriptor> descriptors = new Dictionary<string, CommandMemberDescriptor>();

        public ParseDescriptor(IEnumerable<CommandMemberDescriptor> members)
        {
            this.members = members.ToArray();
            foreach (var item in this.members)
            {
                if (item.IsRequired == true)
                    continue;
                if (item.NamePattern != string.Empty)
                    descriptors.Add(item.NamePattern, item);
                if (item.ShortNamePattern != string.Empty)
                    descriptors.Add(item.ShortNamePattern, item);
            }
        }

        public void Parse(object instance, string commandLine)
        {
            var requirements = this.members.Where(item => item.IsRequired == true).ToList();
            var options = this.members.Where(item => item.IsRequired == false).ToList();
            var variables = this.members.Where(item => item is CommandMemberArrayDescriptor).FirstOrDefault();

            var variableList = new List<string>();
            var arguments = new Queue<string>(CommandLineParser.SplitAll(commandLine));

            while (arguments.Any())
            {
                var arg = arguments.Dequeue();

                if (this.descriptors.ContainsKey(arg) == true)
                {
                    var descriptor = this.descriptors[arg];
                    var nextArg = arguments.FirstOrDefault();
                    var isValue = nextArg != null ? !Regex.IsMatch(nextArg, $"{CommandSettings.Delimiter}\\S+|{CommandSettings.ShortDelimiter}\\S+") : false;

                    if (nextArg != null && isValue == true)
                    {
                        this.args.Add(descriptor, Parser.Parse(descriptor, arguments.Dequeue()));
                    }
                    else if (descriptor.DefaultValue != DBNull.Value)
                    {
                        this.args.Add(descriptor, descriptor.DefaultValue);
                    }
                    else
                    {
                        throw new ArgumentException(@"처리되지 않은 인자가 포함되어 있습니다.");
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
                        throw new ArgumentException("처리되지 않은 인자가 포함되어 있습니다.");
                    }
                }
                else
                {
                    var descriptor = requirements.First();
                    this.args.Add(descriptor, Parser.Parse(descriptor, arg));
                    requirements.Remove(descriptor);
                }
            }

            foreach (var item in requirements.ToArray())
            {
                if (item.DefaultValue != DBNull.Value)
                {
                    this.args.Add(item, item.DefaultValue);
                    requirements.Remove(item);
                }
            }

            foreach (var item in options.ToArray())
            {
                if (item.IsImplicit == true && item.DefaultValue != DBNull.Value)
                {
                    this.args.Add(item, item.DefaultValue);
                    options.Remove(item);
                }
            }

            if (requirements.Count > 0)
            {
                throw new ArgumentException(string.Format("필수 인자 {0}가 빠져있습니다", requirements.First().Name));
            }

            foreach (var item in this.args)
            {
                item.Key.SetValueInternal(instance, item.Value);
            }

            if (variables != null)
            {
                variables.SetValueInternal(instance, Parser.ParseArray(variables, variableList));
            }
        }
    }
}
