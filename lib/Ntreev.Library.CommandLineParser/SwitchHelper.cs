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

namespace Ntreev.Library
{
    class SwitchHelper
    {
        private readonly IEnumerable<SwitchDescriptor> switches;
        private readonly Dictionary<string, string> args = new Dictionary<string, string>();

        public SwitchHelper(object instance)
        {
            this.switches = CommandDescriptor.GetSwitchDescriptors(instance.GetType());
        }

        public SwitchHelper(Type type)
        {
            this.switches = CommandDescriptor.GetSwitchDescriptors(type);
        }

        public SwitchHelper(IEnumerable<SwitchDescriptor> switches)
        {
            this.switches = switches;
        }

        public void Parse(object instance, string[] switchLines, ParseOptions parsingOptions)
        {
            using (new Tracer("Parsing switches"))
            {
                this.args.Clear();
                this.AssertValidation();
                bool ignoreCase = (parsingOptions & ParseOptions.CaseSensitive) != ParseOptions.CaseSensitive;

                foreach (string switchLine in switchLines)
                {
                    Trace.WriteLine("finding switch : " + switchLine);

                    if (this.DoMatch(switchLine, ignoreCase) == false)
                        throw new InvalidSwitchStringException(switchLine);
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
                item.SetValue(instance, this.args[item.Name]);
            }
        }

        private bool DoMatch(string switchLine, bool ignoreCase)
        {
            SwitchDescriptor matchedSwitch = null;
            foreach (SwitchDescriptor item in this.switches)
            {
                if (this.args.ContainsKey(item.Name) == true)
                    continue;
                string arg = item.TryMatch(switchLine, ignoreCase);
                if (arg != null)
                {
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
                    throw new MissingSwitchException(Properties.Resources.SwitchIsMissing, item.Name);
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
                    throw new SwitchException(Properties.Resources.SwitchWasAlreadyRegistered, shortName);
                hashSet.Add(shortName);
            }
        }
    }
}