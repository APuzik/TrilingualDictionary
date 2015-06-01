using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlServerCe;


namespace TrilingualDictionaryCore
{
    public enum LanguageId
    {
        Undefined = -1,
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

        private string m_Topic;
        private string m_Semantic;
        private string m_Link;

        private int m_ConceptionParentId = 0;
        private Conception m_ParentConception = null;

        private Conception(int conceptionId)
        {
            m_ConceptionId = conceptionId;
            m_ConceptionParentId = 0;
            IsHumanHandled = false;
        }

        public string Topic
        {
            get { return m_Topic; }
            set { m_Topic = value; }
        }

        public string Semantic
        {
            get { return m_Semantic; }
            set { m_Semantic = value; }
        }

        public string Link
        {
            get { return m_Link; }
            set { m_Link = value; }
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
            //else
            //    throw new TriLingException(string.Format("Description for language {0} already exists", languageId));
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

        internal void ChangeDescription(string word, LanguageId languageId, string textOld)
        {
            try
            {
                GetConceptionDescription(languageId, textOld).ChangeDescription(word);
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

        public ConceptionDescription GetConceptionDescription(LanguageId languageId, string text)
        {
            if (m_Descriptions.ContainsKey(languageId))
                return m_Descriptions[languageId].Find( x => string.Compare(x.ConceptionRegistryDescription, text, true) == 0);
            else
                throw new TriLingException(string.Format("Description for language {0} is absent", languageId.ToString()));
        }

        public ConceptionDescription GetConceptionDescriptionOrEmpty(LanguageId languageId, int indexDescription)
        {
            if (m_Descriptions.ContainsKey(languageId) && indexDescription < m_Descriptions[languageId].Count)
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

            if (!string.IsNullOrWhiteSpace(Topic))
                writer.WriteAttributeString("Topic", Topic);
            if (!string.IsNullOrWhiteSpace(Semantic))
                writer.WriteAttributeString("Semantic", Semantic);
            if (!string.IsNullOrWhiteSpace(Link))
                writer.WriteAttributeString("Link", Link);

            writer.WriteStartElement("Descriptions");
            foreach (KeyValuePair<LanguageId, List<ConceptionDescription>> decription in m_Descriptions)
            {
                foreach(ConceptionDescription desc in decription.Value)
                {
                    writer.WriteStartElement("Description");
                    writer.WriteAttributeString("LanguageId", decription.Key.ToString());
                    desc.SaveDescription(writer);
                    writer.WriteEndElement();
                }                
            }
            writer.WriteEndElement();
        }

        public void SaveConception(SqlCeConnection conn)
        {
            if (IsConceptionExsists(conn))
                UpdateConception(conn);
            else
                InsertConception(conn);
        }

        private void UpdateConception(SqlCeConnection conn)
        {
            UpdateConceptionData(conn);
            SaveDescriptions(conn);
        }

        public void InsertConception(SqlCeConnection conn)
        {
            InsertConceptionData(conn);
            SaveDescriptions(conn);
        }

        private void UpdateConceptionData(SqlCeConnection conn)
        {
            int idTopic = SaveTopic(this.Topic, conn);
            int idSemantic = SaveSemantic(this.Semantic, conn);

            //string query = "UPDATE Conception SET Id=@ID, ParentId=@ParentId, Topic=@Topic, Semantic=@Semantic";
            string query = "UPDATE Conception SET ParentId=@ParentId, Topic=@Topic, Semantic=@Semantic";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            CreateCommandParams(idTopic, idSemantic, cmd);

            cmd.ExecuteNonQuery();
        }

        public void InsertConceptionData(SqlCeConnection conn)
        {
            int idTopic = SaveTopic(this.Topic, conn);
            int idSemantic = SaveSemantic(this.Semantic, conn);

            //string query = "INSERT INTO Conception(Id, ParentId, Topic, Semantic) VALUES(@ID, @ParentId, @Topic, @Semantic)";
            string query = "INSERT INTO Conception(ParentId, Topic, Semantic) VALUES(@ParentId, @Topic, @Semantic)";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            CreateCommandParams(idTopic, idSemantic, cmd);

            cmd.ExecuteNonQuery();
        }

        private void CreateCommandParams(int idTopic, int idSemantic, SqlCeCommand cmd)
        {
            //cmd.Parameters.Add("@ID", SqlDbType.Int);
            cmd.Parameters.Add("@ParentId", SqlDbType.Int);
            cmd.Parameters.Add("@Topic", SqlDbType.Int);
            cmd.Parameters.Add("@Semantic", SqlDbType.Int);

            //cmd.Parameters["@ID"].Value = this.ConceptionId;
            cmd.Parameters["@ParentId"].Value = this.ParentId;
            cmd.Parameters["@Topic"].Value = (idTopic == 0 ? DBNull.Value : (object)idTopic);
            cmd.Parameters["@Semantic"].Value = (idSemantic == 0 ? DBNull.Value : (object)idSemantic);
        }

        private void SaveDescriptions(SqlCeConnection conn)
        {
            foreach (KeyValuePair<LanguageId, List<ConceptionDescription>> decription in m_Descriptions)
            {
                foreach (ConceptionDescription desc in decription.Value)
                {
                    desc.SaveDescription(conn, decription.Key);
                }
            }
        }

        private bool IsConceptionExsists(SqlCeConnection conn)
        {
            if (m_ConceptionId == 0)
                return false;

            string query = string.Format("SELECT Id FROM Conception WHERE Id={0}", m_ConceptionId);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();//CommandBehavior.CloseConnection);
            if (reader.Read())
            {
                return true;
            }

            return false;
        }
                
        private int SaveSemantic(string semDescription, SqlCeConnection conn)
        {
            if (string.IsNullOrEmpty(semDescription))
                return 0;

            int semId = GetSemanticByLang(semDescription, LanguageId.Russian, conn);
            if (semId == 0)
                semId = InsertSemantic(semDescription, LanguageId.Russian, conn);

            return semId;
        }

        private int InsertSemantic(string semDescription, LanguageId languageId, SqlCeConnection conn)
        {
            string query = "INSERT INTO Semantic (DefaultName) VALUES(NULL)";//(Id) VALUES(@ID)";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;        

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";//SCOPE_IDENTITY();";

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            var id = cmd.ExecuteScalar();

            int semId = Convert.ToInt32(id);

            if (semId != 0)
            {
                query = "INSERT INTO SemanticTranslation (SemId, LangForId, Translation) VALUES(@SemanticId, @Langid, @Translation)";

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@SemanticId", SqlDbType.Int);
                cmd.Parameters.Add("@Langid", SqlDbType.Int);
                cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

                cmd.Parameters["@SemanticId"].Value = semId;
                cmd.Parameters["@Langid"].Value = (int)languageId + 1;
                cmd.Parameters["@Translation"].Value = semDescription;

                cmd.ExecuteNonQuery();
            }
            return semId;

        }

        private int GetSemanticByLang(string semDescription, LanguageId languageId, SqlCeConnection conn)
        {
            string query = string.Format("SELECT SemId FROM SemanticTranslation WHERE LangForId={0} AND Translation='{1}'", (int)languageId + 1, semDescription);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();//CommandBehavior.CloseConnection);
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }

        private int SaveTopic(string topicDescription, SqlCeConnection conn)
        {
            if (string.IsNullOrEmpty(topicDescription))
                return 0;

            int topicId = GetTopicByLang(topicDescription, LanguageId.Russian, conn);
            if (topicId == 0)
                topicId = InsertTopic(topicDescription, LanguageId.Russian, conn);

            return topicId;
        }

        private int InsertTopic(string topicDescription, LanguageId languageId, SqlCeConnection conn)
        {
            string query = "INSERT INTO Topic (DefaultName) VALUES(NULL)";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";//SCOPE_IDENTITY()";

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            var id = cmd.ExecuteScalar();

            int topicId = Convert.ToInt32(id);

            if (topicId != 0)
            {
                query = "INSERT INTO TopicTranslation (TopicId, LangIdFor, Translation) VALUES(@TopicId, @Langid, @Translation)";

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@TopicId", SqlDbType.Int);
                cmd.Parameters.Add("@Langid", SqlDbType.Int);
                cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

                cmd.Parameters["@TopicId"].Value = topicId;
                cmd.Parameters["@Langid"].Value = (int)languageId + 1;
                cmd.Parameters["@Translation"].Value = topicDescription;

                cmd.ExecuteNonQuery();
            }
            return topicId;
        }

        private int GetTopicByLang(string topicDescription, LanguageId languageId, SqlCeConnection conn)
        {
            string query = string.Format("SELECT TopicId FROM TopicTranslation WHERE LangIdFor={0} AND Translation='{1}'", (int)languageId + 1, topicDescription);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();//CommandBehavior.CloseConnection);
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }

        internal static Conception Create(XmlReader reader)
        {
            string id = reader["Id"];
            int i = Int32.Parse(id);
            if(i  == 55)
            {
                int qq = 1;
            }
            Conception conception = new Conception(i);
            string parentId = reader["ParentId"];
            if (!string.IsNullOrWhiteSpace(parentId))
                conception.ParentId = Int32.Parse(parentId);

            string isHumanHandled = reader["IsHumanHandled"];
            if (!string.IsNullOrWhiteSpace(isHumanHandled))
                conception.IsHumanHandled = bool.Parse(isHumanHandled);

            conception.Topic = ConceptionDescription.GetXmlAttribute(reader, "Topic");
            conception.Semantic = ConceptionDescription.GetXmlAttribute(reader, "Semantic");
            conception.Link = ConceptionDescription.GetXmlAttribute(reader, "Link");

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
            if (ParentConception != null)
                return ParentConception.GetConceptionDescriptionOrEmpty(languageId, 0).ConceptionSortDescription;
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

        public List<ConceptionDescription> GetAnyDescriptions()
        {
            foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
            {
                if (langId == LanguageId.Undefined)
                    continue;

                List<ConceptionDescription> descs = GetAllConceptionDescriptions(langId);
                if (descs.Count > 0)
                    return descs;
            }

            return new List<ConceptionDescription>();
        }

        public ConceptionDescription ConceptionDescription
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        internal TopicTranslation TopicTranslation
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        internal SemanticTranslation SemanticTranslation
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
