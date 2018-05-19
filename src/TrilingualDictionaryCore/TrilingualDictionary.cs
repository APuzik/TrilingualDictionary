using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlServerCe;

namespace TrilingualDictionaryCore
{
    public class TrilingualDictionary
    {
        private int m_LastId = 0;
        private List<LanguageId> m_Languages = new List<LanguageId>();

        private List<Conception> m_AllConceptions = new List<Conception>();
        private List<Conception> m_Parents = null; 
        private Dictionary<LanguageId, List<ConceptionDescription>> m_AllDescriptions = new Dictionary<LanguageId,List<ConceptionDescription>>();

        private Dictionary<LanguageId, Dictionary<int, ITranslatable>> m_AllTopics = new Dictionary<LanguageId, Dictionary<int, ITranslatable>>();
        private Dictionary<LanguageId, Dictionary<int, ITranslatable>> m_AllSemantics = new Dictionary<LanguageId, Dictionary<int, ITranslatable>>();
        private Dictionary<LanguageId, Dictionary<int, ITranslatable>> m_AllLangParts = new Dictionary<LanguageId, Dictionary<int, ITranslatable>>();
        private Dictionary<LanguageId, Dictionary<LanguageId, ITranslatable>> m_AllLanguages = new Dictionary<LanguageId, Dictionary<LanguageId, ITranslatable>>();
        private Dictionary<LanguageId, Dictionary<int, ITranslatable>> m_AllChangables = new Dictionary<LanguageId, Dictionary<int, ITranslatable>>();

        private Dictionary<int, Conception> m_Dictionary = new Dictionary<int, Conception>();
        //private Dictionary<int, List<ConceptionDescription>> m_Descriptions = new Dictionary<LanguageId, List<ConceptionDescription>>();
        string m_ConnectionString = @"Data Source=.\TrilingualDictionary.sdf;Password=Password1";
        //string m_ConnectionString = @"Data Source=D:\AS\_Aspirantura\Projects\TrilingualDictionary\src\Data\TrilingualDictionary.sdf;Max Database Size=4091;Password=***********";
            
        public TrilingualDictionary()
        {
            try
            {
                LoadLanguages();
            }
            catch(Exception ex)
            {
                int k = 1;
            }
        }

        private void LoadLanguages()
        {
            using (SqlCeConnection connection = new SqlCeConnection(m_ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id FROM Languages";

                SqlCeCommand cmd = new SqlCeCommand(query);
                cmd.Connection = connection;
                SqlCeDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    m_Languages.Add((LanguageId)(id - 1));
                }
            }
        }

        string GetChangeableTranslation(LanguageId langFor, int changeableTypeId)
        {
            if (m_AllChangables.ContainsKey(langFor))
            {
                if (m_AllChangables[langFor].ContainsKey(changeableTypeId))
                {
                    return m_AllChangables[langFor][changeableTypeId].Translation;
                }
            }
            return "";
        }

        string GetLanguageTranslation(LanguageId langFor, LanguageId langNeeded)
        {
            if (m_AllLanguages.ContainsKey(langFor))
            {
                if (m_AllLanguages[langFor].ContainsKey(langNeeded))
                {
                    return m_AllLanguages[langFor][langNeeded].Translation;
                }
            }
            return "";
        }

        string GetPartOfSpeechTranslation(LanguageId langFor, int partOfSpeechId)
        {
            if (m_AllLangParts.ContainsKey(langFor))
            {
                if (m_AllLangParts[langFor].ContainsKey(partOfSpeechId))
                {
                    return m_AllLangParts[langFor][partOfSpeechId].Translation;
                }
            }
            return "";
        }

        string GetSemanticTranslation(LanguageId langFor, int semId)
        {
            if (m_AllSemantics.ContainsKey(langFor))
            {
                if (m_AllSemantics[langFor].ContainsKey(semId))
                {
                    return m_AllSemantics[langFor][semId].Translation;
                }
            }
            return "";
        }

        string GetTopicTranslation(LanguageId langFor, int topicId)
        {
            if (m_AllTopics.ContainsKey(langFor))
            {
                if (m_AllTopics[langFor].ContainsKey(topicId))
                {
                    return m_AllTopics[langFor][topicId].Translation;
                }
            }
            return "";
        }

