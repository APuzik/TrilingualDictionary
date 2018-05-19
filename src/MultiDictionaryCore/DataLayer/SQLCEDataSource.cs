using MultiDictionaryCore.Core;
using MultiDictionaryCore.DataLayer.Interfaces;
using MultiDictionaryCore.DBEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.DataLayer
{
    public class SQLCEDataSource : IDataSource
    {
        public SQLCEDataSource(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                ConnectionString = connectionString;
            }

            dbConnection = new SqlCeConnection(ConnectionString);
            dbConnection.Open();
        }

        public List<TermTranslation> GetAllTranslations(int languageId)
        {
            List<TermTranslation> translations = new List<TermTranslation>();
            string query = "SELECT * FROM Description WHERE Language=@Lang";
            //string query = "SELECT * FROM Description INNER JOIN Conception ON (Description.ConceptionId = Conception.Id AND Conception.ParentId=0 AND Description.Language=@Lang)";

            using (SqlCeCommand cmd = new SqlCeCommand(query))
            {
                cmd.Connection = dbConnection;
                cmd.Parameters.Add("@Lang", SqlDbType.Int);
                cmd.Parameters["@Lang"].Value = languageId;

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TermTranslation translation = CreateTranslation(reader);
                        translations.Add(translation);
                    }
                }
            }

            // move to ViewModel
            //translations.Sort
            //    (
            //        (trans1, trans2) =>
            //        {
            //            return trans1.CleanValue.CompareTo(trans2.CleanValue);
            //        }
            //    );

            return translations;
        }

        public Term GetTerm(TermTranslation translation)
        {
            Term term = null;

            string query = "SELECT * FROM Conception WHERE Id=@Id";

            using (SqlCeCommand cmd = new SqlCeCommand(query))
            {
                cmd.Connection = dbConnection;
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters["@Id"].Value = translation.TermId;

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        term = CreateTerm(reader);
                    }
                }
            }

            return term;
        }

        public List<TermTranslation> GetTranslationsForTerm(int languageId, int termId)
        {
            List<TermTranslation> translations = new List<TermTranslation>();
            string query = "SELECT * FROM Description WHERE Language=@Lang AND ConceptionId=@TermId";

            using (SqlCeCommand cmd = new SqlCeCommand(query))
            {
                cmd.Connection = dbConnection;
                cmd.Parameters.Add("@Lang", SqlDbType.Int);
                cmd.Parameters["@Lang"].Value = languageId;

                cmd.Parameters.Add("@TermId", SqlDbType.Int);
                cmd.Parameters["@TermId"].Value = termId;

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TermTranslation translation = CreateTranslation(reader);
                        translations.Add(translation);
                    }
                }
            }

            return translations;
        }

        public List<Term> GetChildrenTerms(int parentTermId)
        {
            List<Term> terms = new List<Term>();

            string query = "SELECT * FROM Conception WHERE ParentId=@parentId";

            using (SqlCeCommand cmd = new SqlCeCommand(query))
            {
                cmd.Connection = dbConnection;
                cmd.Parameters.Add("@parentId", SqlDbType.Int);
                cmd.Parameters["@parentId"].Value = parentTermId;

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Term term = CreateTerm(reader);
                        terms.Add(term);
                    }
                }
            }

            return terms;
        }

        public List<Term> GetAllChildrenTerms()
        {
            List<Term> terms = new List<Term>();

            string query = "SELECT * FROM Conception WHERE ParentId<>0";

            using (SqlCeCommand cmd = new SqlCeCommand(query))
            {
                cmd.Connection = dbConnection;

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Term term = CreateTerm(reader);
                        terms.Add(term);
                    }
                }
            }

            return terms;
        }

        public List<TermTranslation> GetChildrenTranslations(int languageId, int parentTermId)
        {
            List<TermTranslation> translations = new List<TermTranslation>();

            string query = "SELECT * FROM Description INNER JOIN Conception ON (Description.ConceptionId = Conception.Id AND Conception.ParentId=@ParentId AND Description.Language=@Lang)";

            using (SqlCeCommand cmd = new SqlCeCommand(query))
            {
                cmd.Connection = dbConnection;
                cmd.Parameters.Add("@ParentId", SqlDbType.Int);
                cmd.Parameters["@ParentId"].Value = parentTermId;

                cmd.Parameters.Add("@Lang", SqlDbType.Int);
                cmd.Parameters["@Lang"].Value = languageId;

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TermTranslation translation = CreateTranslation(reader);
                        translations.Add(translation);
                    }
                }
            }

            return translations;
        }

        public List<TermTranslation> GetChildrenTranslations(int languageId)
        {
            List<TermTranslation> translations = new List<TermTranslation>();

            string query = "SELECT * FROM Description INNER JOIN Conception ON (Description.ConceptionId = Conception.Id AND Conception.ParentId<>0 AND Description.Language=@Lang)";

            using (SqlCeCommand cmd = new SqlCeCommand(query))
            {
                cmd.Connection = dbConnection;

                cmd.Parameters.Add("@Lang", SqlDbType.Int);
                cmd.Parameters["@Lang"].Value = languageId;

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TermTranslation translation = CreateTranslation(reader);
                        translations.Add(translation);
                    }
                }
            }

            return translations;
        }

        private Term CreateTerm(SqlCeDataReader reader)
        {
            Term term = new Term();

            term.Id = (int)reader[0];
            term.ParentId = reader.IsDBNull(1) ? 0 : (int)reader[1];
            term.TopicId = reader.IsDBNull(2) ? 0 : (int)reader[2];
            term.SemanticId = reader.IsDBNull(3) ? 0 : (int)reader[3];

            return term;
        }

        private TermTranslation CreateTranslation(SqlCeDataReader reader)
        {
            TermTranslation translation = new TermTranslation();

            if (reader.IsDBNull(0))
            {
                int j = 1;
            }
            translation.Id = (int)reader[0];
            if (reader.IsDBNull(1))
            {
                int j = 1;
            }
            translation.TermId = (int)reader[1];
            if (reader.IsDBNull(2))
            {
                int j = 1;
            }
            translation.Value = (string)reader[2];
            if (reader.IsDBNull(3))
            {
                int j = 1;
            }
            translation.LanguageId = (int)reader[3] - 1;
            translation.ChangeablePart = reader.IsDBNull(4) ? "" : (string)reader[4];
            translation.ChangeableType = reader.IsDBNull(5) ? 0 : (int)reader[5];
            translation.PartOfSpeech = reader.IsDBNull(6) ? 0 : (int)reader[6];

            return translation;
        }

        public int AddTranslation(TermTranslation translation)
        {
            string query = "INSERT INTO Description (ConceptionId, Description, Language, ChangablePart, ChangableType, PartOfSpeech) VALUES(@ServConceptionId, @LangId, @Translation)";

            using (SqlCeCommand cmd = new SqlCeCommand(query))
            {
                cmd.Connection = dbConnection;

                cmd.Parameters.Add("@ConceptionId", SqlDbType.Int);
                cmd.Parameters.Add("@Language", SqlDbType.Int);
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@ChangablePart", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@ChangableType", SqlDbType.Int);
                cmd.Parameters.Add("@PartOfSpeech", SqlDbType.Int);

                cmd.Parameters["@ConceptionId"].Value = translation.TermId;
                cmd.Parameters["@Language"].Value = translation.LanguageId;
                cmd.Parameters["@Description"].Value = translation.Value;
                cmd.Parameters["@ChangablePart"].Value = translation.ChangeablePart;
                cmd.Parameters["@ChangableType"].Value = translation.ChangeableType;
                cmd.Parameters["@PartOfSpeech"].Value = translation.PartOfSpeech;

                cmd.ExecuteNonQuery();

                query = "SELECT @@IDENTITY;";

                using (SqlCeCommand cmd2 = new SqlCeCommand(query))
                {
                    translation.Id = Convert.ToInt32(cmd2.ExecuteScalar());
                }
            }
            return translation.Id;
        }

        public string ConnectionString { get; internal set; } = @"Data Source=.\TrilingualDictionary.sdf;Password=Password1";

        private SqlCeConnection dbConnection = null;
    }
}
