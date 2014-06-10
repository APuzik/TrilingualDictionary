using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class TrilingualDictionary
    {
        private Dictionary<int, Conception> m_Dictionary = new Dictionary<int, Conception>();

        public int AddConception(string word, Conception.LanguageId languageId)
        {
            int newConceptionId = m_Dictionary.Count > 0 ? m_Dictionary.Last().Key + 1 : 1;
            Conception newConception = new Conception(newConceptionId, word, languageId);
            m_Dictionary.Add(newConception.ConceptionId, newConception);
            return newConception.ConceptionId;            
        }

        public void AddDescriptionToConception(int conceptionId, string word, Conception.LanguageId languageId)
        {
            GetConception(conceptionId).AddDescription(word, languageId);
        }

        public void ChangeDescriptionOfConception(int conceptionId, string word, Conception.LanguageId languageId)
        {
            GetConception(conceptionId).ChangeDescription(word, languageId);
        }

        public void RemoveDescriptionFromConception(int conceptionId, Conception.LanguageId languageId)
        {
            Conception handledConception = GetConception(conceptionId);
            handledConception.RemoveDescription(languageId);

            if( handledConception.DescriptionsCount == 0 )
                RemoveConception(conceptionId);                
        }

        public void RemoveConception(int conceptionId)
        {
            m_Dictionary.Remove(conceptionId);
        }

        public int ConceptionsCount
        {
            get { return m_Dictionary.Count; }
        }

        private Conception GetConception(int conceptionId)
        {
            return m_Dictionary[conceptionId];
        }
    }
}
