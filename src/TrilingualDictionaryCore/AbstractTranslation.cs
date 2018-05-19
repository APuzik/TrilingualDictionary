using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public abstract class AbstractTranslation : ITranslatable
    {
        public abstract int Id { get; set; }
        //Id of transatable entity
        public abstract int ServConceptionId { get; set; }
        public abstract LanguageId LangForId { get; set; }
        public abstract string Translation { get; set; }
        public abstract string InsertTranslationQuery { get; }
        public abstract string InsertServConceptionQuery { get; }
        public abstract string UpdateTranslationQuery { get; }
        public abstract string DeleteTranslationQuery { get; }
        public abstract string SelectServConceptionIdQuery { get; }
        public abstract string DBTableName { get; }
        public abstract string ServIdName { get; }
        public abstract string DBTranslatableTableName { get; }

        public int SaveToDB(LanguageId langId, bool isAddTranslation)
        {
            using (SqlCeConnection conn = new SqlCeConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                return SaveToDB(conn, langId, isAddTranslation);
            }
        }

        public int SaveToDB(SqlCeConnection conn, LanguageId langId, bool isAddTranslation)
        {
            if (string.IsNullOrEmpty(Translation))
                return 0;

            int dbTranslationId = Id;// DatabaseHelper.GetTranslatableIdByTranslation(this, langId, conn);
            if (dbTranslationId <= 0)
            {
                DatabaseHelper.InsertTranslatable(this, langId, conn);
                dbTranslationId = this.Id;
            }
            else if (isAddTranslation)
                dbTranslationId = DatabaseHelper.InsertTranslation(this, langId, conn, this.ServConceptionId);
            else
                dbTranslationId = DatabaseHelper.UpdateTranslation(this, langId, conn);

            return dbTranslationId;
        }

        public bool DeleteFromDB()
        {
            using (SqlCeConnection conn = new SqlCeConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                return DatabaseHelper.DeleteTranslation(this, conn);
            } 
        }

        public void GetAnyTranslation()
        {
            using (SqlCeConnection conn = new SqlCeConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                Translation = DatabaseHelper.GetAnyTranslation(this, conn);
            }
        }

    }
}
