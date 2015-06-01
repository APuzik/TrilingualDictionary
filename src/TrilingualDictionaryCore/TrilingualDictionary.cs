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
        private Dictionary<LanguageId, List<ConceptionDescription>> m_AllDescriptions = new Dictionary<LanguageId,List<ConceptionDescription>>();

        private Dictionary<LanguageId, List<TopicTranslation>> m_AllTopics = new Dictionary<LanguageId, List<TopicTranslation>>();
        private Dictionary<LanguageId, List<SemanticTranslation>> m_AllSemantics = new Dictionary<LanguageId, List<SemanticTranslation>>();
        private Dictionary<LanguageId, List<PartOfSpeechTranslation>> m_AllLangParts = new Dictionary<LanguageId, List<PartOfSpeechTranslation>>();
        private Dictionary<LanguageId, List<LanguageTranslation>> m_AllLanguages = new Dictionary<LanguageId, List<LanguageTranslation>>();
        private Dictionary<LanguageId, List<ChangableTranslation>> m_AllChangables = new Dictionary<LanguageId, List<ChangableTranslation>>();

        private Dictionary<int, int> m_AvailableIds = new Dictionary<int, int>();
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

        public int AddConception(string word, LanguageId languageId)
        {
            lock (this)
            {
                int newConceptionId = m_LastId + 1;
                if (m_AvailableIds.Count > 0)
                {
                    newConceptionId = m_AvailableIds.First().Key;
                    m_AvailableIds.Remove(newConceptionId);
                }

                Conception newConception = new Conception(newConceptionId, word, languageId);
                m_Dictionary.Add(newConception.ConceptionId, newConception);
                m_LastId = newConceptionId;
                return newConceptionId;
            }
        }

        private int AddConception(Conception newConception)
        {
            lock (this)
            {
                m_Dictionary.Add(newConception.ConceptionId, newConception);
                m_LastId = newConception.ConceptionId;
                return newConception.ConceptionId;
            }
        }

        public void AddDescriptionToConception(int conceptionId, string word, LanguageId languageId)
        {
            GetConception(conceptionId).AddDescription(word, languageId);
        }

        public void ChangeDescriptionOfConception(int conceptionId, string word, LanguageId languageId, int index)
        {
            //todo:
            GetConception(conceptionId).ChangeDescription(word, languageId, index);
        }

        public void ChangeDescriptionOfConception(int conceptionId, string word, LanguageId languageId, string textOld)
        {
            GetConception(conceptionId).ChangeDescription(word, languageId, textOld);
        }

        public void RemoveDescriptionFromConception(int conceptionId, string descriptionText, LanguageId languageId)
        {
            Conception handledConception = GetConception(conceptionId);

            handledConception.RemoveDescription(languageId, descriptionText);

            if (handledConception.DescriptionsCount == 0)
                RemoveConception(conceptionId);
        }

        public void RemoveConception(int conceptionId)
        {
            lock (this)
            {
                if (m_Dictionary.Keys.Contains(conceptionId))
                {
                    m_Dictionary.Remove(conceptionId);
                    m_AvailableIds.Add(conceptionId, 0);
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
            else
                throw new TriLingException(string.Format("Conception with id {0} is absent", conceptionId));
        }

        public IEnumerable<Conception> Conceptions
        {
            get
            {
                return m_Dictionary.Values;
            }
        }

        public void Load(string dictionaryDataFolder)
        {
            PlaintTextDataLoader loader = new PlaintTextDataLoader(this);
            loader.Load(dictionaryDataFolder);
        }

        static public void SerializeToXML(string fullPath, TrilingualDictionary dictionary)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TrilingualDictionary));
            TextWriter textWriter = new StreamWriter(fullPath);
            serializer.Serialize(textWriter, dictionary);
            textWriter.Close();
        }

        public void SerializeToXML(string fullPath)
        {
            using (XmlWriter writer = XmlWriter.Create(fullPath, new XmlWriterSettings()
            {
                CloseOutput = true,
                Indent = true,
                Encoding = Encoding.UTF8
            }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Conceptions");
                foreach (KeyValuePair<int, Conception> conception in m_Dictionary)
                {
                    writer.WriteStartElement("Conception");
                    conception.Value.SaveConception(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void SerializeFromXML(string fullPath)
        {
            using (XmlReader reader = XmlReader.Create(fullPath, new XmlReaderSettings()))
            {
                while (reader.Read())
                {
                    //// Get element name and switch on it.
                    switch (reader.Name)
                    {
                        case "Conception":
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                Conception conception = Conception.Create(reader);
                                if (conception.ConceptionId == 4314)
                                {
                                    int j = 1;
                                }
                                AddConception(conception);
                                m_AvailableIds.Add(conception.ConceptionId, 0);
                                
                                //m_Dictionary.Add(conception.ConceptionId, conception);
                            }
                            break;
                    }
                }
            }
            foreach (KeyValuePair<int, Conception> conception in m_Dictionary)
            {
                m_AvailableIds.Remove(conception.Key);
                if (conception.Value.ParentId != 0)
                    conception.Value.ParentConception = m_Dictionary[conception.Value.ParentId];
            }
        }

        void SplitConception(Conception conception)
        {

        }

        void MergeConceptions(Conception conception1, Conception conception2)
        {
            foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
            {
                if (langId == LanguageId.Undefined)
                    continue;
                List<ConceptionDescription> descs = conception2.GetAllConceptionDescriptions(langId);
                foreach(ConceptionDescription desc in descs)
                {
                    conception1.AddDescription(desc, langId);
                }                
            }

            RemoveConception(conception2.ConceptionId);
        }

        public void SaveToDB()
        {
            try
            {
                SqlCeConnection conn = new SqlCeConnection(m_ConnectionString);//@"Data Source=D:\AS\_Aspirantura\Projects\TrilingualDictionary\src\Data\TrilingualDictionary.sdf;Max Database Size=4091;Password=Password1");
                string s = Path.GetFullPath(@".\TrilingualDictionary.sdf");
                //SqlCeConnection conn = new SqlCeConnection(@"Data Source=D:\AS\_Aspirantura\Projects\TrilingualDictionary\src\Data\TrilingualDictionary.sdf;Max Database Size=4091;Password=***********");
                conn.Open();
                foreach (Conception conception in m_Dictionary.Values)
                {
                    conception.SaveConception(conn);
                }
                conn.Close();
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

        
    }


 
}
