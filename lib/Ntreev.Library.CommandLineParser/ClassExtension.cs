using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Ntreev.Library
{
    static class ClassExtension
    {
        public static CommandSwitchAttribute GetCommandSwitchAttribute(this PropertyDescriptor propertyDescriptor)
        {
            CommandSwitchAttribute attribute = propertyDescriptor.Attributes[typeof(CommandSwitchAttribute)] as CommandSwitchAttribute;

            if (attribute == null)
            {
                attribute = CommandSwitchAttribute.DefaultValue;
            }

            return attribute;
        }

        public static CommandSwitchAttribute GetCommandSwitchAttribute(this ICustomAttributeProvider customAttributeProvider)
        {
            CommandSwitchAttribute attribute = customAttributeProvider.GetCustomAttribute<CommandSwitchAttribute>();
            if (attribute == null)
                return CommandSwitchAttribute.DefaultValue;

            return attribute;
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider customAttributeProvider)
            where T : Attribute
        {
            object[] attrs = customAttributeProvider.GetCustomAttributes(typeof(T), true);
            if (attrs.Length == 0)
                return null;

            return attrs[0] as T;
        }

        public static string GetDisplayName(this ICustomAttributeProvider customAttributeProvider)
        {
            DisplayNameAttribute attribute = customAttributeProvider.GetCustomAttribute<DisplayNameAttribute>();
            if (attribute == null)
                return string.Empty;

            return attribute.DisplayName;
        }

        public static string GetDescription(this ICustomAttributeProvider customAttributeProvider)
        {
            DescriptionAttribute attribute = customAttributeProvider.GetCustomAttribute<DescriptionAttribute>();
            if (attribute == null)
                return string.Empty;

            return attribute.Description;
        }

        public static bool GetBrowsable(this ICustomAttributeProvider customAttributeProvider)
        {
            BrowsableAttribute attribute = customAttributeProvider.GetCustomAttribute<BrowsableAttribute>();
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
            TypeConverterAttribute attribute = customAttributeProvider.GetCustomAttribute<TypeConverterAttribute>();
            if (attribute == null)
                return TypeDescriptor.GetConverter(type);

            try
            {
                Type converterType = Type.GetType(attribute.ConverterTypeName);
                return Activator.CreateInstance(converterType) as TypeConverter;
            }
            catch (Exception)
            {
                return TypeDescriptor.GetConverter(type);
            }
        }

        public static bool IsCommandMethod(this MethodInfo methodInfo)
        {
            object[] attrs = methodInfo.GetCustomAttributes(typeof(CommandMethodAttribute), false);
            return attrs.Length > 0;
        }

        public static CommandMethodAttribute GetCommandMethodAttribute(this MethodInfo methodInfo)
        {
            object[] attrs = methodInfo.GetCustomAttributes(typeof(CommandMethodAttribute), false);

            if (attrs.Length > 0)
            {
                return attrs[0] as CommandMethodAttribute;
            }

            return null;
        }

        public static object GetValue(this ParameterInfo parameterInfo, string arg)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(parameterInfo.ParameterType);
            return converter.ConvertFromString(arg);
        }

        public static bool TryGetDefaultValue(this ParameterInfo parameterInfo, out object value)
        {
            value = null;
            object[] attrs = parameterInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
            if (attrs.Length == 0)
                return false;
            DefaultValueAttribute attr = attrs[0] as DefaultValueAttribute;
            value = attr.Value;
            return true;
        }

        public static string GetSimpleName(this Type type)
        {
            if (type == typeof(int))
                return "int";
            else if (type == typeof(string))
                return "string";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(uint))
                return "uint";
            else if (type == typeof(char))
                return "char";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(ushort))
                return "ushort";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(ulong))
                return "ulong";

            return type.Name;
        }
    }
}
