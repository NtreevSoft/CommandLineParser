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

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// 스위치에 사용할 파서의 타입을 지정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandParserAttribute : Attribute
    {
        private readonly Type parserType;

        /// <summary>
        /// 파서의 타입을 가지고 <seealso cref="CommandParserAttribute"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public CommandParserAttribute(Type parserType)
        {
            this.parserType = parserType;
        }

        public CommandParserAttribute(string typeName)
            : this(Type.GetType(typeName))
        {

        }

        /// <summary>
        /// 파서의 타입을 나타냅니다.
        /// </summary>
        public string ParserTypeName
        {
            get { return this.parserType.AssemblyQualifiedName; }
        }

        internal Type ParserType
        {
            get { return this.parserType; }
        }
    }
}