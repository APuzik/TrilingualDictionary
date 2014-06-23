using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class ConceptionDescription
    {
        public class ChangeablelPart
        {
            public string Type;
            public string Value;
        }

        private string m_Word;
        private string m_Explanation = "";

        public ChangeablelPart Changeable
        {
            get;
            set;
        }

        public ConceptionDescription(string word)
        {
            m_Word = word;
            Changeable = new ChangeablelPart();
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

        public string LangPart { get; set; }
    }
}
