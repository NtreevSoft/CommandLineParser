using SampleApplication.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication
{
    [AttributeUsage(AttributeTargets.All)]
    class GitDescriptionAttribute : DescriptionAttribute
    {
        public GitDescriptionAttribute(string resourceName)
            : base(GetResourceString(resourceName))
        {

        }

        private static string GetResourceString(string resourceName)
        {
            return (string)typeof(Resources).InvokeMember(resourceName,
                BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Static,
                null, null, null);
        }
    }
}
