using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;

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

        public class LanguageIdToSting
        {
            private static Dictionary<LanguageId, Dictionary<LanguageId, string>> m_Descriptions = new Dictionary<LanguageId, Dictionary<LanguageId, string>>();

            static LanguageIdToSting()
            {
                m_Descriptions[LanguageId.Russian] = new Dictionary<LanguageId, string>();
                m_Descriptions[LanguageId.Russian][LanguageId.Russian] = "Русский";
                m_Descriptions[LanguageId.Russian][LanguageId.English] = "Английский";
                m_Descriptions[LanguageId.Russian][LanguageId.Ukrainian] = "Украинский";
                m_Descriptions[LanguageId.English] = new Dictionary<LanguageId, string>();
                m_Descriptions[LanguageId.English][LanguageId.Russian] = "Russian";
                m_Descriptions[LanguageId.English][LanguageId.English] = "English";
                m_Descriptions[LanguageId.English][LanguageId.Ukrainian] = "Ukrainian";
                m_Descriptions[LanguageId.Ukrainian] = new Dictionary<LanguageId, string>();
                m_Descriptions[LanguageId.Ukrainian][LanguageId.Russian] = "Росiйська";
                m_Descriptions[LanguageId.Ukrainian][LanguageId.English] = "Англiйська";
                m_Descriptions[LanguageId.Ukrainian][LanguageId.Ukrainian] = "Українська";
            }

            public static string GetDescription(LanguageId languageRequired, LanguageId idDescription)
            {
                return m_Descriptions[languageRequired][idDescription];
            }
        };

        
        private int m_ConceptionId;
        private Dictionary<LanguageId, ConceptionDescription> m_Descriptions = new Dictionary<LanguageId, ConceptionDescription>();

        private string m_ConceptionArea;
        private int m_ConceptionParentId = 0;
        private Conception m_ParentConception = null;

        private Conception(int conceptionId)
        {
            m_ConceptionId = conceptionId;
            m_ConceptionParentId = 0;
            IsHumanHandled = false;
        }

        public Conception(int conceptionId, string word, LanguageId languageId)
        {
            m_ConceptionId = conceptionId;
            m_ConceptionParentId = 0;
            AddDescription(word, languageId);
        }

        internal void AddDescription(string word, LanguageId languageId)
        {
            if (!m_Descriptions.ContainsKey(languageId))
                m_Descriptions.Add(languageId, new ConceptionDescription(word));
            else
                throw new TriLingException(string.Format("Description for language {0} already exists", languageId));//LanguageIdToSting.GetDescription(s_MainLanguage, languageId)));
        }

        internal void ChangeDescription(string word, LanguageId languageId)
        {
            try
            {
                GetConceptionDescription(languageId).ChangeDescription(word);
            }
            catch
            {
                throw;
            }
        }

        internal void RemoveDescription(LanguageId languageId)
        {
            m_Descriptions.Remove(languageId);
        }

        public ConceptionDescription GetConceptionDescription(LanguageId languageId)
        {
            if( m_Descriptions.ContainsKey(languageId) )
                return m_Descriptions[languageId];
            else
                throw new TriLingException(string.Format("Description for language {0} is absent", languageId.ToString()));
        }

        public ConceptionDescription GetConceptionDescriptionOrEmpty(LanguageId languageId)
        {
            if (m_Descriptions.ContainsKey(languageId))
                return m_Descriptions[languageId];
            else
                return new ConceptionDescription("");
        }

        public bool IsHumanHandled { get; set; }

        public int ConceptionId
        {
            get { return m_ConceptionId; }
            set { m_ConceptionId = value; }
        }

        public int DescriptionsCount 
        {
            get { return m_Descriptions.Count; }
        }
        
        public bool FindWithAccent(string textToSearch, LanguageId language)
        {
            string description = GetConceptionDescriptionOrEmpty(language).ConceptionRegistryDescription;
            return description.Contains(textToSearch);
        }

        public bool FindWoAccent(string textToSearch, LanguageId language)
        {
            string description = GetConceptionDescriptionOrEmpty(language).ConceptionRegistryDescriptionWoAccents;
            return description.Contains(textToSearch);           
        }

        public bool Find(string textToSearch, LanguageId language)
        {
            if (textToSearch.Contains('#'))
                return FindWithAccent(textToSearch, language);
            else
                return FindWoAccent(textToSearch, language);
        }

        public int ParentId
        {
            get { return m_ConceptionParentId; }
            set { m_ConceptionParentId = value; }
        }

        public Conception ParentConception
        {
            get { return m_ParentConception; }
            set { m_ParentConception = value; }
        }

        public List<int> LinkedIds 
        { 
            get; set;
        }

        internal void SaveConception(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", ConceptionId.ToString());
            if( ParentConception != null )
                writer.WriteAttributeString("ParentId", ParentConception.ConceptionId.ToString());

            if (IsHumanHandled)
                writer.WriteAttributeString("IsHumanHandled", IsHumanHandled.ToString());

            writer.WriteStartElement("Descriptions");
            foreach (KeyValuePair<LanguageId, ConceptionDescription> decription in m_Descriptions)
            {
                writer.WriteStartElement("Description");
                writer.WriteAttributeString("LanguageId", decription.Key.ToString());
                decription.Value.SaveDescription(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();            
        }

        internal static Conception Create(XmlReader reader)
        {
            string id = reader["Id"];
            int i = Int32.Parse(id);
            Conception conception = new Conception(i);
            string parentId = reader["ParentId"];
            if( !string.IsNullOrWhiteSpace(parentId) )
                conception.ParentId = Int32.Parse(parentId);

            string isHumanHandled = reader["IsHumanHandled"];
            if (!string.IsNullOrWhiteSpace(isHumanHandled))
                conception.IsHumanHandled = bool.Parse(isHumanHandled);

            while (reader.Read())
            {
                switch (reader.Name)
                {
                    case "Description":
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            ConceptionDescription desc = ConceptionDescription.Create(reader);
                            string langId = reader["LanguageId"];
                            conception.AddDescription(desc, (LanguageId)Enum.Parse(typeof(LanguageId), langId));
                        }
                        break;
                    case "Descriptions":
                        if( reader.NodeType == XmlNodeType.EndElement )
                            return conception;
                        break;
                }

            }
            return conception;
        }

        private void AddDescription(ConceptionDescription desc, LanguageId languageId)
        {
            m_Descriptions.Add(languageId, desc);        
        }

        public string GetParentNameForSorting(LanguageId languageId)
        {
            if (ParentConception != null)
                return ParentConception.GetConceptionDescriptionOrEmpty(languageId).ConceptionSortDescription;
            else
            {
                return GetConceptionDescriptionOrEmpty(languageId).ConceptionSortDescription;
            }
        }

        public string GetOwnNameForSorting(LanguageId languageId)
        {
            string ownName = GetConceptionDescriptionOrEmpty(languageId).ConceptionSortDescription;
            if (ownName.Contains("модулированный по плотности"))
            {
                int ka = 1;
            }
            if (ParentConception != null)
                ownName = ownName.Replace("~", ParentConception.GetConceptionDescriptionOrEmpty(languageId).ConceptionSortDescription);

            return ownName;
        }

        public bool IsPotentialUndefinded()
        {
            if (IsHumanHandled)
                return false;

            foreach (ConceptionDescription desc in m_Descriptions.Values)
            {
                if (Regex.IsMatch(desc.ConceptionRegistryDescription, @"(\(|\[).+?(\)|\])"))
                    return true;
            }
            return false;
        }
    }
}
