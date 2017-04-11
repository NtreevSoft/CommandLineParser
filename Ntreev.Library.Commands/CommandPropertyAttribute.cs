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
    /// <summary>
    /// 스위치의 속성을 지정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CommandPropertyAttribute : Attribute
    {
        private string name;
        private char shortName;
        private bool isRequired;
        private bool shortNameOnly;
        private bool isImplicit;

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

        public virtual bool IsRequired
        {
            get { return this.isRequired; }
            set
            {
                if (this.isImplicit == true && value == true)
                    throw new ArgumentException();
                this.isRequired = value;
            }
        }

        /// <summary>
        /// 기본값을 설정하는데 있어서 명령줄에 인자 명확히 포함되어야 하는지에 대한 여부를 설정합니다.
        /// </summary>
        public bool IsImplicit
        {
            get { return this.isImplicit; }
            set
            {
                if (this.isRequired == true && value == true)
                    throw new ArgumentException();
                this.isImplicit = value;
            }
        }

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