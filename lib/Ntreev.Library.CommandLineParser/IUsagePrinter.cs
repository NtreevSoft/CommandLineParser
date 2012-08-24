using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ntreev.Library
{
    public interface IUsagePrinter
    {
        void PrintUsage(TextWriter textWriter);
        void PrintUsage(TextWriter textWriter, int indentLevel);
        void PrintUsage(TextWriter textWriter, string memberName);
        void PrintUsage(TextWriter textWriter, string memberName, int indentLevel);
    }
}
