using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace MultiDictionaryCore
{
    public class DatabaseHelper
    {
        static string s_ConnectionString = @"Data Source=.\TrilingualDictionary.sdf;Password=Password1";
        public static string ConnectionString
        {
            get { return s_ConnectionString; }
            set { s_ConnectionString = value; }
        }
        
        internal static int InsertTranslationEntity(ITranslation translation, SqlCeConnection conn)
        {
            EntityTranslation translationObj = translation as EntityTranslation;

            string query = translationObj.DBOperations.InsertEntityQuery;

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            var id = cmd.ExecuteScalar();

            translation.EntityId = Convert.ToInt32(id);

            if (translation.EntityId != 0)
            {
                InsertTranslation(translationObj, conn);
            }

            return translation.EntityId;
        }

        private static int InsertTranslation(EntityTranslation translation, SqlCeConnection conn)
        {
            string query = translation.DBOperations.InsertTranslationQuery;

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.Parameters.Add("@EntityId", SqlDbType.Int);
            cmd.Parameters.Add("@LangId", SqlDbType.Int);
            cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

            cmd.Parameters["@EntityId"].Value = translation.EntityId;
            cmd.Parameters["@LangId"].Value = translation.Language.Id;
            cmd.Parameters["@Translation"].Value = translation.Translation;

            cmd.ExecuteNonQuery();

            query = "SELECT @@IDENTITY;";

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            translation.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return translation.Id;
        }      


        internal static int GetTranslationEntityIdByTranslation(ITranslation translation, SqlCeConnection conn)
        {
            EntityTranslation translationObj = translation as EntityTranslation;

            string query = translationObj.DBOperations.SelectEntityIdQuery;

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.Parameters.Add("@LangForId", SqlDbType.Int);
            cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

            cmd.Parameters["@LangForId"].Value = translation.Language.Id;
            cmd.Parameters["@Translation"].Value = translation.Translation;

            SqlCeDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (int)reader[0];
            }

            return 0;
        }

        internal static int UpdateTranslation(ITranslation translation, SqlCeConnection conn)
        {
            if (translation.Id != 0)
            {
                EntityTranslation translationObj = translation as EntityTranslation;

                string query = translationObj.DBOperations.UpdateTranslationQuery;
                SqlCeCommand cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@Translation", SqlDbType.NVarChar, 100);

                cmd.Parameters["@Id"].Value = translation.Id;
                cmd.Parameters["@Translation"].Value = translation.Translation;

                cmd.ExecuteNonQuery();
            }
            return translation.Id;
        }

        internal static bool DeleteTranslation(ITranslation translation, SqlCeConnection conn)
        {
            if (translation.Id != 0)
            {
                EntityTranslation translationObj = translation as EntityTranslation;

                string query = translationObj.DBOperations.DeleteTranslationQuery;
                SqlCeCommand cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd = new SqlCeCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters["@Id"].Value = translation.Id;

                cmd.ExecuteNonQuery();

                if (!IsAnyTranslationExists(translation, conn))
                {
                    DeleteTranslationEntity(translation, conn);
                    return true;
                }
            }
            return false;
        }

        private static void DeleteTranslationEntity(ITranslation translation, SqlCeConnection conn)
        {
            EntityTranslation translationObj = translation as EntityTranslation;

            string query = translationObj.DBOperations.DeleteTranslationEntityQuery;
            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = translation.EntityId;

            cmd.ExecuteNonQuery();
        }

        internal static bool IsAnyTranslationExists(ITranslation translation, SqlCeConnection conn)
        {
            EntityTranslation translationObj = translation as EntityTranslation;

            string query = translationObj.DBOperations.SelectAnyTranslationQuery;

            SqlCeCommand cmd = new SqlCeCommand(query);
            cmd.Connection = conn;

            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = translation.EntityId;

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

        //internal static string GetAnyTranslation(ITranslatable translatable, SqlCeConnection conn)
        //{
        //    string query = string.Format("SELECT Id, Translation FROM {0} WHERE {1}={2}", translatable.DBTableName, translatable.ServIdName, translatable.ServConceptionId);

        //    SqlCeCommand cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    SqlCeDataReader reader = cmd.ExecuteReader();
        //    if (reader.Read())
        //    {
        //        translatable.Id = (int)reader[0];
        //        return (string)reader[1];
        //    }

        //    return "";
        //}

        //internal static bool DeleteConceptionDescription(ConceptionDescription desc, SqlCeConnection conn)
        //{
        //    if (desc.DescriptionId != 0)
        //    {
        //        string query = "DELETE FROM Description WHERE Id=@Id";
        //        SqlCeCommand cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd.Parameters.Add("@Id", SqlDbType.Int);

        //        cmd.Parameters["@Id"].Value = desc.DescriptionId;

        //        cmd.ExecuteNonQuery();

        //        return true;
        //    }
        //    return false;
        //}

        //internal static bool DeleteConception(Conception conception, SqlCeConnection conn)
        //{
        //    if (conception.ConceptionId != 0)
        //    {
        //        string query = "DELETE FROM Conception WHERE Id=@Id";
        //        SqlCeCommand cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd = new SqlCeCommand(query);
        //        cmd.Connection = conn;

        //        cmd.Parameters.Add("@Id", SqlDbType.Int);

        //        cmd.Parameters["@Id"].Value = conception.ConceptionId;

        //        cmd.ExecuteNonQuery();

        //        return true;
        //    }
        //    return false;
        //}

        //internal static int GetPartOfSpeechIdByLang(string partOfSpeechTranslation, LanguageId langId, SqlCeConnection conn)
        //{
        //    string query = string.Format("SELECT PartOfSpeechId FROM PartOfSpeechTranslation WHERE LangForId={0} AND Translation='{1}'", (int)langId + 1, partOfSpeechTranslation);

        //    SqlCeCommand cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    SqlCeDataReader reader = cmd.ExecuteReader();
        //    if (reader.Read())
        //    {
        //        return (int)reader[0];
        //    }

        //    return 0;
        //}

        //internal static int GetChangableIdByLang(ConceptionDescription.ChangeablelPart changeablelPart, LanguageId langId, SqlCeConnection conn)
        //{
        //    string query = string.Format("SELECT ChangableTypeId FROM ChangableTranslation WHERE LangForId={0} AND Translation='{1}'", (int)langId + 1, changeablelPart.Type);

        //    SqlCeCommand cmd = new SqlCeCommand(query);
        //    cmd.Connection = conn;

        //    SqlCeDataReader reader = cmd.ExecuteReader();
        //    if (reader.Read())
        //    {
        //        return (int)reader[0];
        //    }

        //    return 0;
        //}
    }
}
