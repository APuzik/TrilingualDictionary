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
    public class ConceptionDescription
    {
        private static List<ConceptionDescription> m_EmptyList = null;
        public class ChangeablelPart
        {
            public string Type;
            public string Value;
        }

        private string m_Word;
        private string m_Explanation = "";
        private Conception m_Conception = null;
        private int m_DescriptionId;
        private int m_OverallDescriptionId = 0;

        public ChangeablelPart Changeable
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
            m_Word = word;
            Changeable = new ChangeablelPart();
        }

        public Conception OwnConception
        {
            get { return m_Conception; }
        }

        internal void ChangeDescription(string word)
        {
            m_Word = word;
        }

        public string ConceptionRegistryDescription
        {
            get { return m_Word; }
        }

        public string ConceptionRegistryDescriptionWoAccents
        {
            get { return m_Word.Replace("#", ""); }
        }

        public string ConceptionExplanation
        {
            get { return m_Explanation; }
        }

        public string LangPart { get; set; }

        public string ConceptionSortDescription 
        {
            get
            {
                //string forSorting = Regex.Replace(m_Word, @"(\(|\)|\[|\]|#)", "");
                string forSorting = Regex.Replace(m_Word, @"([^\w-\.~ ])", "");
                return forSorting;
            }
        }

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

            string query = "UPDATE Description SET ConceptionId=@ConceptionId, Description=@Description, Language=@Language, ChangablePart=@ChangablePart, ChangableType=@ChangableType, PartOfSpeech=@PartOfSpeech"; 

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            CreateCommandParam(langId, idPartSpeech, idChangeable, cmd);

            cmd.ExecuteNonQuery();
        }

        private bool IsDescriptionExsists(SqlCeConnection conn, LanguageId langId)
        {
            if (m_OverallDescriptionId == 0)
                return false;

            string query = string.Format("SELECT Id FROM Description WHERE Id={0}", m_OverallDescriptionId);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();//CommandBehavior.CloseConnection);
            if (reader.Read())
            {
                return true;
            }

            return false;
        }

        internal void InsertDescription(SqlCeConnection conn, LanguageId langId)
        {
            int idPartSpeech = SavePartOfSpeech(conn, this.LangPart, LanguageId.Russian);//langId);
            int idChangeable = SaveChangeable(conn, this.Changeable, LanguageId.Russian);//langId);

            string query = "INSERT INTO Description(ConceptionId, Description, Language, ChangablePart, ChangableType, PartOfSpeech) VALUES(@ConceptionId, @Description, @Language, @ChangablePart, @ChangableType, @PartOfSpeech)";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            CreateCommandParam(langId, idPartSpeech, idChangeable, cmd);

            cmd.ExecuteNonQuery();
        }

        private void CreateCommandParam(LanguageId langId, int idPartSpeech, int idChangeable, SqlCeCommand cmd)
        {
            cmd.Parameters.Add("@ConceptionId", SqlDbType.Int);
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@Language", SqlDbType.Int);
            cmd.Parameters.Add("@ChangablePart", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@ChangableType", SqlDbType.Int);
            cmd.Parameters.Add("@PartOfSpeech", SqlDbType.Int);

            cmd.Parameters["@ConceptionId"].Value = this.OwnConception.ConceptionId;
            cmd.Parameters["@Description"].Value = this.ConceptionRegistryDescription;
            cmd.Parameters["@Language"].Value = (int)langId + 1;
            cmd.Parameters["@ChangablePart"].Value = this.Changeable.Value;
            cmd.Parameters["@ChangableType"].Value = (idChangeable == 0 ? DBNull.Value : (object)idChangeable);
            cmd.Parameters["@PartOfSpeech"].Value = (idPartSpeech == 0 ? DBNull.Value : (object)idPartSpeech);
        }

        private int SavePartOfSpeech(SqlCeConnection conn, string partOfSpeechTranslation, LanguageId langId)
        {
            if (string.IsNullOrEmpty(partOfSpeechTranslation))
                return 0;

            int partOfSpeechId = GetPartOfSpeechByLang(partOfSpeechTranslation, langId, conn);
            if (partOfSpeechId == 0)
                partOfSpeechId = InsertPartOfSpeech(partOfSpeechTranslation, langId, conn);

            return partOfSpeechId;
        }

        private int InsertPartOfSpeech(string partOfSpeechTranslation, LanguageId languageId, SqlCeConnection conn)
        {
            string query = "INSERT INTO PartOfSpeech (DefaultName) VALUES(NULL)";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            var id = cmd.ExecuteScalar();

            int partOfSpeechTranslationId = Convert.ToInt32(id);

            if (partOfSpeechTranslationId != 0)
            {
                query = "INSERT INTO PartOfSpeechTranslation (PartOfSpeechId, LangForId, Translation) VALUES(@PartOfSpeechId, @Langid, @Translation)";

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@PartOfSpeechId", SqlDbType.Int);
                cmd.Parameters.Add("@Langid", SqlDbType.Int);
                cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

                cmd.Parameters["@PartOfSpeechId"].Value = partOfSpeechTranslationId;
                cmd.Parameters["@Langid"].Value = (int)languageId + 1;
                cmd.Parameters["@Translation"].Value = partOfSpeechTranslation;

                cmd.ExecuteNonQuery();
            }
            return partOfSpeechTranslationId;

        }

        private int GetPartOfSpeechByLang(string partOfSpeechTranslation, LanguageId languageId, SqlCeConnection conn)
        {
            string query = string.Format("SELECT PartOfSpeechId FROM PartOfSpeechTranslation WHERE LangForId={0} AND Translation='{1}'", (int)languageId + 1, partOfSpeechTranslation);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();//CommandBehavior.CloseConnection);
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }

        private int SaveChangeable(SqlCeConnection conn, ChangeablelPart changeablelPart, LanguageId langId)
        {
            if (changeablelPart == null || string.IsNullOrEmpty(changeablelPart.Type) || string.IsNullOrEmpty(changeablelPart.Value))
                return 0;

            int partOfSpeechId = GetChangableByLang(changeablelPart, langId, conn);
            if (partOfSpeechId == 0)
                partOfSpeechId = InsertChangable(changeablelPart, langId, conn);

            return partOfSpeechId;
        }

        private int InsertChangable(ChangeablelPart changeablelPart, LanguageId languageId, SqlCeConnection conn)
        {
            string query = "INSERT INTO ChangablePartType (DefaultName) VALUES(NULL)";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            var id = cmd.ExecuteScalar();

            int changableTypeId = Convert.ToInt32(id);

            if (changableTypeId != 0)
            {
                query = "INSERT INTO ChangableTranslation (ChangableTypeId, LangForId, Translation) VALUES(@ChangableTypeId, @Langid, @Translation)";

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@ChangableTypeId", SqlDbType.Int);
                cmd.Parameters.Add("@Langid", SqlDbType.Int);
                cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

                cmd.Parameters["@ChangableTypeId"].Value = changableTypeId;
                cmd.Parameters["@Langid"].Value = (int)languageId + 1;
                cmd.Parameters["@Translation"].Value = changeablelPart.Type;

                cmd.ExecuteNonQuery();
            }
            return changableTypeId;

        }

        private int GetChangableByLang(ChangeablelPart changeablelPart, LanguageId languageId, SqlCeConnection conn)
        {
            string query = string.Format("SELECT ChangableTypeId FROM ChangableTranslation WHERE LangForId = {0} AND Translation='{1}'", (int)languageId + 1, changeablelPart.Type);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();//CommandBehavior.CloseConnection);
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }

        int InsertChangablePart(SqlCeConnection conn, ChangeablelPart changeablelPart, LanguageId langId)
        {
            string query = "INSERT INTO Changable(Id) VALUES(Id); SELECT CAST(scope_identity() AS int)";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            int newID = (int)cmd.ExecuteScalar();

            query = "INSERT INTO ChangableTranslation(Id, ChangableTypeId, LangForId, Translation) VALUES(Id, ChangableTypeId, LangForId, Translation)";


            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;
            cmd.Parameters.Add("@ID", SqlDbType.Int);
            cmd.Parameters.Add("@ChangableTypeId", SqlDbType.Int);
            cmd.Parameters.Add("@LangForId", SqlDbType.Int);
            cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 1024);

            cmd.Parameters["@ID"].Value = this.DescriptionId;
            cmd.Parameters["@ChangableTypeId"].Value = newID;
            cmd.Parameters["@LangForId"].Value = langId;
            cmd.Parameters["@Translation"].Value = changeablelPart.Value;


            newID = (int)cmd.ExecuteScalar();

            return newID;            
        }

        private int SavePartSpeech(SqlCeConnection conn,  PartOfSpeechTranslation partOfSpeechTranslation)
        {
            throw new NotImplementedException();
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
    }
}
