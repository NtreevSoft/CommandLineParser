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
using System.Reflection;
using System.Text;

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// CommandMethod로 사용할 클래스에 추가로 사용될 CommandMethodAttribute가 정의되어 있는 static class 타입을 설정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandStaticMethodAttribute : Attribute
    {
        private readonly string typeName;
        private readonly Type type;
        private readonly string[] methodNames;

        public CommandStaticMethodAttribute(string typeName, params string[] methodNames)
            : this(Type.GetType(typeName), methodNames)
        {

        }

        public CommandStaticMethodAttribute(Type type, params string[] methodNames)
        {
            if (type.GetConstructor(Type.EmptyTypes) == null && type.IsAbstract && type.IsSealed)
            {
                this.type = type;
                this.typeName = type.AssemblyQualifiedName;
            }
            else
            {
                throw new InvalidOperationException("type is not static class.");
            }
            this.methodNames = methodNames;
        }

        public string TypeName
        {
            get { return this.typeName; }
        }

        public string[] MethodNames
        {
            get { return this.methodNames; }
        }

        internal Type StaticType
        {
            get { return this.type; }
        }
    }
}
