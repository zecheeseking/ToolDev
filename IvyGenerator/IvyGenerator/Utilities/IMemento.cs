using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyGenerator.Utilities
{
    public interface IMemento<T>
    {
        IMemento<T> Restore(T target);
    }
}
