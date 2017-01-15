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
        private const string SwitchGroupName = "switch";
        private const string ArgGroupName = "arg";

        private readonly CommandPropertyAttribute switchAttribute;
        private readonly string descriptorName;
        private readonly string name;
        private readonly string shortName;

        private string pattern;

        protected CommandMemberDescriptor(CommandPropertyAttribute switchAttribute, string descriptorName)
        {
            this.switchAttribute = switchAttribute;
            this.descriptorName = descriptorName;
            this.name = this.switchAttribute.Name != string.Empty ? this.switchAttribute.Name : descriptorName;
            this.shortName = this.switchAttribute.ShortNameInternal;
            this.name = CommandSettings.NameGenerator(this.name);
            if (this.switchAttribute.ShortNameOnly == true)
            {
                if (this.shortName == string.Empty)
                    throw new ArgumentException(Resources.ShortNameDoesNotExist);
                this.name = string.Empty;
            }
        }

        public abstract void SetValue(object instance, object value);

        public abstract object GetValue(object instance);

        //public char? ArgSeparator
        //{
        //    get { return this.switchAttribute.GetArgSeparator(); }
        //}

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

        public bool Required
        {
            get { return this.switchAttribute.Required; }
        }

        public abstract Type SwitchType
        {
            get;
        }

        public virtual TypeConverter Converter
        {
            get { return TypeDescriptor.GetConverter(this.SwitchType); }
        }

        public virtual IEnumerable<Attribute> Attributes
        {
            get { yield break; }
        }

        private string BuildPattern()
        {
            var quotes = string.Format(@"(""(?<{0}>.*)"")", CommandMemberDescriptor.ArgGroupName);
            var normal = string.Format(@"(?<{0}>(\S)+)", CommandMemberDescriptor.ArgGroupName);

            var pattern = string.Empty;
            if (this.Name != string.Empty && this.ShortName != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>({1}{2}|{3}{4}))", CommandMemberDescriptor.SwitchGroupName, CommandSettings.SwitchDelimiter, this.Name, CommandSettings.ShortSwitchDelimiter, this.ShortName);
            }
            else if (this.Name != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>{1}{2})", CommandMemberDescriptor.SwitchGroupName, CommandSettings.SwitchDelimiter, this.Name);
            }
            else if (this.ShortName != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>{1}{2})", CommandMemberDescriptor.SwitchGroupName, CommandSettings.ShortSwitchDelimiter, this.ShortName);
            }

            //if (this.SwitchType != typeof(bool)l)
            //{
            //    if (argSeparator == null)
            //    {
            //        pattern += string.Format(@"(((\s+)({0}|{1}))|($))", quotes, normal);
            //    }
            //    else
            //    {
            //        if (argSeparator != char.MinValue)
            //            pattern += argSeparator;
            //        pattern += string.Format(@"(({0}|{1})|$)", quotes, normal);
            //    }
            //}
            //else
            //{
            //    pattern += @"((\s+)|$)";
            //}

            return pattern;
        }

        private string Pattern
        {
            get
            {
                if (this.pattern == null)
                    this.pattern = this.BuildPattern();
                return this.pattern;
            }
        }

        internal object Parse(object instance, string arg)
        {
            return Parser.Parse(this, arg);
        }

        internal void Parse(object instance, List<string> arguments)
        {
            if (this.SwitchType == typeof(bool))
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

        internal string TryMatch(string arguments, ref string parsed)
        {
            var match = Regex.Match(arguments, this.Pattern, RegexOptions.ExplicitCapture);
            if (match.Success == false)
                return null;
            parsed = match.Value;
            return match.Groups[CommandMemberDescriptor.ArgGroupName].Value;
        }

        internal string DescriptorName
        {
            get { return this.descriptorName; }
        }

        internal string NamePattern
        {
            get
            {
                if (this.name == string.Empty)
                    return string.Empty;
                return CommandSettings.SwitchDelimiter + this.name;
            }
        }

        internal string ShortNamePattern
        {
            get
            {
                if (this.ShortName == string.Empty)
                    return string.Empty;
                return CommandSettings.ShortSwitchDelimiter + this.shortName;
            }
        }
    }
}
