using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test
{
    static class Container
    {
        private static CompositionContainer container;

        static Container()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(path));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Container).Assembly));
            container = new CompositionContainer(catalog);
        }

        public static T GetService<T>()
        {
            return container.GetExportedValue<T>();
        }
    }
}
