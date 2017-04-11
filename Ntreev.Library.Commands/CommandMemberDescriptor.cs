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
using System.Text.RegularExpressions;
using System.Reflection;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    public abstract class CommandMemberDescriptor
    {
        private const string keyName = "key";
        private const string valueName = "value";

        private readonly CommandPropertyAttribute attribute;
        private readonly string descriptorName;
        private readonly string name;
        private readonly string shortName;

        protected CommandMemberDescriptor(CommandPropertyAttribute attribute, string descriptorName)
        {
            this.attribute = attribute;
            this.descriptorName = descriptorName;
            this.name = this.attribute.Name != string.Empty ? this.attribute.Name : descriptorName;
            this.shortName = this.attribute.ShortNameInternal;
            this.name = CommandSettings.NameGenerator(this.name);
            if (this.attribute.ShortNameOnly == true)
            {
                if (this.shortName == string.Empty)
                    throw new ArgumentException(Resources.ShortNameDoesNotExist);
                this.name = string.Empty;
            }
        }

        protected abstract void SetValue(object instance, object value);

        protected abstract object GetValue(object instance);

        public string Name
        {
            get { return this.name; }
        }

        public string ShortName
        {
            get { return this.shortName; }
        }

        public virtual string DisplayName
        {
            get { return this.Name; }
        }

        public virtual string Summary
        {
            get { return string.Empty; }
        }

        public virtual string Description
        {
            get { return string.Empty; }
        }

        public virtual object DefaultValue
        {
            get { return DBNull.Value; }
        }

        public bool IsRequired
        {
            get { return this.attribute.IsRequired; }
        }

        public bool IsImplicit
        {
            get { return this.attribute.IsImplicit; }
        }

        public abstract Type MemberType
        {
            get;
        }

        public virtual TypeConverter Converter
        {
            get { return TypeDescriptor.GetConverter(this.MemberType); }
        }

        public virtual IEnumerable<Attribute> Attributes
        {
            get { yield break; }
        }

        public string DescriptorName
        {
            get { return this.descriptorName; }
        }

        internal object Parse(object instance, string arg)
        {
            return Parser.Parse(this, arg);
        }

        internal void Parse(object instance, List<string> arguments)
        {
            if (this.MemberType == typeof(bool))
            {
                this.SetValue(instance, true);
            }
            else
            {
                var arg = arguments.First();
                var value = Parser.Parse(this, arg);
                this.SetValue(instance, value);
                arguments.RemoveAt(0);
            }
        }

        internal string NamePattern
        {
            get
            {
                if (this.name == string.Empty)
                    return string.Empty;
                return CommandSettings.Delimiter + this.name;
            }
        }

        internal string ShortNamePattern
        {
            get
            {
                if (this.ShortName == string.Empty)
                    return string.Empty;
                return CommandSettings.ShortDelimiter + this.shortName;
            }
        }

        internal void SetValueInternal(object instance, object value)
        {
            this.SetValue(instance, value);
        }

        internal object GetValueInternal(object instance)
        {
            return this.GetValue(instance);
        }
    }
}