        private static readonly string[] m_TopicMarks = new string[]
        {
            "бион.",//бионика                                             		
            "біон.",//бионика
            "вчт", //вычислительная тех-ника
            "кв. рф", //квантовая радиофизи-ка
            "кв. эл.", //квантовая электрони-ка
            "кв. ел.", //квантовая электрони-ка
            "крист.", //кристаллография 
            "магн.", //магнетизм
            "мат.", //математика
            "микр.", //микроэлектроника
            "мікр.", //микроэлектроника
            "опт.", //оптика
            "пп", //полупроводники
            "рлк", //радиолокация
            "рф", //радиофизика
            "сп", //сверхпроводники, сверхпроводимость
            "техн.", //техника
            "тлв", //телевидение
            "тлг", //телеграфия
            "тлф", //телефония
            "тн", //теория надёжности
            "физ.", //физика
            "фіз.", //физика
            "фтт" //физика твёрдого тела
        };

        private static readonly string[] m_ChangeableMarks = new string[]
        {
            "род.", //родительный
        };

        private static readonly string[] m_LangMarks = new string[]
        {
            "прил.", //прилагательное
            "прич.", //причастие
            "сущ.", //существительное
        };

        private static readonly string[] m_LinkMarks = new string[]
        {
            "см.", //смотри
//            "см. еще", //смотри ещё
//            "см. ещё", //смотри ещё
        };

        private static readonly string[] m_OtherMarks = new string[]
        {
            "напр.", //например
            "собир.", //собирательное
        };

        public static string[] TopicMarks
        {
            get { return m_TopicMarks; }
        }

        public static string[] ChangeableMarks
        {
            get { return m_ChangeableMarks; }
        }

        public static string[] LangMarks
        {
            get { return m_LangMarks; }
        }

        public static string[] LinkMarks
        {
            get { return m_LinkMarks; }
        }

        public static string[] OtherMarks
        {
            get { return m_OtherMarks; }
        }

        //public int AddConception(string word, LanguageId languageId)
        //{
        //    lock (this)
        //    {
        //        int newConceptionId = m_LastId + 1;
        //        if (m_AvailableIds.Count > 0)
        //        {
        //            newConceptionId = m_AvailableIds.First().Key;
        //            m_AvailableIds.Remove(newConceptionId);
        //        }

        //        Conception newConception = new Conception(newConceptionId, word, languageId);
        //        //newConception.SaveToDB();

        //        m_Dictionary.Add(newConception.ConceptionId, newConception);

        //        m_LastId = newConceptionId;
        //        return newConceptionId;
        //    }
        //}

        public int AddConception(Conception newConception)
        {
            lock (this)
            {
                m_Dictionary.Add(newConception.ConceptionId, newConception);
                //m_LastId = newConception.ConceptionId;
                return newConception.ConceptionId;
            }
        }

        public void AddDescriptionToConception(int conceptionId, string word, LanguageId languageId,
                    int chnageablePartType,//string chnageablePartType,
                    string chnageablePartText,
                    int langPartId,//string langPartText,           
            bool bSave)
        {
            GetConception(conceptionId).AddDescription(word, languageId, chnageablePartType, chnageablePartText, langPartId, bSave);
        }

        public void ChangeDescriptionOfConception(int conceptionId, string word, LanguageId languageId, int index)
        {
            //todo:
            GetConception(conceptionId).ChangeDescription(word, languageId, index);
        }

        public void ChangeDescriptionOfConception(int conceptionId, string word, LanguageId languageId, string textOld,
                    int chnageablePartType,//string chnageablePartType,
                    string chnageablePartText,
                    int langPartId,//string langPartText,  
            bool bSave)
        {
            GetConception(conceptionId).ChangeDescription(word, languageId, textOld,
                chnageablePartType,
                chnageablePartText,
                langPartId,
                bSave);
        }

        public void RemoveDescriptionFromConception(int conceptionId, string descriptionText, LanguageId languageId, bool bSave)
        {
            Conception handledConception = GetConception(conceptionId);

            handledConception.RemoveDescription(languageId, descriptionText, bSave);

            if (handledConception.DescriptionsCount == 0)
                RemoveConception(conceptionId);
        }

        public void RemoveConception(int conceptionId)
        {
            lock (this)
            {
                if (m_Dictionary.Keys.Contains(conceptionId))
                {
                    using (SqlCeConnection conn = new SqlCeConnection(m_ConnectionString))
                    {
                        conn.Open();
                        DatabaseHelper.DeleteConception(m_Dictionary[conceptionId], conn);
                    }
                    m_Dictionary.Remove(conceptionId);
                }
            }
        }

