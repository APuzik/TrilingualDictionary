using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class Conception
    {
        public enum LanguageId
        {
            Russian = 0,
            English,
            Ukrainian
        };

        private int m_ConceptionId;
        private Dictionary<LanguageId, ConceptionDescription> m_Descriptions = new Dictionary<LanguageId, ConceptionDescription>();

        public Conception(int conceptionId, string word, LanguageId languageId)
        {
            m_ConceptionId = conceptionId;
            AddDescription(word, languageId);
        }

        internal void AddDescription(string word, LanguageId languageId)
        {
            m_Descriptions.Add(languageId, new ConceptionDescription(word));
        }

        internal void ChangeDescription(string word, LanguageId languageId)
        {
            GetConceptionDescription(languageId).ChangeDescription(word);
        }

        internal void RemoveDescription(LanguageId languageId)
        {
            m_Descriptions.Remove(languageId);
        }

        public ConceptionDescription GetConceptionDescription(LanguageId languageId)
        {
            return m_Descriptions[languageId];
        }

        public int ConceptionId
        {
            get { return m_ConceptionId; }
        }

        public int DescriptionsCount 
        {
            get { return m_Descriptions.Count; }
        }
    }
}
