using Ntreev.Library;
using Ntreev.Library.Commands.Shell.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    [AttributeUsage(AttributeTargets.All)]
    class ShellSummaryAttribute : SummaryAttribute
    {
        public ShellSummaryAttribute(string resourceName)
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
