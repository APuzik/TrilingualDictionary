using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class ConceptionDescription
    {
        private string m_Word;
        private string m_Explanation = "";

        public ConceptionDescription(string word)
        {
            m_Word = word;
        }

        internal void ChangeDescription(string word)
        {
            m_Word = word;
        }

        public string ConceptionRegistryDescription
        {
            get { return m_Word; }
        }

        public string ConceptionRegistryDescriptionWoAccents
        {
            get { return m_Word.Replace("#", ""); }
        }

        public string ConceptionExplanation
        {
            get { return m_Explanation; }
        }
    }
}
