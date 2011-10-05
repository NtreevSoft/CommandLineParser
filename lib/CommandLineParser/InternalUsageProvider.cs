#region License
//Ntreev CommandLineParser for .Net 1.0.4295.27782
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
    class InternalUsageProvider : UsageProvider
    {
        public InternalUsageProvider(SwitchDescriptor switchDescriptor)
            : base(switchDescriptor)
        {

        }

        public override string Usage
        {
            get 
            {
                string shortName = this.SwitchDescriptor.ShortName;
                string name = this.SwitchDescriptor.Name;

                string help;
                if(shortName == string.Empty)
                    help = string.Format("{0}{1}", SwitchAttribute.SwitchDelimiter, name);
                else
                    help = string.Format("{0}{1}", SwitchAttribute.SwitchDelimiter, shortName);

                char? argSeperator = this.SwitchDescriptor.ArgSeperator;
                Type argType = this.SwitchDescriptor.ArgType;
                if (argType != typeof(bool) || argSeperator != null)
                {
                    string argTypeName = this.SwitchDescriptor.ArgTypeSummary;

                    if (argSeperator == null)
                    {
                        help += string.Format(" <{0}>", argTypeName);
                    }
                    else
                    {
                        if (argSeperator != char.MinValue)
                            help += string.Format("<{0}{1}>", argSeperator, argTypeName);
                        else
                            help += string.Format("<{0}>", argTypeName);
                    }
                }
                return help;
            }
        }

        public override string Description
        {
            get 
            {
                string description = this.SwitchDescriptor.Description;

                if (this.SwitchDescriptor.DisplayName != this.SwitchDescriptor.Name)
                    description = this.SwitchDescriptor.DisplayName + " " + description;
                return description;

            }
        }

        public override string ArgumentTypeDescription
        {
            get
            {
                Type argType = this.SwitchDescriptor.ArgType;
                if (argType.IsEnum == false)
                    return base.ArgumentTypeDescription;

                string description = string.Empty;

                foreach(string name in Enum.GetNames(argType))
                {
                    if(description != string.Empty)
                    {
                        description += string.Format(", {0}", name);
                    }
                    else
                    {
                        description = name;
                    }
                }

                return string.Format("{0} is {1}", argType.Name, description);

            }
        }
    }
}