using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.MultiCommand
{
    class Commands
    {
        public Commands()
        {
            this.Message = string.Empty;
        }

        [CommandMethod("init")]
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
        [CommandMethodSwitch("Message")]
        public void Delete(string path)
        {
            Console.WriteLine("{0} deleted.", path);
        }

        [CommandMethod]
        [CommandMethodSwitch("Message")]
        public void Commit(string path)
        {
            if (this.Message == string.Empty)
                Console.WriteLine("{0} committed.", path);
            else
                Console.WriteLine("{0} committed. : {1}", path, this.Message);
        }

        [CommandSwitch(ShortName = 'm')]
        [Browsable(false)]
        public string Message
        {
            get; set;
        }
    }
}
