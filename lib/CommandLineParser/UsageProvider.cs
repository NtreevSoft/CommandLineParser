#region License
//Ntreev CommandLineParser for .Net 
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

namespace Ntreev.Library
{
    /// <summary>
    /// 스위치 사용법을 제공하는 방법을 나타냅니다.
    /// </summary>
    public abstract class UsageProvider
    {
        readonly SwitchDescriptor switchDescriptor;

        /// <summary>
        /// <seealso cref="SwitchDescriptor"/>를 사용하여 <seealso cref="UsageProvider"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="switchDescriptor">
        /// 스위치의 정보를 담고 있는 <seealso cref="SwitchDescriptor"/>의 인스턴스입니다.
        /// </param>
        public UsageProvider(SwitchDescriptor switchDescriptor)
        {
            this.switchDescriptor = switchDescriptor;
        }

        /// <summary>
        /// 스위치의 정보를 담고 있는 <seealso cref="SwitchDescriptor"/>의 인스턴스를 가져옵니다.
        /// </summary>
        protected SwitchDescriptor SwitchDescriptor
        {
            get { return this.switchDescriptor; }
        }

        /// <summary>
        /// 기본적인 사용방법을 가져옵니다.
        /// </summary>
        public abstract string Usage
        {
            get;
        }

        /// <summary>
        /// 기본적인 사용방법 외에 부가적인 설명을 가져옵니다.
        /// </summary>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// 스위치에서 사용하는 인자형식에 대한 설명을 가져옵니다.
        /// </summary>
        public virtual string ArgumentTypeDescription
        {
            get { return string.Empty; }
        }
    }
}