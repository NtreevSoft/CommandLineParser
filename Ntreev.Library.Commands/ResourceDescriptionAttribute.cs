using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public sealed class ResourceDescriptionAttribute : UsageDescriptionProviderAttribute
    {
        private readonly string resourceName;
        private string relativePath;

        public ResourceDescriptionAttribute()
            : this(string.Empty)
        {
        }

        public ResourceDescriptionAttribute(string resourceName)
            : base(typeof(ResourceUsageDescriptionProvider))
        {
            this.resourceName = resourceName;
        }

        public string RelativePath
        {
            get { return this.relativePath ?? string.Empty; }
            set { this.relativePath = value; }
        }

        public string ResourceName
        {
            get { return this.resourceName ?? string.Empty; }
        }

        protected override IUsageDescriptionProvider CreateInstance(Type type)
        {
            if (this.ResourceName == string.Empty)
            {
                var relativeUri = new Uri(this.RelativePath, UriKind.Relative);
                var uri = new Uri($"http://www.ntreev.com/{type.FullName.Replace('.', '/')}");
                var path = new Uri(uri, relativeUri);
                var resourceName = path.LocalPath.Replace('/', '.').TrimStart('.') + "." + type.Name;
                return new ResourceUsageDescriptionProvider(resourceName);
            }
            else
            {
                return new ResourceUsageDescriptionProvider(this.ResourceName);
            }
        }
    }
}