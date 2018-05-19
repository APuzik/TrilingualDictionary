using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class DatabaseHelper
    {
        static string s_ConnectionString = @"Data Source=.\TrilingualDictionary.sdf;Password=Password1";

        static internal int InsertTopic(string topicDescription, LanguageId languageId, SqlCeConnection conn)
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

        static internal int GetTopicIdByLang(string topicDescription, LanguageId languageId, SqlCeConnection conn)
        {
            string query = string.Format("SELECT TopicId FROM TopicTranslation WHERE LangIdFor={0} AND Translation='{1}'", (int)languageId + 1, topicDescription);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }

        static internal int InsertSemantic(string semDescription, LanguageId languageId, SqlCeConnection conn)
        {
            string query = "INSERT INTO Semantic (DefaultName) VALUES(NULL)";

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";

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

        static internal int GetSemanticIdByLang(string semDescription, LanguageId languageId, SqlCeConnection conn)
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

        static internal bool IsConceptionExsists(SqlCeConnection conn, int conceptionId)
        {
            if (conceptionId == 0)
                return false;

            string query = string.Format("SELECT Id FROM Conception WHERE Id={0}", conceptionId);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }

            return false;
        }

        static internal int InsertConceptionData(SqlCeConnection conn, int idParent, int idTopic, int idSemantic)
        {
            //string query = "INSERT INTO Conception(Id, ParentId, Topic, Semantic) VALUES(@ID, @ParentId, @Topic, @Semantic)";
            string query = "INSERT INTO Conception(ParentId, Topic, Semantic) VALUES(@ParentId, @Topic, @Semantic)";

            return PerformConceptionDBOperation(conn, idParent, idTopic, idSemantic, query, 0);
        }

        static internal void UpdateConceptionData(SqlCeConnection conn, int idParent, int idTopic, int idSemantic, int Id)
        {
            //string query = "UPDATE Conception SET Id=@ID, ParentId=@ParentId, Topic=@Topic, Semantic=@Semantic";
            string query = "UPDATE Conception SET ParentId=@ParentId, Topic=@Topic, Semantic=@Semantic WHERE Id=@Id";

            PerformConceptionDBOperation(conn, idParent, idTopic, idSemantic, query, Id);
        }

        private static int PerformConceptionDBOperation(SqlCeConnection conn, int idParent, int idTopic, int idSemantic, string query, int Id)
        {
            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            CreateConceptionCommandParams(idParent, idTopic, idSemantic, cmd, Id);

            cmd.ExecuteNonQuery();
            if (query.StartsWith("INSERT"))
            {
                query = "SELECT @@IDENTITY;";

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                var id = cmd.ExecuteScalar();

                int newConceptionId = Convert.ToInt32(id);

                return newConceptionId;
            }

            return Id;
        }

        static private void CreateConceptionCommandParams(int idParent, int idTopic, int idSemantic, SqlCeCommand cmd, int Id)
        {
            cmd.Parameters.Add("@ParentId", SqlDbType.Int);
            cmd.Parameters.Add("@Topic", SqlDbType.Int);
            cmd.Parameters.Add("@Semantic", SqlDbType.Int);
            if (Id != 0)
                cmd.Parameters.Add("@Id", SqlDbType.Int);

            cmd.Parameters["@ParentId"].Value = idParent;
            cmd.Parameters["@Topic"].Value = (idTopic == 0 ? DBNull.Value : (object)idTopic);
            cmd.Parameters["@Semantic"].Value = (idSemantic == 0 ? DBNull.Value : (object)idSemantic);
            if (Id != 0)
                cmd.Parameters["@Id"].Value = Id;
        }

        static internal void InsertConceptionDescription(SqlCeConnection conn, LanguageId langId, ConceptionDescription desc, int idPartSpeech, int idChangeable)
        {
            string query = "INSERT INTO Description(ConceptionId, Description, Language, ChangablePart, ChangableType, PartOfSpeech) VALUES(@ConceptionId, @Description, @Language, @ChangablePart, @ChangableType, @PartOfSpeech)";

            PerformDescriptionDBOperation(conn, langId, desc, idPartSpeech, idChangeable, query);
        }

        static internal void UpdateConceptionDescription(SqlCeConnection conn, LanguageId langId, ConceptionDescription desc, int idPartSpeech, int idChangeable)
        {
            string query = "UPDATE Description SET ConceptionId=@ConceptionId, Description=@Description, Language=@Language, ChangablePart=@ChangablePart, ChangableType=@ChangableType, PartOfSpeech=@PartOfSpeech WHERE Id=@Id";

            PerformDescriptionDBOperation(conn, langId, desc, idPartSpeech, idChangeable, query);
        }

        private static void PerformDescriptionDBOperation(SqlCeConnection conn, LanguageId langId, ConceptionDescription desc, int idPartSpeech, int idChangeable, string query)
        {
            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            CreateDescriptionCommandParam(langId, desc, idPartSpeech, idChangeable, cmd);

            cmd.ExecuteNonQuery();
            if (query.StartsWith("INSERT"))
            {
                query = "SELECT @@IDENTITY;";

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                var id = cmd.ExecuteScalar();

                int descId = Convert.ToInt32(id);

                if (descId != 0)
                {
                    desc.DescriptionId = descId;
                }
            }
        }

        static private void CreateDescriptionCommandParam(LanguageId langId, ConceptionDescription desc,
                                                            int idPartSpeech, int idChangeable, SqlCeCommand cmd)
        {
            cmd.Parameters.Add("@ConceptionId", SqlDbType.Int);
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@Language", SqlDbType.Int);
            cmd.Parameters.Add("@ChangablePart", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@ChangableType", SqlDbType.Int);
            cmd.Parameters.Add("@PartOfSpeech", SqlDbType.Int);
            if (desc.DescriptionId != 0)
                cmd.Parameters.Add("@Id", SqlDbType.Int);

            cmd.Parameters["@ConceptionId"].Value = desc.OwnConception.ConceptionId;
            cmd.Parameters["@Description"].Value = desc.ConceptionRegistryDescription;
            cmd.Parameters["@Language"].Value = (int)langId + 1;
            cmd.Parameters["@ChangablePart"].Value = desc.Changeable == null ? "" : (desc.Changeable.Value == null ? "" : desc.Changeable.Value);
            cmd.Parameters["@ChangableType"].Value = (idChangeable == 0 ? DBNull.Value : (object)idChangeable);
            cmd.Parameters["@PartOfSpeech"].Value = (idPartSpeech == 0 ? DBNull.Value : (object)idPartSpeech);
            if (desc.DescriptionId != 0)
                cmd.Parameters["@Id"].Value = desc.DescriptionId;
        }

        internal static bool IsConceptionDescriptionExsists(SqlCeConnection conn, int overallDescriptionId)
        {
            if (overallDescriptionId == 0)
                return false;

            string query = string.Format("SELECT Id FROM Description WHERE Id={0}", overallDescriptionId);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }

            return false;
        }

        internal static int GetTranslatableIdByTranslation(ITranslatable translatable, LanguageId languageId, SqlCeConnection conn)
        {
            string query = string.Format(translatable.SelectServConceptionIdQuery, (int)languageId + 1, translatable.Translation);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }

        internal static int InsertTranslatable(ITranslatable translatable, LanguageId languageId, SqlCeConnection conn)
        {
            string query = translatable.InsertServConceptionQuery;

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            var id = cmd.ExecuteScalar();

            int translatableId = Convert.ToInt32(id);

            if (translatableId != 0)
            {
                InsertTranslation(translatable, languageId, conn, translatableId);
            }

            return translatableId;
        }

        public static int InsertTranslation(ITranslatable translatable, LanguageId languageId, SqlCeConnection conn, int translatableId)
        {
            string query = translatable.InsertTranslationQuery;

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.Parameters.Add("@ServConceptionId", SqlDbType.Int);
            cmd.Parameters.Add("@LangId", SqlDbType.Int);
            cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

            cmd.Parameters["@ServConceptionId"].Value = translatableId;
            cmd.Parameters["@LangId"].Value = (int)languageId + 1;
            cmd.Parameters["@Translation"].Value = translatable.Translation;

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            translatable.Id = Convert.ToInt32(cmd.ExecuteScalar());
            translatable.ServConceptionId = translatableId;
            return translatable.Id;
        }

        internal static int UpdateTranslation(ITranslatable translatable, LanguageId languageId, SqlCeConnection conn)
        {
            if (translatable.Id != 0)
            {
                string query = translatable.UpdateTranslationQuery;
                SqlCeCommand cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

                cmd.Parameters["@Id"].Value = translatable.Id;
                cmd.Parameters["@Translation"].Value = translatable.Translation;

                cmd.ExecuteNonQuery();
            }
            return translatable.Id;
        }

        internal static bool DeleteTranslation(ITranslatable translatable, SqlCeConnection conn)
        {
            if (translatable.Id != 0)
            {
                string query = translatable.DeleteTranslationQuery;
                SqlCeCommand cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@Id", SqlDbType.Int);

                cmd.Parameters["@Id"].Value = translatable.Id;

                cmd.ExecuteNonQuery();

                if (!IsTranslationExists(translatable, conn))
                {
                    DeleteTranslatable(translatable, conn);
                    return true;
                }
            }
            return false;
        }

        private static void DeleteTranslatable(ITranslatable translatable, SqlCeConnection conn)
        {
            string query = string.Format("DELETE FROM {0} WHERE Id={1}", translatable.DBTranslatableTableName, translatable.ServConceptionId);
            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.ExecuteNonQuery();
        }

        internal static bool IsTranslationExists(ITranslatable translatable, SqlCeConnection conn)
        {
            string query = string.Format("SELECT Id FROM {0} WHERE {1}={2}", translatable.DBTableName, translatable.ServIdName, translatable.ServConceptionId);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }

            return false;
        }
        //internal static int InsertPartOfSpeech(string partOfSpeechTranslation, LanguageId languageId, SqlCeConnection conn)
        //{
        //    string query = "INSERT INTO PartOfSpeech (DefaultName) VALUES(NULL)";

        //    SqlCeCommand cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    cmd.ExecuteNonQuery();

        //    query = "SELECT @@IDENTITY;";

        //    cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    var id = cmd.ExecuteScalar();

        //    int partOfSpeechTranslationId = Convert.ToInt32(id);

        //    if (partOfSpeechTranslationId != 0)
        //    {
        //        query = "INSERT INTO PartOfSpeechTranslation (PartOfSpeechId, LangForId, Translation) VALUES(@PartOfSpeechId, @Langid, @Translation)";

        //        cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd.Parameters.Add("@PartOfSpeechId", SqlDbType.Int);
        //        cmd.Parameters.Add("@Langid", SqlDbType.Int);
        //        cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

        //        cmd.Parameters["@PartOfSpeechId"].Value = partOfSpeechTranslationId;
        //        cmd.Parameters["@Langid"].Value = (int)languageId + 1;
        //        cmd.Parameters["@Translation"].Value = partOfSpeechTranslation;

        //        cmd.ExecuteNonQuery();
        //    }
        //    return partOfSpeechTranslationId;

        //}

        //public static int UpdatePartOfSpeech(int partId, string partOfSpeechNewTranslation, LanguageId languageId)
        //{
        //    using (SqlCeConnection conn = new SqlCeConnection(s_ConnectionString))
        //    {
        //        conn.Open();
        //        return UpdatePartOfSpeech(partId, partOfSpeechNewTranslation, languageId, conn);
        //    }
        //}

        //public static int UpdatePartOfSpeech(int partId, string partOfSpeechNewTranslation, LanguageId languageId, SqlCeConnection conn)
        //{
        //    int partOfSpeechTranslationId = partId;

        //    if (partOfSpeechTranslationId != 0)
        //    {
        //        string query = "Update PartOfSpeechTranslation SET Translation=@Translation WHERE Id=@Id";
        //        SqlCeCommand cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd.Parameters.Add("@Id", SqlDbType.Int);
        //        cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

        //        cmd.Parameters["@Id"].Value = partOfSpeechTranslationId;
        //        cmd.Parameters["@Translation"].Value = partOfSpeechNewTranslation;

        //        cmd.ExecuteNonQuery();
        //    }
        //    return partOfSpeechTranslationId;

        //}

        //internal static int GetPartOfSpeechIdByLang(string partOfSpeechTranslation, LanguageId languageId, SqlCeConnection conn)
        //{
        //    string query = string.Format("SELECT PartOfSpeechId FROM PartOfSpeechTranslation WHERE LangForId={0} AND Translation='{1}'", (int)languageId + 1, partOfSpeechTranslation);

        //    SqlCeCommand cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    SqlCeDataReader reader = cmd.ExecuteReader();
        //    if (reader.Read())
        //    {
        //        return (int)reader[0];
        //    }

        //    return 0;
        //}

        //internal static int InsertChangable(ConceptionDescription.ChangeablelPart changeablelPart, LanguageId languageId, SqlCeConnection conn)
        //{
        //    return InsertChangable(changeablelPart.Type, languageId, conn);
        //}

        //internal static int InsertChangable(string changeableTypeTranslation, LanguageId languageId, SqlCeConnection conn)
        //{
        //    string query = "INSERT INTO ChangablePartType (DefaultName) VALUES(NULL)";

        //    SqlCeCommand cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    cmd.ExecuteNonQuery();

        //    query = "SELECT @@IDENTITY;";

        //    cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    var id = cmd.ExecuteScalar();

        //    int changableTypeId = Convert.ToInt32(id);

        //    if (changableTypeId != 0)
        //    {
        //        query = "INSERT INTO ChangableTranslation (ChangableTypeId, LangForId, Translation) VALUES(@ChangableTypeId, @Langid, @Translation)";

        //        cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd.Parameters.Add("@ChangableTypeId", SqlDbType.Int);
        //        cmd.Parameters.Add("@Langid", SqlDbType.Int);
        //        cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

        //        cmd.Parameters["@ChangableTypeId"].Value = changableTypeId;
        //        cmd.Parameters["@Langid"].Value = (int)languageId + 1;
        //        cmd.Parameters["@Translation"].Value = changeableTypeTranslation;

        //        cmd.ExecuteNonQuery();
        //    }
        //    return changableTypeId;

        //}
        //internal static int UpdateChangable(int changeableId, string translation, SqlCeConnection conn)
        //{
        //    if (changeableId != 0)
        //    {
        //        string query = "Update ChangableTranslation SET Translation=@Translation WHERE Id=@Id";
        //        SqlCeCommand cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd.Parameters.Add("@Id", SqlDbType.Int);
        //        cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

        //        cmd.Parameters["@Id"].Value = changeableId;
        //        cmd.Parameters["@Translation"].Value = translation;

        //        cmd.ExecuteNonQuery();
        //    }
        //    return changeableId;
        //}

        //internal static int GetChangableIdByLang(ConceptionDescription.ChangeablelPart changeablelPart, LanguageId languageId, SqlCeConnection conn)
        //{
        //    return GetChangableIdByLang(changeablelPart.Type, languageId, conn);
        //}

        //internal static int GetChangableIdByLang(string changeableTypeTranslation, LanguageId languageId, SqlCeConnection conn)
        //{
        //    string query = string.Format("SELECT ChangableTypeId FROM ChangableTranslation WHERE LangForId = {0} AND Translation='{1}'", (int)languageId + 1, changeableTypeTranslation);

        //    SqlCeCommand cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    SqlCeDataReader reader = cmd.ExecuteReader();
        //    if (reader.Read())
        //    {
        //        return (int)reader[0];
        //    }

        //    return 0;
        //}


        //public static int SavePartOfSpeech(string partOfSpeechTranslation, LanguageId langId)
        //{
        //    using (SqlCeConnection conn = new SqlCeConnection(s_ConnectionString))
        //    {
        //        conn.Open();
        //        return SavePartOfSpeech(conn, partOfSpeechTranslation, langId);
        //    }
        //}

        //public static int SavePartOfSpeech(SqlCeConnection conn, string partOfSpeechTranslation, LanguageId langId)
        //{
        //    if (string.IsNullOrEmpty(partOfSpeechTranslation))
        //        return 0;

        //    int partOfSpeechId = DatabaseHelper.GetPartOfSpeechIdByLang(partOfSpeechTranslation, langId, conn);
        //    if (partOfSpeechId == 0)
        //        partOfSpeechId = DatabaseHelper.InsertPartOfSpeech(partOfSpeechTranslation, langId, conn);

        //    return partOfSpeechId;
        //}

        public static string ConnectionString
        {
            get { return s_ConnectionString; }
            set { s_ConnectionString = value; }
        }

        internal static string GetAnyTranslation(ITranslatable translatable, SqlCeConnection conn)
        {
            string query = string.Format("SELECT Id, Translation FROM {0} WHERE {1}={2}", translatable.DBTableName, translatable.ServIdName, translatable.ServConceptionId);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                translatable.Id = (int)reader[0];
                return (string)reader[1];
            }

            return "";
        }

        internal static bool DeleteConceptionDescription(ConceptionDescription desc, SqlCeConnection conn)
        {
            if (desc.DescriptionId != 0)
            {
                string query = "DELETE FROM Description WHERE Id=@Id";
                SqlCeCommand cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@Id", SqlDbType.Int);

                cmd.Parameters["@Id"].Value = desc.DescriptionId;

                cmd.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        internal static bool DeleteConception(Conception conception, SqlCeConnection conn)
        {
            if (conception.ConceptionId != 0)
            {
                string query = "DELETE FROM Conception WHERE Id=@Id";
                SqlCeCommand cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@Id", SqlDbType.Int);

                cmd.Parameters["@Id"].Value = conception.ConceptionId;

                cmd.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        internal static int GetPartOfSpeechIdByLang(string partOfSpeechTranslation, LanguageId langId, SqlCeConnection conn)
        {
            string query = string.Format("SELECT PartOfSpeechId FROM PartOfSpeechTranslation WHERE LangForId={0} AND Translation='{1}'", (int)langId + 1, partOfSpeechTranslation);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }

        internal static int GetChangableIdByLang(ConceptionDescription.ChangeablelPart changeablelPart, LanguageId langId, SqlCeConnection conn)
        {
            string query = string.Format("SELECT ChangableTypeId FROM ChangableTranslation WHERE LangForId={0} AND Translation='{1}'", (int)langId + 1, changeablelPart.Type);

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }
    }
}
