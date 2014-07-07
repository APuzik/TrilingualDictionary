using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

namespace TrilingualDictionaryCore
{
    public class TrilingualDictionary
    {
        private int m_LastId = 0;
        private Dictionary<int, int> m_AvailableIds = new Dictionary<int, int>();
        private Dictionary<int, Conception> m_Dictionary = new Dictionary<int, Conception>();

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

        public int AddConception(string word, Conception.LanguageId languageId)
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

            if (handledConception.DescriptionsCount == 0)
                RemoveConception(conceptionId);
        }

        public void RemoveConception(int conceptionId)
        {
            lock (this)
            {
                if (m_Dictionary.Keys.Contains(conceptionId))
                {
                    m_AvailableIds.Add(conceptionId, 0);
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
    }


 
}