        public int ConceptionsCount
        {
            get { return m_Dictionary.Count; }
        }

        public Conception GetConception(int conceptionId)
        {
            if (m_Dictionary.ContainsKey(conceptionId))
                return m_Dictionary[conceptionId];

            return null;
            //else
              //  throw new TriLingException(string.Format("Conception with id {0} is absent", conceptionId));
        }

        public IEnumerable<Conception> Conceptions
        {
            get
            {
                return m_Dictionary.Values;
            }
        }

        //public void Load(string dictionaryDataFolder)
        //{
        //    PlaintTextDataLoader loader = new PlaintTextDataLoader(this);
        //    loader.Load(dictionaryDataFolder);
        //}

        //static public void SerializeToXML(string fullPath, TrilingualDictionary dictionary)
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(TrilingualDictionary));
        //    TextWriter textWriter = new StreamWriter(fullPath);
        //    serializer.Serialize(textWriter, dictionary);
        //    textWriter.Close();
        //}

        //public void SerializeToXML(string fullPath)
        //{
        //    using (XmlWriter writer = XmlWriter.Create(fullPath, new XmlWriterSettings()
        //    {
        //        CloseOutput = true,
        //        Indent = true,
        //        Encoding = Encoding.UTF8
        //    }))
        //    {
        //        writer.WriteStartDocument();
        //        writer.WriteStartElement("Conceptions");
        //        foreach (KeyValuePair<int, Conception> conception in m_Dictionary)
        //        {
        //            writer.WriteStartElement("Conception");
        //            conception.Value.SaveConception(writer);
        //            writer.WriteEndElement();
        //        }
        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
        //    }
        //}

        //public void SerializeFromXML(string fullPath)
        //{
        //    using (XmlReader reader = XmlReader.Create(fullPath, new XmlReaderSettings()))
        //    {
        //        while (reader.Read())
        //        {
        //            //// Get element name and switch on it.
        //            switch (reader.Name)
        //            {
        //                case "Conception":
        //                    if (reader.NodeType == XmlNodeType.Element)
        //                    {
        //                        Conception conception = Conception.Create(reader);
        //                        if (conception.ConceptionId == 4314)
        //                        {
        //                            int j = 1;
        //                        }
        //                        AddConception(conception);
        //                        m_AvailableIds.Add(conception.ConceptionId, 0);
                                
        //                        //m_Dictionary.Add(conception.ConceptionId, conception);
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //    foreach (KeyValuePair<int, Conception> conception in m_Dictionary)
        //    {
        //        m_AvailableIds.Remove(conception.Key);
        //        if (conception.Value.ParentId != 0)
        //            conception.Value.ParentConception = m_Dictionary[conception.Value.ParentId];
        //    }
        //}

        void SplitConception(Conception conception)
        {

        }

        ////TO DO: will be needed
        ////void MergeConceptions(Conception conception1, Conception conception2)
        ////{
        ////    foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
        ////    {
        ////        if (langId == LanguageId.Undefined)
        ////            continue;
        ////        List<ConceptionDescription> descs = conception2.GetAllConceptionDescriptions(langId);
        ////        foreach(ConceptionDescription desc in descs)
        ////        {
        ////            conception1.AddDescription(desc, langId, false);
        ////        }                
        ////    }

        ////    RemoveConception(conception2.ConceptionId);
        ////}

        public void SaveToDB()
        {
            try
            {
                using (SqlCeConnection conn = new SqlCeConnection(m_ConnectionString))
                {
                    conn.Open();
                    foreach (Conception conception in m_Dictionary.Values)
                    {
                        conception.SaveConception(conn);
                    }
                }
            }
            catch(Exception ex)
            {
                int j = 1;
            }
        }

        public Conception Conception
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public LanguageIdToSting LanguageIdToSting
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        internal LanguageTranslation LanguageTranslation
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public void LoadFromDB()
        {
            try
            {
                SqlCeConnection conn = new SqlCeConnection(m_ConnectionString);
                conn.Open();
                LoadConceptions(conn);
                LoadDescriptions(conn);
                LoadTranslations(conn);
                conn.Close();
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }
        }

        private void LoadTranslations(SqlCeConnection conn)
        {
            LoadChangeable(conn);
            LoadLanguage(conn);
            LoadPartOfSpeech(conn);
            LoadSemantic(conn);
            LoadTopic(conn);
        }

