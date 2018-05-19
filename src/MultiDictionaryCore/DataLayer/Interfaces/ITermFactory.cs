using MultiDictionaryCore.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.DataLayer.Interfaces
{
    public interface ITermFactory
    {
        Term CreateTerm();
    }
}
