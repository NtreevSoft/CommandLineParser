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
        private readonly IEnumerable<SwitchDescriptor> switches;
        private readonly IEnumerable<SwitchDescriptor> options;
        private readonly Dictionary<string, string> args = new Dictionary<string, string>();

        public SwitchHelper(object target)
        {
            SwitchDescriptorCollection switchDescriptors;
            if (target is Type)
                switchDescriptors = CommandDescriptor.GetSwitchDescriptors(target as Type);
            else
                switchDescriptors = CommandDescriptor.GetSwitchDescriptors(target);
            this.switches = switchDescriptors.Where(item => item.Required == true);
            this.options = switchDescriptors.Where(item => item.Required == false);
        }

        public SwitchHelper(IEnumerable<SwitchDescriptor> switches, IEnumerable<SwitchDescriptor> options)
        {
            this.switches = switches;
            this.options = options;
        }

        public void Parse(object instance, string arguments)
        {
            IDictionary<string, SwitchDescriptor> o = this.options.ToDictionary(item => item.Name);
            IList<SwitchDescriptor> r = this.switches.ToList();

            string line = arguments;
            while (string.IsNullOrEmpty(line) == false)
            {
                if (line.First() == CommandSwitchAttribute.SwitchDelimiter)
                {
                    line = this.ParseOption(instance, o, line);
                }
                else
                {
                    if (r.Count == 0)
                        throw new SwitchException("필수 인자가 너무 많이 포함되어 있습니다.");
                    line = this.ParseRequired(instance, r.First(), line);
                    r.RemoveAt(0);
                }
            }

            if (r.Count > 0)
            {
                throw new MissingSwitchException("필수 인자가 빠져있습니다", r.First().Name);
            }

            this.SetValues(instance);
        }

        private string ParseOption(object instance, IDictionary<string, SwitchDescriptor> options, string arguments)
        {
            string delimiterPattern = string.Format(@"{0}\S+((\s+""[^""]*"")|(\s+[\S-[{0}]][\S]*)|(\s*))", CommandSwitchAttribute.SwitchDelimiter);

            Match match = Regex.Match(arguments, delimiterPattern);

            if (match.Success == true)
            {
                if (this.DoMatch(match.Value) == true)
                {
                    return arguments.Substring(match.Length).Trim();
                }
            }
            
            throw new SwitchException("확인할 수 없는 인자가 포함되어 있습니다.");
        }

        private string ParseRequired(object instance, SwitchDescriptor switchDescriptor, string arguments)
        {
            string normalPattern = @"^((""[^""]*"")|(\S+))";

            Match match = Regex.Match(arguments, normalPattern);

            if (match.Success == true)
            {
                this.args.Add(switchDescriptor.Name, match.Value);
                return arguments.Substring(match.Length).Trim();
            }

            throw new Exception();
        }
        

        public void Parse(object instance, string[] switchLines)
        {
            using (new Tracer("Parsing switches"))
            {
                this.args.Clear();
                this.AssertValidation();

                foreach (string switchLine in switchLines)
                {
                    Trace.WriteLine("finding switch : " + switchLine);

                    if (this.DoMatch(switchLine) == false)
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
                if (this.args.ContainsKey(item.Name) == false)
                    continue;
                item.SetValue(instance, this.args[item.Name].Trim('\"'));
            }

            foreach (SwitchDescriptor item in this.options)
            {
                if (this.args.ContainsKey(item.Name) == false)
                    continue;
                item.SetValue(instance, this.args[item.Name].Trim('\"'));
            }
        }

        private bool DoMatch(string switchLine)
        {
            SwitchDescriptor matchedSwitch = null;
            foreach (SwitchDescriptor item in this.options)
            {
                if (this.args.ContainsKey(item.Name) == true)
                    continue;
                string arg = item.TryMatch(switchLine);
                if (arg != null)
                {
                    if(this.args.ContainsKey(item.Name) == true)
                        throw new SwitchException("이름이 같은 선택 인자가 두번 정의되어 있습니다.");
                    this.args.Add(item.Name, arg);
                    return true;
                }
            }

            return matchedSwitch != null;
        }

        private void AssertRequired()
        {
            foreach (SwitchDescriptor item in this.switches)
            {
                if (item.Required == true && this.args.ContainsKey(item.Name) == false)
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