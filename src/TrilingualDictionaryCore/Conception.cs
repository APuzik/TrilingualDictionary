using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class Conception
    {
        private static LanguageId s_MainLanguage = LanguageId.Russian;
        public enum LanguageId
        {
            Russian = 0,
            English,
            Ukrainian
        };

        private int m_ConceptionId;
        private Dictionary<LanguageId, List<ConceptionDescription>> m_Descriptions = new Dictionary<LanguageId, List<ConceptionDescription>>();

        private string m_ConceptionArea;
        private int m_ConceptionParentId = 0;

        public Conception(int conceptionId, string word, LanguageId languageId)
        {
            m_ConceptionId = conceptionId;
            AddDescription(word, languageId);
        }

        internal void AddDescription(string word, LanguageId languageId)
        {
            if (!m_Descriptions.ContainsKey(languageId))
                m_Descriptions.Add(languageId, new List<ConceptionDescription>());

            m_Descriptions[languageId].Add(new ConceptionDescription(word));
        }

        internal void ChangeDescription(string word, LanguageId languageId, int pos)
        {
            GetConceptionDescription(languageId, pos).ChangeDescription(word);
        }

        internal void RemoveAllDescriptions(LanguageId languageId)
        {
            m_Descriptions.Remove(languageId);
        }

        internal void RemoveDescription(LanguageId languageId, int pos)
        {
            m_Descriptions[languageId].RemoveAt(pos);
        }

        public ConceptionDescription GetConceptionDescription(LanguageId languageId, int pos)
        {
            if( m_Descriptions.ContainsKey(languageId) )
                return m_Descriptions[languageId][pos];

            return new ConceptionDescription("");
        }

        public int ConceptionId
        {
            get { return m_ConceptionId; }
            set { m_ConceptionId = value; }
        }

        public int DescriptionsCount 
        {
            get { return m_Descriptions.Count; }
        }

        public string ActiveConceptionRegistryDescription
        {
            get 
            {
                if (!m_Descriptions.ContainsKey(MainLanguage))
                    return "";

                StringBuilder sb = new StringBuilder();
                string separator = "; ";
                for (int i = 0; i < m_Descriptions[MainLanguage].Count; i++)
                {
                    string description = GetConceptionDescription(MainLanguage, i).ConceptionRegistryDescription;
                    sb.AppendFormat("{0}{1}", description, separator);
                }
                sb = sb.Remove(sb.Length - separator.Length, separator.Length);

                return sb.ToString();// GetConceptionDescription(MainLanguage).ConceptionRegistryDescription; 
            }
        }

        public static LanguageId MainLanguage
        {
            get { return s_MainLanguage; }
            set { s_MainLanguage = value; }
        }

        public bool FindWithAccent(string textToSearch)
        {
            bool isFound = false;
            for(int i = 0; i < m_Descriptions[MainLanguage].Count && !isFound; i++)
            {
                string description = GetConceptionDescription(MainLanguage, i).ConceptionRegistryDescription;
                isFound = description.Contains(textToSearch);
            }
            return isFound;
        }

        public bool FindWoAccent(string textToSearch)
        {
            bool isFound = false;
            for (int i = 0; i < m_Descriptions[MainLanguage].Count && !isFound; i++)
            {
                string description = GetConceptionDescription(MainLanguage, i).ConceptionRegistryDescriptionWoAccents;
                isFound = description.Contains(textToSearch);
            }
            return isFound;
        }

        public bool Find(string textToSearch)
        {
            if (textToSearch.Contains('#'))
                return FindWithAccent(textToSearch);
            else
                return FindWoAccent(textToSearch);
        }

        public int ParentId
        {
            get { return m_ConceptionParentId; }
            set { m_ConceptionParentId = value; }
        }

        public string ConceptionArea
        {
            get { return m_ConceptionArea; }
            set { m_ConceptionArea = value; }
        }

        public string Topic { get; set; }

        public string TopicEx { get; set; }

        public string Link { get; set; }

        internal int GetLanguageDescriptionsCount(LanguageId languageId)
        {
            return m_Descriptions[languageId].Count;
        }
    }
}
