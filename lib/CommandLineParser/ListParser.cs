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
using System.Reflection;
using System.ComponentModel;

namespace Ntreev.Library
{
    public class ListParser : Parser
    {
        public override object Parse(SwitchDescriptor switchDescriptor, string arg, object value)
        {
            System.Collections.IList list;

            if (value == null)
            {
                list = TypeDescriptor.CreateInstance(null, switchDescriptor.ArgType, null, null) as System.Collections.IList;
            }
            else
            {
                list = value as System.Collections.IList;
            }

            Type type = GetElementType(switchDescriptor.ArgType);
            if (type == null)
                throw new NotSupportedException();
            string[] args = SplitArgument(arg);

            try
            {
                foreach (string item in args)
                {
                    string s = item.Trim();
                    if(s.Length == 0)
                        continue;
                    object element = OnItemParse(s, type);
                    list.Add(element);
                }
            }
            catch (Exception e)
            {
                throw new SwitchException("잘못된 인수 형식입니다.", switchDescriptor.Name, e);
            }

            return list;
        }

        virtual protected string[] SplitArgument(string arg)
        {
            return arg.Split(new char[] { ',', });
        }

        virtual protected object OnItemParse(string arg, Type itemType)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(itemType);
            return typeConverter.ConvertFrom(arg);
        }

        static public Type GetElementType(Type propertyType)
        {
            PropertyInfo[] properties = TypeDescriptor.GetReflectionType(propertyType).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo item in properties)
            {
                if (item.Name.Equals("Item") || item.Name.Equals("Items"))
                {
                    return item.PropertyType;
                }
            }

            return null;
        }
    }
}