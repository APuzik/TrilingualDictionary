using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace TrilingualDictionaryCore
{
    public class ConceptionDescription : IComparable<ConceptionDescription>
    {
        private static List<ConceptionDescription> m_EmptyList = null;
        public class ChangeablelPart
        {
            public string Type;
            public string Value;
        }

        public class ChangeablePartDB
        {
            public int Type;
            public string Value;
        }

        private string m_Word = "";
        private string m_WordWoAcccent = "";
        private string m_Explanation = "";
        private Conception m_Conception = null;
        private int m_DescriptionId = 0;
        //private int m_OverallDescriptionId = 0;

        public ChangeablelPart Changeable
        {
            get;
            set;
        }

        public ChangeablePartDB ChangeableDB
        {
            get;
            set;
        }
        //public string Topic { get; set; }

        //public string Semantic { get; set; }

        //public string Link { get; set; }

        public ConceptionDescription(Conception conception, string word)
        {
            m_Conception = conception;
            ChangeDescription(word);

            //Changeable = new ChangeablelPart();
            ChangeableDB = new ChangeablePartDB();
        }

        public int CompareTo(ConceptionDescription other)
        {
            return string.Compare(this.m_Word, other.m_Word, true);
        }

        public Conception OwnConception
        {
            get { return m_Conception; }
        }

        internal void ChangeDescription(string word)
        {
            m_Word = word;
            m_WordWoAcccent = m_Word.Replace("#", "");
        }

        public string ConceptionRegistryDescription
        {
            get { return m_Word; }
        }

        public string ConceptionRegistryDescriptionWoAccents
        {
            get { return m_WordWoAcccent; }
        }

        public string ConceptionExplanation
        {
            get { return m_Explanation; }
        }

        public string LangPart { get; set; }

        //Performance Warning!!!
        //public string ConceptionSortDescription 
        //{
        //    get
        //    {
        //        //string forSorting = Regex.Replace(m_Word, @"(\(|\)|\[|\]|#)", "");
        //        string forSorting = Regex.Replace(m_Word, @"([^\w-\.~ ])", "");
        //        return forSorting;
        //    }
        //}

        internal void SaveDescription(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("RegistryDescription", ConceptionRegistryDescription);
            
            if (!string.IsNullOrWhiteSpace(Changeable.Type))
                writer.WriteAttributeString("ChangeableType", Changeable.Type);
            if (!string.IsNullOrWhiteSpace(Changeable.Value))
                writer.WriteAttributeString("ChangeableValue", Changeable.Value);
            if (!string.IsNullOrWhiteSpace(LangPart))
                writer.WriteAttributeString("LangPart", LangPart);

        }

        internal static ConceptionDescription Create(XmlReader reader, Conception conception)
        {
            string RegistryDescription = GetXmlAttribute(reader, "RegistryDescription");
            ConceptionDescription desc = new ConceptionDescription(conception, RegistryDescription);

            if (RegistryDescription == "аво#ника")
            {
                int k = 1;
            }

            if (string.IsNullOrEmpty(conception.Topic))
                conception.Topic = ConceptionDescription.GetXmlAttribute(reader, "Topic");
            if (string.IsNullOrEmpty(conception.Semantic))
                conception.Semantic = ConceptionDescription.GetXmlAttribute(reader, "Semantic");
            if (string.IsNullOrEmpty(conception.Link))
                conception.Link = ConceptionDescription.GetXmlAttribute(reader, "Link");

            desc.Changeable.Type = GetXmlAttribute(reader, "ChangeableType");
            desc.Changeable.Value = GetXmlAttribute(reader, "ChangeableValue");
            desc.LangPart = GetXmlAttribute(reader, "LangPart");

            return desc;
        }

        /// <summary>
        /// Converts english scanned equivalent to Cyrillic
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetXmlAttribute(XmlReader reader, string name)
        {
            string result = reader[name] ?? "";
            string pattern = "(a|A|B|e|E|c|C|T|y|i|I|o|O|p|P|H|k|K|x|X|m|M)";
            string result1 = result;
            //result = Regex.Replace(result, pattern, (match) =>
            //{
            //    string s = result1;
            //    switch (match.Value)
            //    {
            //        case "a":
            //            return "а";
            //        case "A":
            //            return "А";
            //        case "B":
            //            return "В";
            //        case "e":
            //            return "е";
            //        case "E":
            //            return "Е";
            //        case "c":
            //            return "с";
            //        case "C":
            //            return "С";
            //        case "T":
            //            return "Т";
            //        case "y":
            //            return "у";
            //        case "i":
            //            return "і";
            //        case "I":
            //            return "І";
            //        case "o":
            //            return "о";
            //        case "O":
            //            return "О";
            //        case "p":
            //            return "р";
            //        case "P":
            //            return "Р";
            //        case "H":
            //            return "Н";
            //        case "k":
            //            return "к";
            //        case "K":
            //            return "К";
            //        case "x":
            //            return "х";
            //        case "X":
            //            return "Х";
            //        //case "m":
            //        //    return "м";
            //        case "M":
            //            return "М";
            //    }
            //    return "";
            //});
            return result;
        }

        public int DescriptionId
        {
            get { return m_DescriptionId; }
            set { m_DescriptionId = value; }
        }

        public bool IsEmpty
        {
            get { return m_Word == "Отсутствует"; }
        }

        public static List<ConceptionDescription> EmptyList 
        {
            get
            {
                if( m_EmptyList == null)
                {
                    m_EmptyList= new List<ConceptionDescription>();
                    m_EmptyList.Add(new ConceptionDescription(null, "Отсутствует"));
                }
                return m_EmptyList;
            }           
        }

        internal void SaveDescription(SqlCeConnection conn, LanguageId langId)
        {
            if (IsDescriptionExsists(conn, langId))
                UpdateDescription(conn, langId);
            else
                InsertDescription(conn, langId);
        }

        private void UpdateDescription(SqlCeConnection conn, LanguageId langId)
        {
            int idPartSpeech = SavePartOfSpeech(conn, this.LangPart, LanguageId.Russian);//langId);
            int idChangeable = SaveChangeable(conn, this.Changeable, LanguageId.Russian);//langId);
            //this.LangPartId = idPartSpeech;
            //this.ChangeableDB.Type = idChangeable;
            //this.ChangeableDB.Value = this.Changeable.Value;

            DatabaseHelper.UpdateConceptionDescription(conn, langId, this, idPartSpeech, idChangeable);
        }

        private bool IsDescriptionExsists(SqlCeConnection conn, LanguageId langId)
        {
            return DatabaseHelper.IsConceptionDescriptionExsists(conn, m_DescriptionId);
        }

        internal void InsertDescription(SqlCeConnection conn, LanguageId langId)
        {
            int idPartSpeech = SavePartOfSpeech(conn, this.LangPart, LanguageId.Russian);//langId);
            int idChangeable = SaveChangeable(conn, this.Changeable, LanguageId.Russian);//langId);
            //this.LangPartId = idPartSpeech;
            //this.ChangeableDB.Type = idChangeable;
            //this.ChangeableDB.Value = this.Changeable.Value;

            DatabaseHelper.InsertConceptionDescription(conn, langId, this, idPartSpeech, idChangeable);
        }        

        private int SavePartOfSpeech(SqlCeConnection conn, string partOfSpeechTranslation, LanguageId langId)
        {
            if (string.IsNullOrEmpty(partOfSpeechTranslation))
                return 0;

            PartOfSpeechTranslation trans = new PartOfSpeechTranslation
            {
                Id = 0,
                PartOfSpeechId = 0,
                LangForId = langId,
                Translation = partOfSpeechTranslation
            };

            trans.Id = DatabaseHelper.GetPartOfSpeechIdByLang(partOfSpeechTranslation, langId, conn);
            int partOfSpeechId = trans.SaveToDB(conn, langId, false);

            //int partOfSpeechId = DatabaseHelper.GetPartOfSpeechIdByLang(partOfSpeechTranslation, langId, conn);
            //if (partOfSpeechId == 0)
            //    partOfSpeechId = DatabaseHelper.InsertPartOfSpeech(partOfSpeechTranslation, langId, conn);

            return partOfSpeechId;
        }

        private int SaveChangeable(SqlCeConnection conn, ChangeablelPart changeablelPart, LanguageId langId)
        {
            if (changeablelPart == null || string.IsNullOrEmpty(changeablelPart.Type) || string.IsNullOrEmpty(changeablelPart.Value))
                return 0;

            ChangableTranslation trans = new ChangableTranslation
            {
                Id = 0,
                ChangeableTypeId = 0,
                LangForId = langId,
                Translation = changeablelPart.Type
            };

            trans.Id = DatabaseHelper.GetChangableIdByLang(changeablelPart, langId, conn);
            int changeableId = trans.SaveToDB(conn, langId, false);
            //if (partOfSpeechId == 0)
            //    partOfSpeechId = DatabaseHelper.InsertChangable(changeablelPart, langId, conn);

            return changeableId;
        }

        internal string PartOfSpeechTranslation
        {
            get;
            set;
        }

        //internal ChangableTranslation ChangableTranslation
        //{
        //    get
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //    set
        //    {
        //    }
        //}

        public int LangPartId { get; set; }

        public LanguageId LanguageId { get; set; }

        //public int OverallDescriptionId 
        //{ 
        //    get { return m_OverallDescriptionId; }
        //    set { m_OverallDescriptionId = value; }
        //}
    }
}
