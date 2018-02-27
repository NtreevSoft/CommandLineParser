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

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// CommandMethod로 사용할 메소드에 추가적으로 사용할 속성을 설정합니다.
    /// 속성의 이름은 여러개를 설정할 수 있으며 해당 클래스내에 CommandProperty 특성을 갖고 있는 public 속성이여야만 합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CommandMethodPropertyAttribute : Attribute
    {
        private readonly string[] propertyNames;

        public CommandMethodPropertyAttribute(params string[] propertyNames)
        {
            if (propertyNames.Any() == false)
                throw new InvalidOperationException("최소 1개 이상의 속성이 설정되어야만 합니다.");
            this.propertyNames = propertyNames;
        }

        public string[] PropertyNames
        {
            get { return this.propertyNames; }
        }
    }
}
