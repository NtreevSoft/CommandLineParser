//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Ntreev.Library.Commands.Properties;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CommandPropertyAttribute : Attribute
    {
        private string name;
        private char shortName;
        private bool useName;
        private bool isRequired;
        private bool isExplicit;
        private int group;

        public CommandPropertyAttribute()
        {
            this.useName = true;
        }

        public CommandPropertyAttribute(string name)
            : this(name, char.MinValue)
        {

        }

        public CommandPropertyAttribute(string name, char shortName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Length <= 1)
                throw new ArgumentException("name length must be greater than 1", nameof(name));
            CommandSettings.ValidateIdentifier(name);
            if (shortName != char.MinValue && Regex.IsMatch(shortName.ToString(), "[a-z]", RegexOptions.IgnoreCase) == false)
                throw new ArgumentException("shortName must be a alphabet character");
            this.name = name;
            this.shortName = shortName;
            this.useName = true;
        }

        public CommandPropertyAttribute(char shortName)
            : this(shortName, false)
        {

        }

        public CommandPropertyAttribute(char shortName, bool useName)
        {
            if (shortName != char.MinValue && Regex.IsMatch(shortName.ToString(), "[a-z]", RegexOptions.IgnoreCase) == false)
                throw new ArgumentException("shortName must be a alphabet character");
            this.shortName = shortName;
            this.useName = useName;
        }

        public string Name
        {
            get { return this.name ?? string.Empty; }
        }

        public char ShortName
        {
            get { return this.shortName; }
        }

        /// <summary>
        /// 필수 인자를 나타냅니다. 필수 인자는 스위치 없이 값만 설정할 수 있습니다. 단 IsExplicit 가 true일 경우에는 스위치가 필요합니다.
        /// 기본값이 있는 경우 정렬시 뒤로 밀리게 됩니다. 
        /// </summary>
        public bool IsRequired
        {
            get { return this.isRequired; }
            set { this.isRequired = value; }
        }

        /// <summary>
        /// 일반적인 형태는 --name value와 같이 스위치와 값의 형식이 필요하지만 때로는 value를 생략할 경우가 필요합니다.
        /// 이럴때는 IsExplicit 값을 true로 설정하여 스위치만으로도 동작을 할 수 있게 합니다.
        /// 명령구문에 해당 스위치가 정의된 경우 DefaultValueAttribute의 값으로 설정되며 이 특성이 선언되어 있지 않은 경우에는
        /// 타입의 초기값으로 설정됩니다.
        /// </summary>
        public  bool IsExplicit
        {
            get { return this.isExplicit; }
            set { this.isExplicit = value; }
        }

        public int Group
        {
            get { return this.group; }
            set { this.group = value; }
        }

        protected virtual void Validate(object target)
        {

        }

        internal void InvokeValidate(object target)
        {
            this.Validate(target);
        }

        internal string GetName(string descriptorName)
        {
            if (this.name == null)
            {
                if (this.useName == true)
                    return CommandSettings.NameGenerator(descriptorName);
                return string.Empty;
            }
            return CommandSettings.NameGenerator(this.name);
        }

        internal string InternalShortName
        {
            get { return this.shortName == char.MinValue ? string.Empty : this.shortName.ToString(); }
        }
    }
}