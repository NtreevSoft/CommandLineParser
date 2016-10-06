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
        //private static readonly Parser DefaultBooleanParser = new BooleanParser();
        //private static readonly Parser DefaultParser = new Parser();
        //private static readonly Parser DefaultListParser = new ListParser();


        public static object Parse(SwitchDescriptor descriptor, string arg)
        {
            if (descriptor.ArgType.IsArray == true || typeof(System.Collections.IList).IsAssignableFrom(descriptor.ArgType) == true)
            {
                return ParseArray(descriptor, arg);
            }
            else if (descriptor.ArgType == typeof(bool))
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
            if (descriptor.ArgType == typeof(bool) && descriptor.ArgSeperator == null)
            {
                return true;
            }
            return ParseDefault(descriptor, arg);
        }

        private static object ParseDefault(SwitchDescriptor descriptor, string arg)
        {
            var typeConverter = descriptor.Converter;

            if (typeConverter.CanConvertFrom(typeof(string)) == false)
                throw new NotSupportedException(Resources.CannotConvertFromString);

            try
            {
                return typeConverter.ConvertFrom(arg);
            }
            catch (Exception e)
            {
                throw new ArgumentException(Resources.InvalidArgumentType, descriptor.Name, e);
            }
        }

        private static object ParseArray(SwitchDescriptor descriptor, string arg)
        {
            System.Collections.IList list;

            if (descriptor.ArgType.IsArray == true)
            {
                list = new System.Collections.ArrayList() as System.Collections.IList;
            }
            else
            {
                list = TypeDescriptor.CreateInstance(null, descriptor.ArgType, null, null) as System.Collections.IList;
            }

            var itemType = GetItemType(descriptor.ArgType);
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

                if (descriptor.ArgType.IsArray == true)
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

        //internal static Parser GetParser(PropertyDescriptor propertyDescriptor)
        //{
        //    var parserAttribute = propertyDescriptor.Attributes[typeof(CommandParserAttribute)] as CommandParserAttribute;

        //    if (parserAttribute != null)
        //    {
        //        return TypeDescriptor.CreateInstance(null, parserAttribute.ParserType, null, null) as Parser;
        //    }

        //    if (propertyDescriptor.PropertyType.IsArray == true || typeof(System.Collections.IList).IsAssignableFrom(propertyDescriptor.PropertyType) == true)
        //    {
        //        return Parser.DefaultListParser;
        //    }
        //    else if (propertyDescriptor.PropertyType == typeof(bool))
        //    {
        //        return Parser.DefaultBooleanParser;
        //    }
        //    else if (propertyDescriptor.PropertyType.IsValueType == true)
        //    {
        //        return Parser.DefaultParser;
        //    }

        //    if (propertyDescriptor.Converter.CanConvertFrom(typeof(string)) == true)
        //        return Parser.DefaultParser;
        //    return null;
        //}

        //internal static Parser GetParser(ICustomAttributeProvider customAttributeProvider, Type type)
        //{
        //    var attribute = customAttributeProvider.GetCustomAttribute<CommandParserAttribute>();

        //    if (attribute != null)
        //    {
        //        return TypeDescriptor.CreateInstance(null, attribute.ParserType, null, null) as Parser;
        //    }

        //    var converter = customAttributeProvider.GetConverter(type);
        //    if (type.IsArray == true || typeof(System.Collections.IList).IsAssignableFrom(type) == true)
        //    {
        //        return Parser.DefaultListParser;
        //    }
        //    else if (type == typeof(bool))
        //    {
        //        return Parser.DefaultBooleanParser;
        //    }
        //    else if (type.IsValueType == true)
        //    {
        //        return Parser.DefaultParser;
        //    }

        //    if (converter.CanConvertFrom(typeof(string)) == true)
        //        return Parser.DefaultParser;
        //    return null;
        //}

        //internal static Parser GetParser(ParameterInfo parameterInfo)
        //{
        //    return Parser.GetParser(parameterInfo, parameterInfo.ParameterType);
        //}

        //internal static Parser GetParser(PropertyInfo propertyInfo)
        //{
        //    return Parser.GetParser(propertyInfo, propertyInfo.PropertyType);
        //}
    }
}