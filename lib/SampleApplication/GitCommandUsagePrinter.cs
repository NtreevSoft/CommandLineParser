//using Ntreev.Library;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using System.CodeDom.Compiler;
//using System.ComponentModel;
//using System.Reflection;

//namespace SampleApplication
//{
//    class GitCommandUsagePrinter : CommandUsagePrinter
//    {
//        public GitCommandUsagePrinter(string name, object instance)
//            : base(name, instance)
//        {

//        }

//        public override void PrintUsage(TextWriter textWriter)
//        {
//            using (var tw = new IndentedTextWriter(textWriter))
//            {
//                this.PrintUsage(tw);
//            }
//        }

        

//        public static T GetCustomAttribute<T>(ICustomAttributeProvider customAttributeProvider)
//            where T : Attribute
//        {
//            var attrs = customAttributeProvider.GetCustomAttributes(typeof(T), true);
//            if (attrs.Length == 0)
//                return null;

//            return attrs[0] as T;
//        }

//        private void PrintUsage(IndentedTextWriter textWriter)
//        {
//            this.PrintName(textWriter);
//            this.PrintSynopsis(textWriter);
//            this.PrintDescription(textWriter);
//            this.PrintOptions(textWriter);
//        }

//        private void PrintName(IndentedTextWriter textWriter)
//        {
//            var attr = GetCustomAttribute<GitSummaryAttribute>(this.Instance.GetType());
//            var summary = attr != null ? attr.Summary : string.Empty;
//            textWriter.WriteLine("Name");
//            textWriter.Indent++;
//            textWriter.WriteLine("git-{0} - {1}", this.Name, summary);
//            textWriter.Indent--;
//            textWriter.WriteLine();
//        }

//        private void PrintSynopsis(IndentedTextWriter textWriter)
//        {
//            var query = from item in this.Options
//                        let patternItems = new string[] { item.ShortNamePattern, item.NamePattern, }
//                        select string.Join(" | ", patternItems.Where(i => i != string.Empty));

//            var options = query.Aggregate("", (l, n) => l += "[" + n + "] ", item => item);

//            var switches = this.Switches.Aggregate("", (l, n) => l += "[" + n.DisplayName + "] ", item => item);

//            textWriter.WriteLine("Synopsis");
//            textWriter.Indent++;
//            textWriter.WriteLine("{0}[{1}]", options, switches);
//            textWriter.Indent--;
//            textWriter.WriteLine();
//        }

//        private void PrintDescription(IndentedTextWriter textWriter)
//        {
//            textWriter.WriteLine("Description");
//            textWriter.Indent++;
//            textWriter.WriteMultiline(this.Description);
//            textWriter.Indent--;
//            textWriter.WriteLine();
//        }

//        private void PrintOptions(IndentedTextWriter textWriter)
//        {
//            textWriter.WriteLine("Options");
//            textWriter.Indent++;
//            foreach (var item in this.Switches)
//            {
//                this.PrintSwitch(textWriter, item);
//            }
//            foreach (var item in this.Options)
//            {
//                this.PrintOption(textWriter, item);
//            }
//            textWriter.Indent--;
//            textWriter.WriteLine();
//        }

//        private void PrintSwitch(IndentedTextWriter textWriter, SwitchDescriptor descriptor)
//        {
//            if (descriptor.Description == string.Empty)
//                return;

//            textWriter.WriteLine(descriptor.DisplayName);
//            textWriter.Indent++;
//            textWriter.WriteMultiline(descriptor.Description);
//            textWriter.Indent--;
//            textWriter.WriteLine();
//        }

//        private void PrintOption(IndentedTextWriter textWriter, SwitchDescriptor descriptor)
//        {
//            if (descriptor.ShortNamePattern != string.Empty)
//                textWriter.WriteLine(descriptor.ShortNamePattern);
//            if (descriptor.NamePattern != string.Empty)
//                textWriter.WriteLine(descriptor.NamePattern);

//            textWriter.Indent++;
//            textWriter.WriteMultiline(descriptor.Description);
//            textWriter.Indent--;
//            textWriter.WriteLine();
//        }

//        private string GetOptionsString(IEnumerable<SwitchDescriptor> switches)
//        {
//            var query = from item in switches
//                        let patternItems = new string[] { item.ShortNamePattern, item.NamePattern, }
//                        select string.Join(" | ", patternItems.Where(i => i != string.Empty));

//            return string.Join(" ", query.Select(item => "[" + item + "]"));
//        }

//        private string GetSwitchesString(IEnumerable<SwitchDescriptor> switches)
//        {
//            return string.Join(" ", switches.Select(item => "[" + item.DisplayName + "]"));
//        }
//    }
//}
