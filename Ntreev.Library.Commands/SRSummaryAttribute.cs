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
    sealed class SRSummaryAttribute : SummaryAttribute
    {
        public SRSummaryAttribute(string summary)
        {
            this.Summary = SR.ResourceManager.GetString(summary, SR.Culture);
        }

        public SRSummaryAttribute(string description, string resourceSet)
        {
            var rm = new ResourceManager(resourceSet, Assembly.GetExecutingAssembly());
            this.Summary = rm.GetString(description);
        }
    }
}
