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
    class SwitchHelper
    {
        private readonly CommandMemberDescriptor[] members;
        private readonly Dictionary<CommandMemberDescriptor, string> args = new Dictionary<CommandMemberDescriptor, string>();
        private readonly Dictionary<string, CommandMemberDescriptor> descriptors = new Dictionary<string, CommandMemberDescriptor>();

        public SwitchHelper(IEnumerable<CommandMemberDescriptor> members)
        {
            this.members = members.ToArray();
            foreach (var item in this.members)
            {
                if (item.Required == true)
                    continue;
                if (item.NamePattern != string.Empty)
                    descriptors.Add(item.NamePattern, item);
                if (item.ShortNamePattern != string.Empty)
                    descriptors.Add(item.ShortNamePattern, item);
            }
        }

        public void Parse(object instance, string commandLine)
        {
            var requirements = this.members.Where(item => item.Required == true).ToList();
            var options = this.members.Where(item => item.Required == false).ToList();
            var variables = this.members.Where(item => item is CommandPropertyArrayDescriptor).FirstOrDefault();
            var variableList = new List<string>();

            var ss1 = CommandLineParser.SplitAll(commandLine);

            var line = commandLine;
            while (ss1.Any())
            {
                var nam = ss1.First();

                if (descriptors.ContainsKey(nam) == true)
                {
                    var descriptor = descriptors[nam];
                    ss1.RemoveAt(0);
                    descriptor.Parse(instance, ss1);
                }
                else
                {
                    var descriptor = requirements.First();
                    descriptor.Parse(instance, ss1);
                    requirements.RemoveAt(0);
                }
                //if (line.StartsWith(CommandSettings.SwitchDelimiter) == true || line.StartsWith(CommandSettings.ShortSwitchDelimiter))
                //{
                //    var descriptor = this.ParseOption(instance, ref line);
                //    if (descriptor.Required == true)
                //        requirements.Remove(descriptor);
                //    else
                //        options.Remove(descriptor);
                //}
                //else
                //{
                //    if (requirements.Count == 0 && variables == null)
                //    {
                //        throw new ArgumentException("필수 인자가 너무 많이 포함되어 있습니다.");
                //    }
                //    line = this.ParseArguments(requirements.First(), line);
                //    requirements.RemoveAt(0);
                //}
            }

            foreach (var item in requirements.ToArray())
            {
                if (item.DefaultValue != DBNull.Value)
                {
                    item.SetValue(instance, item.DefaultValue);
                    requirements.Remove(item);
                }
            }

            if (requirements.Count > 0)
            {
                throw new ArgumentException(string.Format("필수 인자 {0}가 빠져있습니다", requirements.First().Name));
            }

            this.SetValues(instance);

            foreach (var item in options)
            {
                if (item.DefaultValue != DBNull.Value)
                {
                    item.SetValue(instance, item.DefaultValue);
                }
            }
        }

        public void SetValues(object instance)
        {
            foreach (var item in this.members)
            {
                if (this.args.ContainsKey(item) == false)
                    continue;
                var value = item.Parse(instance, this.args[item].Trim('\"'));
                item.SetValue(instance, value);
            }
        }

        private CommandMemberDescriptor ParseOption(object instance, ref string arguments)
        {
            var pattern = string.Format(@"^{0}\S+((\s+""[^""]*"")|(\s+[\S-[{0}]][\S]*)|(\s*))", CommandSettings.SwitchDelimiter);
            var shortPattern = string.Format(@"^{0}\S+((\s+""[^""]*"")|(\s+[\S-[{0}]][\S]*)|(\s*))", CommandSettings.ShortSwitchDelimiter);

            var match = Regex.Match(arguments, pattern);
            var parsed = string.Empty;

            if (match.Success == true)
            {
                var descriptor = this.DoMatch(match.Value, ref parsed);
                if (descriptor != null)
                {
                    arguments = arguments.Substring(parsed.Length).Trim();
                    return descriptor;
                }
            }

            var shortMatch = Regex.Match(arguments, shortPattern);

            if (shortMatch.Success == true)
            {
                var descriptor = this.DoMatch(shortMatch.Value, ref parsed);
                if (descriptor != null)
                {
                    arguments = arguments.Substring(parsed.Length).Trim();
                    return descriptor;
                }
            }

            throw new ArgumentException("확인할 수 없는 인자가 포함되어 있습니다.");
        }

        private string ParseArguments(CommandMemberDescriptor descriptor, string arguments)
        {
            var pattern = @"^((""[^""]*"")|(\S+))";

            var match = Regex.Match(arguments, pattern);

            if (match.Success == true)
            {
                this.args.Add(descriptor, match.Value);
                return arguments.Substring(match.Length).Trim();
            }

            throw new Exception();
        }

        private CommandMemberDescriptor DoMatch(string line, ref string parsed)
        {
            foreach (var item in this.members)
            {
                var arg = item.TryMatch(line, ref parsed);
                if (arg != null)
                {
                    if (this.args.ContainsKey(item) == true)
                        throw new ArgumentException("이름이 같은 선택 인자가 두번 설정되었습니다.");
                    this.args.Add(item, arg);
                    return item;
                }
            }

            return null;
        }

        private void AssertRequired()
        {
            foreach (var item in this.members)
            {
                if (item.Required == true && this.args.ContainsKey(item) == false)
                    throw new ArgumentException(Resources.SwitchIsMissing, item.Name);
            }
        }

        private void AssertValidation()
        {
            var hashSet = new HashSet<string>();

            foreach (var item in this.members)
            {
                hashSet.Add(item.Name);
            }

            foreach (var item in this.members)
            {
                var shortName = item.ShortName;
                if (shortName == string.Empty)
                    continue;

                if (hashSet.Contains(shortName) == true)
                    throw new ArgumentException(Resources.SwitchWasAlreadyRegistered, shortName);
                hashSet.Add(shortName);
            }
        }
    }
}
