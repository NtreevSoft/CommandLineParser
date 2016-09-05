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
using System.Reflection;
using System.ComponentModel;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// 문자열을 리스트 형식으로 변환하는 방법을 제공합니다.
    /// </summary>
    public class ListParser : Parser
    {
        public override object Parse(SwitchDescriptor switchDescriptor, string arg, object value)
        {
            System.Collections.IList list;

            if (value == null)
            {
                if (switchDescriptor.ArgType.IsArray == true)
                {
                    list = new System.Collections.ArrayList() as System.Collections.IList;
                }
                else
                {
                    list = TypeDescriptor.CreateInstance(null, switchDescriptor.ArgType, null, null) as System.Collections.IList;
                }
            }
            else
            {
                list = value as System.Collections.IList;
            }

            var itemType = GetItemType(switchDescriptor.ArgType);
            if (itemType == null)
                throw new NotSupportedException();
            var args = this.SplitArgument(arg);

            try
            {
                foreach (var item in args)
                {
                    var s = item.Trim();
                    if(s.Length == 0)
                        continue;
                    var element = OnItemParse(s, itemType);
                    list.Add(element);
                }

                if (switchDescriptor.ArgType.IsArray == true)
                {
                    var array = Array.CreateInstance(itemType, list.Count);
                    list.CopyTo(array, 0);
                    list = array as System.Collections.IList;
                }
            }
            catch (Exception e)
            {
                throw new SwitchException(Resources.InvalidArgumentType, switchDescriptor.Name, e);
            }

            return list;
        }

        /// <summary>
        /// 문자열에서 항목별 문자열로 분리합니다.
        /// </summary>
        /// <param name="arg">항목별 문자열이 담긴 문자열입니다.</param>
        /// <returns>분리된 항목별 문자열의 배열입니다.</returns>
        protected virtual string[] SplitArgument(string arg)
        {
            return arg.Split(new char[] { ',', });
        }

        /// <summary>
        /// 항목별 문자열을 데이터로 변환합니다.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="itemType">항목의 데이터 형식을 나타냅니다.</param>
        /// <returns>항목을 나타내는 문자열을 데이터로 변환한 값을 나타냅니다.</returns>
        protected virtual object OnItemParse(string arg, Type itemType)
        {
            var typeConverter = TypeDescriptor.GetConverter(itemType);
            return typeConverter.ConvertFrom(arg);
        }

        /// <summary>
        /// 속성의 타입이 리스트 형태일때 항목의 타입을 가져옵니다.
        /// </summary>
        /// <param name="propertyType">리스트를 나타내는 타입입니다.</param>
        /// <returns>리스트 형식의 타입이면 항목의 타입을 반환합니다. 그렇지 않다면 null을 반환합니다.</returns>
        public static Type GetItemType(Type propertyType)
        {
            if (propertyType.IsArray == true)
            {
                return propertyType.GetElementType();
            }
            else
            {
                var properties = TypeDescriptor.GetReflectionType(propertyType).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var item in properties)
                {
                    if (item.Name.Equals("Item") || item.Name.Equals("Items"))
                    {
                        return item.PropertyType;
                    }
                }
            }
            return null;
        }
    }
}