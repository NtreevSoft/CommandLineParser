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

namespace Ntreev.Library.CommandLineParser
{
    /// <summary>
    /// 문자열을 분석하여 데이터로 변환할 수 있는 방법을 제공합니다.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// <seealso cref="Parser"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public Parser()
        {

        }

        /// <summary>
        /// 문자열을 분석하여 데이터로 변환합니다.
        /// </summary>
        /// <param name="switchDescriptor">분석할 스위치의 정보를 담고 있는<seealso cref="CommandSwitchDescriptor"/>의 인스턴스입니다.</param>
        /// <param name="arg">분석할 문자열을 나타냅니다.</param>
        /// <param name="value">분석할 스위치와 연결되어 있는 데이터의 원본값 입니다.</param>
        /// <returns>문자열을 데이터로 변환한 값 입니다.</returns>
        /// <exception cref="NotSupportedException">문자열을 데이터로 변환할 수 없을때</exception>
        virtual public object Parse(CommandSwitchDescriptor switchDescriptor, string arg, object value)
        {
            TypeConverter typeConverter = switchDescriptor.Converter;

            if (typeConverter.CanConvertFrom(typeof(string)) == false)
                throw new NotSupportedException(Properties.Resources.CannotConvertFromString);

            try
            {
                value = typeConverter.ConvertFrom(arg);
            }
            catch (Exception e)
            {
                throw new CommandSwitchException(Properties.Resources.InvalidArgumentType, switchDescriptor.Name, e);
            }
            return value;
        }
    }
}