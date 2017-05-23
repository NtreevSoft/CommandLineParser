using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Invoke
{
    [CommandStaticMethod(typeof(StaticCommand))]
    class Commands
    {
        public Commands()
        {
            this.Message = string.Empty;
        }

        [CommandMethod("init")]
        [CommandStaticProperty(typeof(GlobalSettings))]
        [CommandMethodProperty("Message", nameof(Message1))]
        public void Initialize(string path)
        {
            Console.WriteLine("{0} initialized.", path);
        }

        [CommandMethod]
        public void Update(string path)
        {
            Console.WriteLine("{0} updated.", path);
        }

        [CommandMethod]
        [CommandMethodProperty("Message")]
        [Browsable(false)]
        public void Delete(string path)
        {
            Console.WriteLine("{0} deleted.", path);
        }

        [CommandMethod]
        [CommandMethodProperty("Message")]
        public void Commit(string path)
        {
            if (this.Message == string.Empty)
                Console.WriteLine("{0} committed.", path);
            else
                Console.WriteLine("{0} committed. : {1}", path, this.Message);
        }

        [CommandMethod]
        [CommandMethodProperty("Message")]
        public void Add(params string[] items)
        {
            
        }

        [CommandProperty('m')]
        public string Message
        {
            get; set;
        }

        [CommandProperty('q')]
        public string Message1
        {
            get; set;
        }
    }
}
