using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.All)]
    sealed class SRDescriptionAttribute : DescriptionAttribute
    {
        public SRDescriptionAttribute(string description)
        {
            DescriptionValue = SR.ResourceManager.GetString(description, SR.Culture);
        }

        public SRDescriptionAttribute(string description, string resourceSet)
        {
            ResourceManager rm = new ResourceManager(resourceSet, Assembly.GetExecutingAssembly());
            DescriptionValue = rm.GetString(description);
            //Fx.Assert(DescriptionValue != null, string.Format(CultureInfo.CurrentCulture, "String resource {0} not found.", new object[] { description }));
        }
    }

    //sealed partial class SR
    //{
    //    internal static string GetString(string name, params object[] args)
    //    {
    //        return GetString(resourceCulture, name, args);
    //    }
    //    internal static string GetString(CultureInfo culture, string name, params object[] args)
    //    {
    //        if (args != null && args.Length > 0)
    //        {
    //            return string.Format(culture, name, args);
    //        }
    //        else
    //        {
    //            return name;
    //        }
    //    }
    //}
}
