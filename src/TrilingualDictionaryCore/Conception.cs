using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;


namespace TrilingualDictionaryCore
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

    public class Conception
    {    
        private int m_ConceptionId;
        private Dictionary<LanguageId, List<ConceptionDescription>> m_Descriptions = new Dictionary<LanguageId, List<ConceptionDescription>>();

        private string m_ConceptionTopic;
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
            {
                m_Descriptions.Add(languageId, new List<ConceptionDescription>());
            }

            if (!m_Descriptions[languageId].Exists(x => x.ConceptionRegistryDescription == word))
            {
                ConceptionDescription newDescription = new ConceptionDescription(this, word);
                newDescription.DescriptionId = m_Descriptions[languageId].Count + 1;
                m_Descriptions[languageId].Add(newDescription);
            }
            else
                throw new TriLingException(string.Format("Description for language {0} already exists", languageId));
        }

        internal void AddDescription(ConceptionDescription desc, LanguageId languageId)
        {
            if (!m_Descriptions.ContainsKey(languageId))
            {
                m_Descriptions.Add(languageId, new List<ConceptionDescription>());
            }

            if (!m_Descriptions[languageId].Exists(x => x.ConceptionRegistryDescription == desc.ConceptionRegistryDescription))
            {
                desc.DescriptionId = m_Descriptions[languageId].Count + 1;
                m_Descriptions[languageId].Add(desc);
            }
            //else
            //    throw new TriLingException(string.Format("Description for language {0} already exists", languageId));
        }

        internal void ChangeDescription(string word, LanguageId languageId, int indexDescription)
        {
            try
            {
                GetConceptionDescription(languageId, indexDescription).ChangeDescription(word);
            }
            catch
            {
                throw;
            }
        }

        internal void RemoveDescription(LanguageId languageId, int indexDescription)
        {
            if (indexDescription >= 0 && indexDescription < m_Descriptions[languageId].Count)
                m_Descriptions[languageId].RemoveAt(indexDescription);
        }

        internal void RemoveDescription(LanguageId languageId, string description)
        {
            m_Descriptions[languageId].RemoveAll(x => x.ConceptionRegistryDescription == description);

        }

        public ConceptionDescription GetConceptionDescription(LanguageId languageId, int indexDescription)
        {
            if (m_Descriptions.ContainsKey(languageId))
                return m_Descriptions[languageId][indexDescription];
            else
                throw new TriLingException(string.Format("Description for language {0} is absent", languageId.ToString()));
        }

        public ConceptionDescription GetConceptionDescriptionOrEmpty(LanguageId languageId, int indexDescription)
        {
            if (m_Descriptions.ContainsKey(languageId))
                return m_Descriptions[languageId][indexDescription];
            else
                return new ConceptionDescription(this, "");
        }

        public bool IsHumanHandled { get; set; }

        public int ConceptionId
        {
            get { return m_ConceptionId; }
            set { m_ConceptionId = value; }
        }

        public int GetDescriptionsCount(LanguageId langId)
        {
            if (!m_Descriptions.ContainsKey(langId))
                return 0;
            return m_Descriptions[langId].Count;
        }

        public int DescriptionsCount
        {
            get
            {
                int sum = 0;
                foreach (List<ConceptionDescription> descs in m_Descriptions.Values)
                    sum += descs.Count;
                return sum;
            }
        }

        public bool FindWithAccent(string textToSearch, LanguageId language)
        {
            return m_Descriptions[language].Exists(x => x.ConceptionRegistryDescription.Contains(textToSearch));            
        }

        public bool FindWoAccent(string textToSearch, LanguageId language)
        {
            return m_Descriptions[language].Exists(x => x.ConceptionRegistryDescriptionWoAccents.Contains(textToSearch));            
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
            get;
            set;
        }

        internal void SaveConception(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", ConceptionId.ToString());
            if (ParentConception != null)
                writer.WriteAttributeString("ParentId", ParentConception.ConceptionId.ToString());

            if (IsHumanHandled)
                writer.WriteAttributeString("IsHumanHandled", IsHumanHandled.ToString());

            writer.WriteStartElement("Descriptions");
            foreach (KeyValuePair<LanguageId, List<ConceptionDescription>> decription in m_Descriptions)
            {
                writer.WriteStartElement("Description");
                writer.WriteAttributeString("LanguageId", decription.Key.ToString());
                foreach(ConceptionDescription desc in decription.Value)
                    desc.SaveDescription(writer);
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
            if (!string.IsNullOrWhiteSpace(parentId))
                conception.ParentId = Int32.Parse(parentId);

            string isHumanHandled = reader["IsHumanHandled"];
            if (!string.IsNullOrWhiteSpace(isHumanHandled))
                conception.IsHumanHandled = bool.Parse(isHumanHandled);

            //int descriptionId = 1;
            Dictionary<LanguageId, int> descIds = new Dictionary<LanguageId, int>();
            while (reader.Read())
            {
                switch (reader.Name)
                {
                    case "Description":
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            ConceptionDescription desc = ConceptionDescription.Create(reader, conception);
                            string sLangId = reader["LanguageId"];
                            LanguageId langId = (LanguageId)Enum.Parse(typeof(LanguageId), sLangId);
                            if(!descIds.ContainsKey(langId))                            
                            {
                                descIds.Add(langId, 1);
                            }

                            string descId = reader["DescriptionId"];
                            if (string.IsNullOrEmpty(descId))
                            {
                                desc.DescriptionId = descIds[langId];
                            }
                            else 
                            {
                                desc.DescriptionId = Int32.Parse(descId);
                            }
                            
                            descIds[langId] = desc.DescriptionId + 1;

                            conception.AddDescription(desc, langId);
                        }
                        break;
                    case "Descriptions":
                        if (reader.NodeType == XmlNodeType.EndElement)
                            return conception;
                        break;
                }

            }
            return conception;
        }

        //private void AddDescription(ConceptionDescription desc, LanguageId languageId)
        //{
        //    if (!m_Descriptions.ContainsKey(languageId))
        //    {
        //        m_Descriptions.Add(languageId, new List<ConceptionDescription>());
        //    }
                
        //    m_Descriptions[languageId].Add(desc);
        //}

        public string GetParentNameForSorting(LanguageId languageId)
        {
            //todo:
            //if (ParentConception != null)
            //    return ParentConception.GetConceptionDescriptionOrEmpty(languageId).ConceptionSortDescription;
            //else
            //{
            //    return GetConceptionDescriptionOrEmpty(languageId).ConceptionSortDescription;
            //}
            return "";
        }

        public string GetOwnNameForSorting(LanguageId languageId)
        {
            //todo:
            //string ownName = GetConceptionDescriptionOrEmpty(languageId).ConceptionSortDescription;
            //if (ownName.Contains("модулированный по плотности"))
            //{
            //    int ka = 1;
            //}
            //if (ParentConception != null)
            //    ownName = ownName.Replace("~", ParentConception.GetConceptionDescriptionOrEmpty(languageId).ConceptionSortDescription);

            string ownName = "";
            return ownName;
        }

        public bool IsPotentialUndefinded()
        {
            if (IsHumanHandled)
                return false;

            foreach (List<ConceptionDescription> descs in m_Descriptions.Values)
            {
                foreach (ConceptionDescription desc in descs)
                {
                    if (Regex.IsMatch(desc.ConceptionRegistryDescription, @"(\(|\[).+?(\)|\])"))
                        return true;
                }
            }
            return false;
        }

        public List<ConceptionDescription> GetAllConceptionDescriptions(LanguageId languageId)
        {
            if (m_Descriptions.ContainsKey(languageId))
                return m_Descriptions[languageId];
            return new List<ConceptionDescription>();
        }
    }
}
