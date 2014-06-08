using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    class ConceptionDescription
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

        public string GetConceptionDescription()
        {
            return m_Word;
        }

        public string GetConceptionExplanation()
        {
            return m_Explanation;
        }
    }
}
