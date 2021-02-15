using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    interface IConnectionStringFactory
    {
        string Build(string url);
        string Build();
    }
}
