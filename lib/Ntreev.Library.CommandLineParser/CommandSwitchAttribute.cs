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
using Ntreev.Library.Properties;

namespace Ntreev.Library
{
    /// <summary>
    /// 스위치의 속성을 지정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CommandSwitchAttribute : Attribute
    {
        private static string switchDelimiter = "--";
        private static string shortSwitchDelimiter = "-";
        static internal CommandSwitchAttribute DefaultValue = new CommandSwitchAttribute();

        private string name = string.Empty;
        private char shortName;
        private char? argSeperator = null;

        public static string SwitchDelimiter
        {
            get
            {
                return CommandSwitchAttribute.switchDelimiter;
            }
            set
            {
                if (value.Any(item => char.IsPunctuation(item)) == false)
                    throw new Exception(Resources.SwitchDelimiterMustBePunctuation);
                CommandSwitchAttribute.switchDelimiter = value;
            }
        }

        public static string ShortSwitchDelimiter
        {
            get
            {
                return CommandSwitchAttribute.shortSwitchDelimiter;
            }
            set
            {
                if (value.Any(item => char.IsPunctuation(item)) == false)
                    throw new Exception(Resources.SwitchDelimiterMustBePunctuation);
                CommandSwitchAttribute.shortSwitchDelimiter = value;
            }
        }

        internal char? GetArgSeperator()
        {
            return this.argSeperator;
        }

        /// <summary>
        /// <seealso cref="CommandSwitchAttribute"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public CommandSwitchAttribute()
        {
            this.Required = false;

            //if (char.IsLetterOrDigit(this.shortName) == false)
            //    throw new SwitchException(Resources.InvalidSwitchName);
        }

        /// <summary>
        /// 해당 스위치의 이름을 설정하거나 가져옵니다.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 해당 스위치의 짧은 이름을 설정하거나 가져옵니다.
        /// </summary>
        public char ShortName
        {
            get { return this.shortName; }
            set { this.shortName = value; }
        }

        /// <summary>
        /// 해당 스위치가 꼭 필요한지의 여부를 설정하거나 가져옵니다.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 인자가 스위치에 포함되어 있을때 인자와 스위치를 구분하기 위한 문자를 설정하거나 가져옵니다.
        /// </summary>
        /// <remarks>
        /// /Level6 처럼 스위치와 인자의 구분이 필요가 없다면 <seealso cref="char.MinValue"/>를 설정하세요.
        /// </remarks>
        public char ArgSeperator
        {
            get
            {
                if (this.argSeperator == null)
                    return char.MinValue;
                return (char)this.argSeperator; 
            }
            set 
            {
                if (value != char.MinValue)
                {
                    if (char.IsPunctuation(value) == false)
                        throw new Exception(Resources.ArgSeperatorMustBeAPunctuation);
                    if (value.ToString() == CommandSwitchAttribute.SwitchDelimiter || value.ToString() == CommandSwitchAttribute.ShortSwitchDelimiter)
                        throw new Exception(Resources.ArgSeperatorAndSwitchDelimiterCannotBeTheSame);
                }
                this.argSeperator = (char)value; 
            }
        }

        /// <summary>
        /// 해당 스위치의 사용법 출력 제공자의 타입을 설정하거나 가져옵니다.
        /// </summary>
        public Type UsageProvider { get; set; }

        internal string ShortNameInternal
        {
            get { return this.shortName == char.MinValue ? string.Empty : this.shortName.ToString(); }
        }
    }
}