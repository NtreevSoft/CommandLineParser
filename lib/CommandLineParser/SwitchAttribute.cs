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
using System.ComponentModel;

namespace Ntreev.Library
{
    /// <summary>
    /// 스위치의 속성을 지정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SwitchAttribute : Attribute
    {
        static char switchDelimiter = '/';
        static internal SwitchAttribute DefaultValue = new SwitchAttribute();

        string shortName = string.Empty;
        char? argSeperator = null;
       
        public static char SwitchDelimiter
        {
            get
            {
                return SwitchAttribute.switchDelimiter;
            }
            set
            {
                if (char.IsPunctuation(value) == false)
                    throw new Exception(Properties.Resources.SwitchDelimiterMustBePunctuation);
                SwitchAttribute.switchDelimiter = value;
            }
        }

        internal char? GetArgSeperator()
        {
            return this.argSeperator;
        }

        /// <summary>
        /// <seealso cref="SwitchAttribute"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public SwitchAttribute()
        {
            this.Required = false;
            this.MutuallyExclusive = string.Empty;
            this.ArgTypeSummary = string.Empty;

            foreach (char item in this.shortName)
            {
                if (char.IsLetterOrDigit(item) == false)
                    throw new Exception(Properties.Resources.InvalidSwitchName);
            }
        }

        /// <summary>
        /// 짧은 이름을 사용하여 <seealso cref="SwitchAttribute"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public SwitchAttribute(string shortName)
            : this()
        {
            this.shortName = shortName == null ? string.Empty : shortName;
        }

        /// <summary>
        /// 해당 스위치의 짧은 이름을 설정하거나 가져옵니다.
        /// </summary>
        public string ShortName
        {
            get { return this.shortName; }
        }

        /// <summary>
        /// 인수 형식의 요약을 나타내는 문자열입니다.
        /// </summary>
        public string ArgTypeSummary { get; set; }

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
                        throw new Exception(Properties.Resources.ArgSeperatorMustBeAPunctuation);
                    if (value == SwitchAttribute.SwitchDelimiter)
                        throw new Exception(Properties.Resources.ArgSeperatorAndSwitchDelimiterCannotBeTheSame);
                }
                this.argSeperator = (char)value; 
            }
        }

        public bool AllowMultiple { get; set; }

        public string MutuallyExclusive { get; set; }

        /// <summary>
        /// 해당 스위치의 사용법 출력 제공자의 타입을 설정하거나 가져옵니다.
        /// </summary>
        public Type UsageProvider { get; set; }
    }
}