        private void LoadChangeable(SqlCeConnection conn)
        {
            string query = "SELECT * FROM ChangableTranslation";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ChangableTranslation translation = CreateChangeableTranslation(reader);

                if (!m_AllChangables.ContainsKey(translation.LangForId))
                {
                    m_AllChangables.Add(translation.LangForId, new Dictionary<int, ITranslatable>());
                }

                m_AllChangables[translation.LangForId].Add(translation.ChangeableTypeId, translation);
            }            
        }

        private ChangableTranslation CreateChangeableTranslation(SqlCeDataReader reader)
        {
            int id = (int)reader[0];
            int idChangeablePart = (int)reader[1];
            int idLang = (int)reader[2];
            string translation = (string)reader[3];

            ChangableTranslation transl = new ChangableTranslation
            {
                Id = id,
                ChangeableTypeId = idChangeablePart,
                LangForId = (LanguageId)((int)idLang - 1),
                Translation = translation
            };

            return transl;
        }

        private void LoadLanguage(SqlCeConnection conn)
        {
            string query = "SELECT * FROM LangTranslation";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                LanguageTranslation translation = CreateLangTranslation(reader);

                if (!m_AllLanguages.ContainsKey(translation.LangForId))
                {
                    m_AllLanguages.Add(translation.LangForId, new Dictionary<LanguageId, ITranslatable>());
                }

                m_AllLanguages[translation.LangForId].Add(translation.LangIdNeeded, translation);
            }
        }

        private LanguageTranslation CreateLangTranslation(SqlCeDataReader reader)
        {
            int id = (int)reader[0];
            int idLangNeeded = (int)reader[1];
            int idLang = (int)reader[2];
            string translation = (string)reader[3];

            LanguageTranslation transl = new LanguageTranslation
            {
                Id = id,
                LangIdNeeded = (LanguageId)((int)idLangNeeded - 1),
                LangForId = (LanguageId)((int)idLang - 1),
                Translation = translation
            };

            return transl;
        }

        public void LoadPartOfSpeech(SqlCeConnection conn)
        {
            string query = "SELECT * FROM PartOfSpeechTranslation";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PartOfSpeechTranslation translation = CreatePartOfSpeechTranslation(reader);

                if (!m_AllLangParts.ContainsKey(translation.LangForId))
                {
                    m_AllLangParts.Add(translation.LangForId, new Dictionary<int, ITranslatable>());
                }

                m_AllLangParts[translation.LangForId].Add(translation.PartOfSpeechId, translation);
            }
        }

        private PartOfSpeechTranslation CreatePartOfSpeechTranslation(SqlCeDataReader reader)
        {
            int id = (int)reader[0];
            int idPartOfSpeech = (int)reader[1];
            int idLang = (int)reader[2];
            string translation = (string)reader[3];

            PartOfSpeechTranslation transl = new PartOfSpeechTranslation
            {
                Id = id,
                PartOfSpeechId = idPartOfSpeech,
                LangForId = (LanguageId)((int)idLang - 1),
                Translation = translation
            };

            return transl;
        }

        private void LoadSemantic(SqlCeConnection conn)
        {
            string query = "SELECT * FROM SemanticTranslation";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                SemanticTranslation translation = CreateSemanticTranslation(reader);

                if (!m_AllSemantics.ContainsKey(translation.LangForId))
                {
                    m_AllSemantics.Add(translation.LangForId, new Dictionary<int, ITranslatable>());
                }

                m_AllSemantics[translation.LangForId].Add(translation.SemId, translation);
            }
        }

        private SemanticTranslation CreateSemanticTranslation(SqlCeDataReader reader)
        {
            int id = (int)reader[0];
            int idSem = (int)reader[1];
            int idLang = (int)reader[2];
            string translation = (string)reader[3];

            SemanticTranslation transl = new SemanticTranslation
            {
                Id = id,
                SemId = idSem,
                LangForId = (LanguageId)((int)idLang - 1),
                Translation = translation
            };

            return transl;
        }

        private void LoadTopic(SqlCeConnection conn)
        {
            string query = "SELECT * FROM TopicTranslation";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                TopicTranslation translation = CreateTopicTranslation(reader);

                if (!m_AllTopics.ContainsKey(translation.LangForId))
                {
                    m_AllTopics.Add(translation.LangForId, new Dictionary<int, ITranslatable>());
                }

                m_AllTopics[translation.LangForId].Add(translation.TopicId, translation);
            }
        }

        private TopicTranslation CreateTopicTranslation(SqlCeDataReader reader)
        {
            int id = (int)reader[0];
            int idTopic = (int)reader[1];
            int idLang = (int)reader[2];
            string translation = (string)reader[3];

            TopicTranslation transl = new TopicTranslation
            {
                Id = id,
                TopicId = idTopic,
                LangForId = (LanguageId)((int)idLang - 1),
                Translation = translation
            };

            return transl;
        }

        private void LoadConceptions(SqlCeConnection conn)
        {
            string query = "SELECT * FROM Conception";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                Conception conception = CreateConception(reader);
                m_Dictionary.Add(conception.ConceptionId, conception);
            }
        }

        private void LoadDescriptions(SqlCeConnection conn)
        {
            string query = "SELECT * FROM Description";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ConceptionDescription description = CreateDescription(reader);
                m_Dictionary[description.OwnConception.ConceptionId].AddDescription(description, description.LanguageId, false);
            }
        }

        private ConceptionDescription CreateDescription(SqlCeDataReader reader)
        {
            int id = (int)reader[0];
            int idConception = (int)reader[1];
            string desc = (string)reader[2];
            if (desc.Contains("ааа"))
            {
                int j = 1;
            }
            LanguageId lang = (LanguageId)((int)reader[3] - 1);
            string changeableValue = reader.IsDBNull(4) ? "" : (string)reader[4];
            int changeableType = reader.IsDBNull(5) ? 0 : (int)reader[5];
            int partOfSpeech = reader.IsDBNull(6) ? 0 : (int)reader[6];

            ConceptionDescription description = new ConceptionDescription(m_Dictionary[idConception], desc);
            description.DescriptionId = id;
            description.ChangeableDB.Type = changeableType;
            description.ChangeableDB.Value = changeableValue;
            description.LangPartId = partOfSpeech;
            description.LanguageId = lang;

            return description;
        }

        private Conception CreateConception(SqlCeDataReader reader)
        {
            int id = (int)reader[0];
            int idParent = reader.IsDBNull(1) ? 0 : (int)reader[1];
            int idTopic = reader.IsDBNull(2) ? 0 : (int)reader[2];
            int idSemantic = reader.IsDBNull(3) ? 0 : (int)reader[3];

            Conception conception = new Conception(id, idParent, idTopic, idSemantic);

            return conception;
        }

        public Dictionary<LanguageId, Dictionary<int, ITranslatable>> AllTopics
        {
            get { return m_AllTopics; }
        }
        public Dictionary<LanguageId, Dictionary<int, ITranslatable>> AllSemantics
        {
            get { return m_AllSemantics; }
        }
        public Dictionary<LanguageId, Dictionary<int, ITranslatable>> AllLangParts
        {
            get { return m_AllLangParts; }
        }
        public Dictionary<LanguageId, Dictionary<LanguageId, ITranslatable>> AllLanguages
        {
            get { return m_AllLanguages; }
        }
        public Dictionary<LanguageId, Dictionary<int, ITranslatable>> AllChangeables
        {
            get { return m_AllChangables; }
        }

        public void LoadPartOfSpeech()
        {
            using (SqlCeConnection conn = new SqlCeConnection(m_ConnectionString))
            {
                conn.Open();
                LoadPartOfSpeech(conn);
            }
        }

        public void LoadSemantic()
        {
            using (SqlCeConnection conn = new SqlCeConnection(m_ConnectionString))
            {
                conn.Open();
                LoadSemantic(conn);
            }
        }

        public void LoadTopic()
        {
            using (SqlCeConnection conn = new SqlCeConnection(m_ConnectionString))
            {
                conn.Open();
                LoadTopic(conn);
            }
        }

        public void LoadChangeable()
        {
            using (SqlCeConnection conn = new SqlCeConnection(m_ConnectionString))
            {
                conn.Open();
                LoadChangeable(conn);
            }
        }

        public List<Conception> GetParents()
        {
            if (m_Parents == null)
            {
                m_Parents = new List<Conception>();
                foreach (Conception con in m_Dictionary.Values)
                {
                    if (con.ParentId != 0)
                        continue;
                    m_Parents.Add(con);
                }
            }
            return m_Parents;
        }
    }


 
}
