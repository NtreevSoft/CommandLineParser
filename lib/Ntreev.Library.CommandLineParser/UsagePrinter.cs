using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Ntreev.Library.Properties;

namespace Ntreev.Library
{
    public abstract class UsagePrinter
    {
        private readonly string command;
        private readonly Type type;

        public UsagePrinter(Type type, string command)
        {
            this.type = type;

            Assembly assembly = type.Assembly;

            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            if (File.Exists(command) == true)
            {
                this.command = Path.GetFileName(command);
            }
            else
            {
                this.command = command;
            }

            var titleAttr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            if (titleAttr != null)
            {
                this.Title = titleAttr.Title;
            }

            var descAttr = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            if (descAttr != null)
            {
                this.Description = descAttr.Description;
            }

            var companyAttr = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            if (companyAttr != null)
            {
                this.Company = companyAttr.Company;
            }

            var copyrightAttr = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            if (copyrightAttr != null)
            {
                this.Copyright = copyrightAttr.Copyright;
            }

            var productAttr = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            if (productAttr != null)
            {
                this.Product = productAttr.Product;
            }

            this.Version = assembly.GetName().Version.ToString();

            this.License = Resources.License;
        }

        public abstract void PrintUsage(TextWriter textWriter, int indentLevel);

        public abstract void PrintUsage(TextWriter textWriter, string memberName, int indentLevel);

        public void PrintUsage(TextWriter textWriter)
        {
            this.PrintUsage(textWriter, 0);
        }

        public void PrintUsage(TextWriter textWriter, string memberName)
        {
            this.PrintUsage(textWriter, memberName, 0);
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Company { get; set; }

        public string Copyright { get; set; }

        public string Product { get; set; }

        public string Version { get; set; }

        public string License { get; set; }

        public string Command { get { return this.command; } }

        public Type Type { get { return this.type; } }
    }
}
