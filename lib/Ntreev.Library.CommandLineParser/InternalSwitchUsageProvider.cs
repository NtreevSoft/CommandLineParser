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

namespace Ntreev.Library
{
    class InternalSwitchUsageProvider : SwitchUsageProvider
    {
        private bool hasDelimiter = true;

        public InternalSwitchUsageProvider(SwitchDescriptor switchDescriptor, bool hasDelimiter)
            : base(switchDescriptor)
        {
            this.hasDelimiter = hasDelimiter;
        }

        public InternalSwitchUsageProvider(SwitchDescriptor switchDescriptor)
            : base(switchDescriptor)
        {

        }

        public override string Usage
        {
            get
            {
                var shortName = this.SwitchDescriptor.ShortName;
                var name = this.SwitchDescriptor.Name;

                var delim = this.hasDelimiter == true ? CommandSwitchAttribute.SwitchDelimiter : string.Empty;
                var help = string.Empty;

                if (name != string.Empty)
                {
                    help = string.Format("{0}{1}", delim, name);
                }

                if (shortName != string.Empty)
                {
                    delim = this.hasDelimiter == true ? CommandSwitchAttribute.ShortSwitchDelimiter : string.Empty;
                    if(help != string.Empty)
                        help += " | ";
                    help += string.Format("{0}{1}", CommandSwitchAttribute.ShortSwitchDelimiter, shortName);
                }

                return help;
            }
        }

        public override string Description
        {
            get { return this.SwitchDescriptor.Description; }
        }

        public override string ArgumentTypeDescription
        {
            get
            {
                var argType = this.SwitchDescriptor.ArgType;
                if (argType.IsEnum == false)
                    return base.ArgumentTypeDescription;

                var description = string.Empty;

                foreach (var item in Enum.GetNames(argType))
                {
                    if (description != string.Empty)
                    {
                        description += string.Format(", {0}", item);
                    }
                    else
                    {
                        description = item;
                    }
                }

                return string.Format("{0} is {1}", argType.GetSimpleName(), description);
            }
        }
    }
}