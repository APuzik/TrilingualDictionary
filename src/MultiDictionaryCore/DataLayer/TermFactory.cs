using MultiDictionaryCore.DataLayer.Interfaces;
using MultiDictionaryCore.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.DataLayer
{
    internal class TermFactory : ITermFactory
    {
        public Term CreateTerm()
        {
            return new Term();
        }
    }
}
