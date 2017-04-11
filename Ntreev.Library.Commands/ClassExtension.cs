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
        public static CommandPropertyAttribute GetCommandPropertyAttribute(this ICustomAttributeProvider customAttributeProvider)
        {
            return customAttributeProvider.GetCustomAttribute<CommandPropertyAttribute>();
        }

        public static CommandMethodAttribute GetCommandMethodAttribute(this ICustomAttributeProvider customAttributeProvider)
        {
            return customAttributeProvider.GetCustomAttribute<CommandMethodAttribute>();
        }

        public static object GetDefaultValue(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (attr == null)
            {
                if (propertyInfo.PropertyType == typeof(bool))
                    return true;
                return DBNull.Value;
            }

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
    }
}
