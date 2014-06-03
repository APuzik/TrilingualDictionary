using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class TrilingualDictionary
    {
        List<string> m_Dictionary = new List<string>();

        public int addConception(string word, int languageId)
        {
            m_Dictionary.Add(word);
            return getConceptionsCount();
            
        }

        public int getConceptionsCount()
        {
            return m_Dictionary.Count;
        }
    }
}
