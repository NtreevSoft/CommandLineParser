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
        //private readonly Dictionary<CommandMemberDescriptor, object> parsedDescriptors = new Dictionary<CommandMemberDescriptor, object>();
        //private readonly List<CommandMemberDescriptor> unparsedDescriptors = new List<CommandMemberDescriptor>();
        private readonly Dictionary<string, string> unparsedArguments = new Dictionary<string, string>();
        //private readonly List<CommandMemberDescriptor> descriptors = new List<CommandMemberDescriptor>();

        private readonly Dictionary<CommandMemberDescriptor, ParseDescriptorItem> itemList = new Dictionary<CommandMemberDescriptor, ParseDescriptorItem>();

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
            //this.itemList = new List<ParseDescriptorItem>(members.Select(item => new ParseDescriptorItem(item)));

            foreach (var item in members)
            {
                this.itemList.Add(item, new ParseDescriptorItem(item));
            }

            //this.descriptors = new List<CommandMemberDescriptor>(members);
            //this.unparsedDescriptors = new List<CommandMemberDescriptor>(members.Count());

            //foreach (var item in members)
            //{
            //    if (item.GetType() == type && item.IsRequired == true)
            //        this.unparsedDescriptors.Add(item);
            //}

            var descriptors = new Dictionary<string, CommandMemberDescriptor>();
            foreach (var item in members)
            {
                if (item is CommandMemberArrayDescriptor)
                    continue;
                if (item.GetType() == type && item.IsRequired == true && item.IsExplicit == false)
                    continue;
                if (item.NamePattern != string.Empty)
                    descriptors.Add(item.NamePattern, item);
                if (item.ShortNamePattern != string.Empty)
                    descriptors.Add(item.ShortNamePattern, item);
            }

            var variableList = new List<string>();
            //var requirements = members.Where(item => item.GetType() == type && item.IsRequired == true && item.IsExplicit == false).ToList();
            //var options = members.Where(item => (item.IsRequired == false || item.GetType() != type) && item is CommandMemberArrayDescriptor == false).ToList();
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

                    if (nextArg != null && isValue == true && descriptor.MemberType != typeof(bool))
                    {
                        var textValue = arguments.Dequeue();
                        if (CommandStringUtility.IsWrappedOfQuote(textValue) == true)
                            textValue = Regex.Unescape(textValue);
                        //this.parsedDescriptors.Add(descriptor, Parser.Parse(descriptor, textValue));
                        //this.unparsedDescriptors.Remove(descriptor);
                        this.itemList[descriptor].Desiredvalue = Parser.Parse(descriptor, textValue);
                    }
                    else if (descriptor.MemberType == typeof(bool))
                    {
                        //this.parsedDescriptors.Add(descriptor, true);
                        // this.unparsedDescriptors.Remove(descriptor);
                        this.itemList[descriptor].Desiredvalue = true;
                    }
                    //else if (descriptor.DefaultValue != DBNull.Value)
                    //{
                    //    throw new Exception();
                    //    //this.parsedDescriptors.Add(descriptor, descriptor.DefaultValue);
                    //}
                    else if (descriptor.IsExplicit == true && descriptor.DefaultValue != DBNull.Value)
                    {
                        //throw new Exception();
                        //if (descriptor.MemberType.IsValueType)
                        //{
                        //    this.parsedDescriptors.Add(descriptor, Activator.CreateInstance(descriptor.MemberType));
                        //}
                        //else
                        //{
                        //    this.parsedDescriptors.Add(descriptor, null);
                        //}
                        this.itemList[descriptor].Desiredvalue = descriptor.DefaultValue;
                    }
                    else
                    {
                        //this.unparsedDescriptors.Insert(0, descriptor);
                        return;
                    }
                    //options.Remove(descriptor);
                }
                //else if (requirements.Any() == false)
                //{
                //    if (variables != null)
                //    {
                //        variableList.Add(arg);
                //    }
                //    else
                //    {
                //        var nextArg = arguments.FirstOrDefault();
                //        if (nextArg != null && CommandStringUtility.IsSwitch(nextArg) == false)
                //            this.unparsedArguments.Add(arg, arguments.Dequeue());
                //        else
                //            this.unparsedArguments.Add(arg, null);
                //    }
                //}
                else if (CommandStringUtility.IsSwitch(arg) == false)
                {
                    var requiredDescriptor = this.itemList.Where(item => item.Key.GetType() == type && item.Key.IsRequired == true && item.Value.IsParsed == false)
                                                          .Select(item => item.Key).FirstOrDefault();
                    if (requiredDescriptor != null)
                    {
                        var parseInfo = this.itemList[requiredDescriptor];
                        var value = Parser.Parse(requiredDescriptor, arg);
                        parseInfo.Desiredvalue = value;
                    }
                    else if (variables != null)
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
                    //var descriptor = requirements.First();
                    //this.parsedDescriptors.Add(descriptor, Parser.Parse(descriptor, arg));
                    //requirements.Remove(descriptor);
                    //this.unparsedDescriptors.Remove(descriptor);

                }
                else
                {
                    this.unparsedArguments.Add(arg, null);
                }
            }

            //foreach (var item in requirements.ToArray())
            //{
            //    if (isInitializable == true && item.IsExplicit == false && item.DefaultValue != DBNull.Value)
            //    {
            //        this.parsedDescriptors.Add(item, item.DefaultValue);
            //        this.unparsedDescriptors.Remove(item);
            //        requirements.Remove(item);
            //    }
            //}

            //foreach (var item in options.ToArray())
            //{
            //    if (isInitializable == false || item.IsExplicit == true)
            //        continue;

            //    //if (item.DefaultValue != DBNull.Value)
            //    //{
            //    //    this.parsedDescriptors.Add(item, item.DefaultValue);
            //    //}
            //    //else if (item.MemberType.IsValueType == true)
            //    //{
            //    //    this.parsedDescriptors.Add(item, Activator.CreateInstance(item.MemberType));
            //    //}
            //    //else
            //    //{
            //    //    this.parsedDescriptors.Add(item, null);
            //    //}
            //}

            if (variables != null)
            {
                this.itemList[variables].Desiredvalue = Parser.ParseArray(variables, variableList);
                //this.parsedDescriptors.Add(variables, Parser.ParseArray(variables, variableList));
                //this.unparsedDescriptors.Remove(variables);
            }
        }

        public void SetValue(object instance)
        {
            this.ValidateSetValue(instance);
            //var descriptor = this.unparsedDescriptors.FirstOrDefault();
            //if (descriptor != null)
            //{
            //    if (descriptor.IsRequired == true)
            //    {
            //        if (descriptor.IsExplicit == false)
            //            throw new ArgumentException($"필수 인자 {descriptor.Name}가 빠져있습니다");
            //        else
            //            throw new ArgumentException($"필수 인자 {descriptor.DisplayPattern}가 빠져있습니다");
            //    }
            //    else
            //        throw new ArgumentException("처리되지 않은 인자가 포함되어 있습니다.");
            //}



            //var keyValues = new Dictionary<CommandMemberDescriptor, object>(this.descriptors.Count);
            //foreach (var item in this.descriptors)
            //{
            //    if (this.parsedDescriptors.ContainsKey(item) == true)
            //        keyValues.Add(item, this.parsedDescriptors[item]);
            //    else
            //        keyValues.Add(item, DBNull.Value);
            //}

            foreach (var item in this.itemList)
            {
                var descriptor = item.Key;
                descriptor.ValidateTrigger(this.itemList);
            }

            foreach (var item in this.itemList)
            {
                var descriptor = item.Key;
                var parseInfo = item.Value;

                if (parseInfo.Desiredvalue != DBNull.Value)
                {
                    descriptor.SetValueInternal(instance, parseInfo.Desiredvalue);
                }
                else if (parseInfo.DefaultValue != DBNull.Value && descriptor.IsExplicit == false)
                {
                    descriptor.SetValueInternal(instance, parseInfo.DefaultValue);
                }
                else if (descriptor.MemberType.IsValueType == true)
                {
                    descriptor.SetValueInternal(instance, Activator.CreateInstance(descriptor.MemberType));
                }
                else
                {
                    descriptor.SetValueInternal(instance, null);
                }
            }
        }

        public IReadOnlyDictionary<CommandMemberDescriptor, ParseDescriptorItem> Descriptors
        {
            get { return this.itemList; }
        }

        private void ValidateSetValue(object instance)
        {
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

            foreach (var item in this.itemList)
            {
                var descriptor = item.Key;
                var parseInfo = item.Value;
                if (parseInfo.IsParsed == true)
                    continue;

                if (descriptor.IsRequired == true && parseInfo.DefaultValue == DBNull.Value)
                {
                    if (descriptor.IsExplicit == false)
                        throw new ArgumentException($"필수 인자 {descriptor.DisplayName}가 빠져있습니다");
                    else
                        throw new ArgumentException($"필수 인자 {descriptor.DisplayPattern}가 빠져있습니다");
                }
            }
        }
    }
}
