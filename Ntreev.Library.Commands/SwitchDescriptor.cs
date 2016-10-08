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

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// 스위치의 정보를 담고 있는 클래스입니다.
    /// </summary>
    public abstract class SwitchDescriptor
    {
        private const string SwitchGroupName = "switch";
        private const string ArgGroupName = "arg";

        private readonly CommandSwitchAttribute switchAttribute;
        private readonly string descriptorName;
        private readonly string name;
        private readonly string shortName;

        private string pattern;

        protected SwitchDescriptor(CommandSwitchAttribute switchAttribute, string descriptorName)
        {
            this.switchAttribute = switchAttribute;
            this.descriptorName = descriptorName;
            this.name = this.switchAttribute.Name != string.Empty ? this.switchAttribute.Name : descriptorName;
            this.shortName = this.switchAttribute.ShortNameInternal;

            //this.VerifyName(ref this.name, ref this.shortName, ref this.displayName);

            this.name = CommandSettings.NameGenerator(this.name);
            if (this.switchAttribute.ShortNameOnly == true)
            {
                if (this.shortName == string.Empty)
                    throw new ArgumentException("짧은 이름이 존재하지 않습니다.");
                this.name = string.Empty;
            }
            else
            {

            }
        }

        public abstract void SetValue(object instance, object value);

        public abstract object GetValue(object instance);

        /// <summary>
        /// 인자와 스위치가 한 문자열내에 포함되어 있을때 이를 구분하는 문자를 가져옵니다.
        /// </summary>
        /// <remarks>
        /// 이 속성의 값이 null이면 인자와 스위치는 공백으로 구분되어져 있습니다.
        /// <seealso cref="Char.MinValue"/>일 경우에는 스위치와 인자가 하나의 단어로 이루어져 있는 상태입니다.
        /// 그외에는 <see cref="Char.IsPunctuation"/>의 값이 true인 문자로 구분되어집니다.
        /// </remarks>
        public char? ArgSeperator
        {
            get { return this.switchAttribute.GetArgSeperator(); }
        }

        internal string DescriptorName
        {
            get { return this.descriptorName; }
        }

        /// <summary>
        /// 스위치의 이름을 가져옵니다.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 스위치의 짧은 이름을 가져옵니다.
        /// </summary>
        public string ShortName
        {
            get { return this.shortName; }
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

        public virtual string DisplayName
        {
            get { return this.Name; }
        }

        /// <summary>
        /// 스위치의 부가적인 설명을 가져옵니다.
        /// </summary>
        public virtual string Summary
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 스위치의 부가적인 설명을 가져옵니다.
        /// </summary>
        public virtual string Description
        {
            get { return string.Empty; }
        }

        public virtual object DefaultValue
        {
            get { return DBNull.Value; }
        }

        /// <summary>
        /// 분석할때 해당 스위치가 꼭 필요한지에 대한 여부를 가져옵니다.
        /// </summary>
        public bool Required
        {
            get { return this.switchAttribute.Required; }
        }

        /// <summary>
        /// 해당 스위치가 가지고 있는 인자의 타입을 가져옵니다.
        /// </summary>
        public abstract Type SwitchType
        {
            get;// { return this.type; }
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
            var quotes = string.Format(@"(""(?<{0}>.*)"")", SwitchDescriptor.ArgGroupName);
            var normal = string.Format(@"(?<{0}>(\S)+)", SwitchDescriptor.ArgGroupName);

            var pattern = string.Empty;
            if (this.Name != string.Empty && this.ShortName != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>({1}{2}|{3}{4}))", SwitchDescriptor.SwitchGroupName, CommandSettings.SwitchDelimiter, this.Name, CommandSettings.ShortSwitchDelimiter, this.ShortName);
            }
            else if (this.Name != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>{1}{2})", SwitchDescriptor.SwitchGroupName, CommandSettings.SwitchDelimiter, this.Name);
            }
            else if (this.ShortName != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>{1}{2})", SwitchDescriptor.SwitchGroupName, CommandSettings.ShortSwitchDelimiter, this.ShortName);
            }

            var argSeperator = this.switchAttribute.GetArgSeperator();
            if (this.SwitchType != typeof(bool) || argSeperator != null)
            {
                if (argSeperator == null)
                {
                    pattern += string.Format(@"(((\s+)({0}|{1}))|($))", quotes, normal);
                }
                else
                {
                    if (argSeperator != char.MinValue)
                        pattern += argSeperator;
                    pattern += string.Format(@"(({0}|{1})|$)", quotes, normal);
                }
            }
            else
            {
                pattern += @"((\s+)|$)";
            }

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

        internal string TryMatch(string switchLine, ref string parsed)
        {
            var match = Regex.Match(switchLine, this.Pattern, RegexOptions.ExplicitCapture);
            if (match.Success == false)
                return null;
            parsed = match.Value;
            return match.Groups[SwitchDescriptor.ArgGroupName].Value;
        }
    }
}
