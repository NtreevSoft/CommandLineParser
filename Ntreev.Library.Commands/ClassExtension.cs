using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    static class ClassExtension
    {
        public static CommandSwitchAttribute GetCommandSwitchAttribute(this PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.Attributes[typeof(CommandSwitchAttribute)] as CommandSwitchAttribute;
        }

        public static string GetSummary(this PropertyDescriptor propertyDescriptor)
        {
            var attr = propertyDescriptor.Attributes[typeof(SummaryAttribute)] as SummaryAttribute;
            if (attr == null)
                return string.Empty;

            return attr.Summary;
        }

        public static object GetDefaultValue(this PropertyDescriptor propertyDescriptor)
        {
            var attr = propertyDescriptor.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
            if (attr == null)
                return DBNull.Value;

            var value = attr.Value;
            if (value == null)
            {
                if (propertyDescriptor.PropertyType.IsClass == false)
                    return DBNull.Value;
                return null;
            }

            if (value.GetType() == propertyDescriptor.PropertyType)
                return value;

            return propertyDescriptor.Converter.ConvertFrom(value);
        }

        public static string GetDisplayName(this PropertyDescriptor propertyDescriptor)
        {
            var attr = propertyDescriptor.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
            if (attr == null)
                return string.Empty;

            return attr.DisplayName;
        }

        public static CommandSwitchAttribute GetCommandSwitchAttribute(this ICustomAttributeProvider customAttributeProvider)
        {
            return customAttributeProvider.GetCustomAttribute<CommandSwitchAttribute>();
        }

        public static object GetDefaultValue(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (attr == null)
                return DBNull.Value;

            var value = attr.Value;
            if (value == null)
            {
                if (propertyInfo.PropertyType.IsClass == false)
                    return DBNull.Value;
                return null;
            }

            if (value.GetType() == propertyInfo.PropertyType)
                return value;

            if (propertyInfo.PropertyType.IsArray == true)
                return Parser.ParseArray(propertyInfo.PropertyType, value.ToString()); 
            return propertyInfo.GetConverter().ConvertFrom(value);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider customAttributeProvider)
            where T : Attribute
        {
            return customAttributeProvider.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }

        public static string GetDisplayName(this ICustomAttributeProvider customAttributeProvider)
        {
            var attribute = customAttributeProvider.GetCustomAttribute<DisplayNameAttribute>();
            if (attribute == null)
                return string.Empty;

            return attribute.DisplayName;
        }

        public static string GetSummary(this ICustomAttributeProvider customAttributeProvider)
        {
            var attribute = customAttributeProvider.GetCustomAttribute<SummaryAttribute>();
            if (attribute == null)
                return string.Empty;

            return attribute.Summary;
        }

        public static string GetDescription(this ICustomAttributeProvider customAttributeProvider)
        {
            var attribute = customAttributeProvider.GetCustomAttribute<DescriptionAttribute>();
            if (attribute == null)
                return string.Empty;

            return attribute.Description;
        }

        public static bool GetBrowsable(this ICustomAttributeProvider customAttributeProvider)
        {
            var attribute = customAttributeProvider.GetCustomAttribute<BrowsableAttribute>();
            if (attribute == null)
                return true;

            return attribute.Browsable;
        }

        public static TypeConverter GetConverter(this PropertyInfo propertyInfo)
        {
            return (propertyInfo as ICustomAttributeProvider).GetConverter(propertyInfo.PropertyType);
        }

        public static TypeConverter GetConverter(this ParameterInfo parameterInfo)
        {
            return (parameterInfo as ICustomAttributeProvider).GetConverter(parameterInfo.ParameterType);
        }

        public static TypeConverter GetConverter(this ICustomAttributeProvider customAttributeProvider, Type type)
        {
            var attribute = customAttributeProvider.GetCustomAttribute<TypeConverterAttribute>();
            if (attribute == null)
                return TypeDescriptor.GetConverter(type);

            try
            {
                var converterType = Type.GetType(attribute.ConverterTypeName);
                return Activator.CreateInstance(converterType) as TypeConverter;
            }
            catch (Exception)
            {
                return TypeDescriptor.GetConverter(type);
            }
        }

        public static bool IsCommandMethod(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes(typeof(CommandMethodAttribute), false).Any();
        }

        public static CommandMethodAttribute GetCommandMethodAttribute(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes(typeof(CommandMethodAttribute), false).FirstOrDefault() as CommandMethodAttribute;
        }

        public static object GetValue(this ParameterInfo parameterInfo, string arg)
        {
            var converter = TypeDescriptor.GetConverter(parameterInfo.ParameterType);
            return converter.ConvertFromString(arg);
        }

        public static bool TryGetDefaultValue(this ParameterInfo parameterInfo, out object value)
        {
            value = null;
            var attrs = parameterInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
            if (attrs.Any() == false)
                return false;
            var attr = attrs.First() as DefaultValueAttribute;
            value = attr.Value;
            return true;
        }

        public static string GetSimpleName(this Type type)
        {
            if (type == typeof(bool))
                return "bool";
            else if (type == typeof(string))
                return "string";
            else if (type == typeof(char))
                return "char";
            else if (type == typeof(sbyte))
                return "int8";
            else if (type == typeof(byte))
                return "uint8";
            else if (type == typeof(short))
                return "int16";
            else if (type == typeof(ushort))
                return "uint16";
            else if (type == typeof(int))
                return "int32";
            else if (type == typeof(uint))
                return "uint32";
            else if (type == typeof(long))
                return "int64";
            else if (type == typeof(ulong))
                return "uint64";
            else if (type == typeof(float))
                return "single";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            return type.Name;
        }

        //public static string ToSpinalCase(this string text)
        //{
        //    return Regex.Replace(text, @"([a-z])([A-Z])", "$1-$2").ToLower();
        //}
    }
}
