using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    class Tracer : IDisposable
    {
        string message = null;

        public Tracer()
        {
            System.Diagnostics.Trace.Indent();
        }

        public Tracer(string message)
        {
            this.message = message;
            System.Diagnostics.Trace.WriteLine("begin: " + message);
            System.Diagnostics.Trace.Indent();
        }

        #region IDisposable 멤버

        public void Dispose()
        {
            System.Diagnostics.Trace.Unindent();
            if (message != null)
                System.Diagnostics.Trace.WriteLine("end  : " + message);
        }

        #endregion
    }
}
