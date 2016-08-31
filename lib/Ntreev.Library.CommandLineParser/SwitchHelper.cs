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

using Trace = System.Diagnostics.Trace;
using Ntreev.Library.Properties;

namespace Ntreev.Library
{
    class SwitchHelper
    {
        private readonly SwitchDescriptor[] switches;
        private readonly Dictionary<SwitchDescriptor, string> args = new Dictionary<SwitchDescriptor, string>();

        public SwitchHelper(object target)
        {
            SwitchDescriptorCollection switchDescriptors;
            if (target is Type)
                switchDescriptors = CommandDescriptor.GetSwitchDescriptors(target as Type);
            else
                switchDescriptors = CommandDescriptor.GetSwitchDescriptors(target);
            this.switches = switchDescriptors.ToArray();
        }

        public SwitchHelper(IEnumerable<SwitchDescriptor> switches)
        {
            this.switches = switches.ToArray();
            //this.options = options;
        }

        public void Parse(object instance, string arguments)
        {
            //IDictionary<string, SwitchDescriptor> o = this.switches.Where(item => item.Required == false).ToDictionary(item => item.Name);
            IList<SwitchDescriptor> requiredSwitches = this.switches.Where(item => item.Required == true).ToList();

            string line = arguments;
            while (string.IsNullOrEmpty(line) == false)
            {
                if (line.StartsWith(CommandSwitchAttribute.SwitchDelimiter) == true || line.StartsWith(CommandSwitchAttribute.ShortSwitchDelimiter))
                {
                    SwitchDescriptor descriptor = this.ParseOption(instance, ref line);
                    if(descriptor.Required == true)
                        requiredSwitches.Remove(descriptor);
                }
                else
                {
                    if (requiredSwitches.Count == 0)
                        throw new SwitchException("필수 인자가 너무 많이 포함되어 있습니다.");
                    this.ParseRequired(requiredSwitches.First(), ref line);
                    requiredSwitches.RemoveAt(0);
                }
            }

            if (requiredSwitches.Count > 0)
            {
                throw new MissingSwitchException("필수 인자가 빠져있습니다", requiredSwitches.First().Name);
            }

            this.SetValues(instance);
        }

        private SwitchDescriptor ParseOption(object instance, ref string arguments)
        {
            string pattern = string.Format(@"^{0}\S+((\s+""[^""]*"")|(\s+[\S-[{0}]][\S]*)|(\s*))", CommandSwitchAttribute.SwitchDelimiter);
            string shortPattern = string.Format(@"^{0}\S+((\s+""[^""]*"")|(\s+[\S-[{0}]][\S]*)|(\s*))", CommandSwitchAttribute.ShortSwitchDelimiter);

            Match match = Regex.Match(arguments, pattern);

            if (match.Success == true)
            {
                SwitchDescriptor descriptor = this.DoMatch(match.Value);
                if (descriptor != null)
                {
                    arguments = arguments.Substring(match.Length).Trim();
                    return descriptor;
                }
            }

            Match shortMatch = Regex.Match(arguments, shortPattern);

            if (shortMatch.Success == true)
            {
                SwitchDescriptor descriptor = this.DoMatch(shortMatch.Value);
                if (descriptor != null)
                {
                    arguments = arguments.Substring(shortMatch.Length).Trim();
                    return descriptor;
                }
            }

            throw new SwitchException("확인할 수 없는 인자가 포함되어 있습니다.");
        }

        private void ParseRequired(SwitchDescriptor switchDescriptor, ref string arguments)
        {
            string normalPattern = @"^((""[^""]*"")|(\S+))";

            Match match = Regex.Match(arguments, normalPattern);

            if (match.Success == true)
            {
                this.args.Add(switchDescriptor, match.Value);
                arguments = arguments.Substring(match.Length).Trim();
                return;
            }

            throw new Exception();
        }


        public void Parse(object instance, string[] switchLines)
        {
            //using (new Tracer("Parsing switches"))
            {
                this.args.Clear();
                this.AssertValidation();

                foreach (string switchLine in switchLines)
                {
                    Trace.WriteLine("finding switch : " + switchLine);
                    SwitchDescriptor descriptor = this.DoMatch(switchLine);
                    if (descriptor== null)
                        throw new SwitchException(Resources.NotFoundMatchedSwitch, switchLine);
                }

                this.AssertRequired();
                this.SetValues(instance);
            }
        }

        public void SetValues(object instance)
        {
            foreach (SwitchDescriptor item in this.switches)
            {
                if (this.args.ContainsKey(item) == false)
                    continue;
                item.SetValue(instance, this.args[item].Trim('\"'));
            }
        }

        private SwitchDescriptor DoMatch(string switchLine)
        {
            foreach (SwitchDescriptor item in this.switches)
            {
                string arg = item.TryMatch(switchLine);
                if (arg != null)
                {
                    if (this.args.ContainsKey(item) == true)
                        throw new SwitchException("이름이 같은 선택 인자가 두번 설정되었습니다.");
                    this.args.Add(item, arg);
                    return item;
                }
            }

            return null;
        }

        private void AssertRequired()
        {
            foreach (SwitchDescriptor item in this.switches)
            {
                if (item.Required == true && this.args.ContainsKey(item) == false)
                    throw new MissingSwitchException(Resources.SwitchIsMissing, item.Name);
            }
        }

        private void AssertValidation()
        {
            HashSet<string> hashSet = new HashSet<string>();

            foreach (SwitchDescriptor item in this.switches)
            {
                hashSet.Add(item.Name);
            }

            foreach (SwitchDescriptor item in this.switches)
            {
                string shortName = item.ShortName;
                if (shortName == string.Empty)
                    continue;

                if (hashSet.Contains(shortName) == true)
                    throw new SwitchException(Resources.SwitchWasAlreadyRegistered, shortName);
                hashSet.Add(shortName);
            }
        }
    }
}