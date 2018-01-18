using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public interface IUnknownArgument
    {
        bool Parse(string key, string value);
    }

    public interface ICustomCommandPropertyDescriptor
    {
        PropertyInfo[] Properties { get; }
    }
}
