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

        public static CommandSwitchAttribute GetCommandSwitchAttribute(this ParameterInfo parameterInfo)
        {
            object[] attrs = parameterInfo.GetCustomAttributes(typeof(CommandSwitchAttribute), true);
            if(attrs.Length == 0)
                return CommandSwitchAttribute.DefaultValue;

            return attrs[0] as CommandSwitchAttribute;
        }

        public static string GetDescription(this ParameterInfo parameterInfo)
        {
            object[] attrs = parameterInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs.Length == 0)
                return string.Empty;

            DescriptionAttribute descriptionAttribute = attrs[0] as DescriptionAttribute;
            return descriptionAttribute.Description;
        }

        public static string GetDescription(this MethodInfo methodInfo)
        {
            object[] attrs = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs.Length == 0)
                return string.Empty;

            DescriptionAttribute descriptionAttribute = attrs[0] as DescriptionAttribute;
            return descriptionAttribute.Description;
        }

        public static string GetDisplayName(this ParameterInfo parameterInfo)
        {
            object[] attrs = parameterInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (attrs.Length == 0)
                return string.Empty;

            DisplayNameAttribute displayNameAttribute = attrs[0] as DisplayNameAttribute;
            return displayNameAttribute.DisplayName;
        }

        public static TypeConverter GetConverter(this ParameterInfo parameterInfo)
        {
            object[] attrs = parameterInfo.GetCustomAttributes(typeof(TypeConverterAttribute), true);
            if (attrs.Length == 0)
                return TypeDescriptor.GetConverter(parameterInfo.ParameterType);

            //TypeConverter converter = Activator.CreateInstance(
            TypeConverterAttribute typeConverterAttribute = attrs[0] as TypeConverterAttribute;
            try
            {
                Type converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);
                return Activator.CreateInstance(converterType) as TypeConverter;
            }
            catch (Exception)
            {
                return TypeDescriptor.GetConverter(parameterInfo.ParameterType);
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
            if (type == typeof(uint))
                return "uint";
            else if (type == typeof(string))
                return "string";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(float))
                return "float";
            return type.Name;
        }
    }
}
