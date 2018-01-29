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
using Ntreev.Library.Commands.Properties;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CommandPropertyAttribute : Attribute
    {
        private string name;
        private char shortName;
        private bool shortNameOnly;
        private CommandPropertyTypes type;

        public CommandPropertyAttribute()
        {

        }

        public CommandPropertyAttribute(string name)
            : this(name, char.MinValue)
        {

        }

        public CommandPropertyAttribute(string name, char shortName)
        {
            this.name = name;
            if (shortName != char.MinValue && Regex.IsMatch(shortName.ToString(), "[a-z]", RegexOptions.IgnoreCase) == false)
                throw new ArgumentException("shortName must be a alphabet character");
            this.shortName = shortName;
        }

        public CommandPropertyAttribute(char shortName)
            : this(null, shortName)
        {

        }

        public CommandPropertyAttribute(char shortName, bool shortNameOnly)
            : this(null, shortName)
        {
            this.shortNameOnly = shortNameOnly;
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
        /// 필수 속성을 나타냅니다. 필수 속성은 스위치값이 필요없이 인자값만 필요로 합니다. 
        /// 정의된 필수 속성 순서대로 인자값이 설정됩니다.
        /// 정의된 필수 속성 갯수와 인자 갯수가 다르면 예외를 발생합니다.
        /// 단 기본값이 정의되어 있는 경우 생략할 수 있습니다.
        /// </summary>
        public virtual bool IsRequired
        {
            get { return this.type.HasFlag(CommandPropertyTypes.IsRequired); }
            set
            {
                if (this.type != CommandPropertyTypes.None && value == true)
                    throw new ArgumentException();
                if (value == true)
                    this.type = CommandPropertyTypes.IsRequired;
                else
                    this.type = CommandPropertyTypes.None;
            }
        }

        /// <summary>
        /// 일반적인 형태는 --name value와 같이 스위치와 값의 형식이 필요하지만 때로는 value를 생략할 경우가 필요합니다.
        /// 이럴때는 IsImplicit 값을 true로 설정하여 스위치만으로도 동작을 할 수 있게 합니다.
        /// 명령구문에 해당 스위치가 정의된 경우 DefaultValueAttribute의 값으로 설정되며 이 특성이 선언되어 있지 않은 경우에는
        /// 타입의 초기값으로 설정됩니다.
        /// </summary>
        public virtual bool IsImplicit
        {
            get { return this.type.HasFlag(CommandPropertyTypes.IsImplicit); }
            set
            {
                if (this.type != CommandPropertyTypes.None && value == true)
                    throw new ArgumentException();
                if (value == true)
                    this.type = CommandPropertyTypes.IsImplicit;
                else
                    this.type = CommandPropertyTypes.None;
            }
        }

        /// <summary>
        /// 토글 형태의 속성을 나타냅니다. 토글로 설정되면 인자값은 필요없이 스위치값만 요구합니다. 스위치가 설정되면 스위치의 기본값으로 설정합니다.
        /// 따라서 토글 형태의 속성은 항상 기본값이 존재해야 합니다. bool 형태의 속성은 기본값이 자동으로 true로 설정됩니다.
        /// </summary>
        //public virtual bool IsToggle
        //{
        //    get { return this.type.HasFlag(CommandPropertyTypes.IsToggle); }
        //    set
        //    {
        //        if (this.type != CommandPropertyTypes.None && value == true)
        //            throw new ArgumentException();
        //        if (value == true)
        //            this.type = CommandPropertyTypes.IsToggle;
        //        else
        //            this.type = CommandPropertyTypes.None;
        //    }
        //}

        internal bool ShortNameOnly
        {
            get
            {
                if (this.shortNameOnly == true)
                    return true;
                return this.name == null && this.shortName != char.MinValue;
            }
        }

        internal string ShortNameInternal
        {
            get { return this.shortName == char.MinValue ? string.Empty : this.shortName.ToString(); }
        }
    }
}