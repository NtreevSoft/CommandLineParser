//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;

namespace Ntreev.Library.Commands
{
    public sealed class ResourceDescriptionAttribute : UsageDescriptionProviderAttribute
    {
        private readonly string resourceName;
        private string relativePath;
        private string prefix;

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

        public string Prefix
        {
            get { return this.prefix ?? string.Empty; }
            set { this.prefix = value; }
        }

        public bool IsShared
        {
            get; set;
        }

        protected override IUsageDescriptionProvider CreateInstance(Type type)
        {
            var relativePath = this.RelativePath == string.Empty ? "." : this.RelativePath;
            if (relativePath.EndsWith("/") == false)
                relativePath += "/";
            var name = this.ResourceName == string.Empty ? type.Name : this.ResourceName;
            var relativeUri = new Uri(relativePath + name, UriKind.Relative);
            var uri = new Uri($"http://www.ntreev.com/{type.FullName.Replace('.', '/')}");
            var path = new Uri(uri, relativeUri);
            
            var resourceName = path.LocalPath.Replace('/', '.').TrimStart('.');
            return new ResourceUsageDescriptionProvider(resourceName) { IsShared = this.IsShared, Prefix = this.Prefix };
        }
    }
}