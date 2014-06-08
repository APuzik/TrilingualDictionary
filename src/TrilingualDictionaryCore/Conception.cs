using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    class Conception
    {
        private int m_ConceptionId;
        private Dictionary<int, ConceptionDescription> m_Descriptions = new Dictionary<int, ConceptionDescription>();

        public Conception(int conceptionId, string word, int languageId)
        {
            m_ConceptionId = conceptionId;
            AddDescription(word, languageId);
        }
        
        internal void AddDescription(string word, int languageId)
        {
            m_Descriptions.Add(languageId, new ConceptionDescription(word));
        }

        internal void ChangeDescription(string word, int languageId)
        {
            GetConceptionDescription(languageId).ChangeDescription(word);
        }

        internal void RemoveDescription(int languageId)
        {
            m_Descriptions.Remove(languageId);
        }

        public ConceptionDescription GetConceptionDescription(int languageId)
        {
            return m_Descriptions[languageId];
        }

        public int ConceptionId
        {
            get { return m_ConceptionId; }
        }
    }
}
