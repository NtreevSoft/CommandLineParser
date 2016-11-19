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
using System.Reflection;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    static class Parser
    {
        public static object Parse(SwitchDescriptor descriptor, string arg)
        {
            if (descriptor.SwitchType.IsArray == true || typeof(System.Collections.IList).IsAssignableFrom(descriptor.SwitchType) == true)
            {
                return ParseArray(descriptor, arg);
            }
            else if (descriptor.SwitchType == typeof(bool))
            {
                return ParseBoolean(descriptor, arg);
            }
            else
            {
                return ParseDefault(descriptor, arg);
            }
        }

        private static object ParseBoolean(SwitchDescriptor descriptor, string arg)
        {
            if (descriptor.SwitchType == typeof(bool) && descriptor.ArgSeperator == null)
            {
                return true;
            }
            return ParseDefault(descriptor, arg);
        }

        private static object ParseDefault(SwitchDescriptor descriptor, string arg)
        {
            var converter = descriptor.Converter;

            if (converter.CanConvertFrom(typeof(string)) == false)
                throw new NotSupportedException(string.Format(Resources.CannotConvert_Format, arg));

            try
            {
                return converter.ConvertFrom(arg);
            }
            catch (Exception e)
            {
                throw new ArgumentException(Resources.InvalidArgumentType, descriptor.Name, e);
            }
        }

        public static object ParseArray(Type propertyType, string arg)
        {
            System.Collections.IList list;

            if (propertyType.IsArray == true)
            {
                list = new System.Collections.ArrayList() as System.Collections.IList;
            }
            else
            {
                list = TypeDescriptor.CreateInstance(null, propertyType, null, null) as System.Collections.IList;
            }

            var itemType = GetItemType(propertyType);
            if (itemType == null)
                throw new NotSupportedException();

            var segments = arg.Split(new char[] { CommandSettings.ItemSperator, });

            var converter = TypeDescriptor.GetConverter(itemType);
            foreach (var item in segments)
            {
                var s = item.Trim();
                if (s.Length == 0)
                    continue;
                var element = converter.ConvertFromString(s);
                list.Add(element);
            }

            if (propertyType.IsArray == true)
            {
                var array = Array.CreateInstance(itemType, list.Count);
                list.CopyTo(array, 0);
                list = array as System.Collections.IList;
            }
            else
            {

            }
            return list;
        }

        private static object ParseArray(SwitchDescriptor descriptor, string arg)
        {
            System.Collections.IList list;

            if (descriptor.SwitchType.IsArray == true)
            {
                list = new System.Collections.ArrayList() as System.Collections.IList;
            }
            else
            {
                list = TypeDescriptor.CreateInstance(null, descriptor.SwitchType, null, null) as System.Collections.IList;
            }

            var itemType = GetItemType(descriptor.SwitchType);
            if (itemType == null)
                throw new NotSupportedException();

            var segments = arg.Split(new char[] { CommandSettings.ItemSperator, });

            try
            {
                var converter = TypeDescriptor.GetConverter(itemType);
                foreach (var item in segments)
                {
                    var s = item.Trim();
                    if (s.Length == 0)
                        continue;
                    var element = converter.ConvertFromString(s);
                    list.Add(element);
                }

                if (descriptor.SwitchType.IsArray == true)
                {
                    var array = Array.CreateInstance(itemType, list.Count);
                    list.CopyTo(array, 0);
                    list = array as System.Collections.IList;
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                throw new ArgumentException(Resources.InvalidArgumentType, descriptor.Name, e);
            }
            return list;
        }

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