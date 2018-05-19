using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public interface ITranslatable
    {
        int Id { get; set; }
        //Id of transatable entity
        int ServConceptionId { get; set; }
        LanguageId LangForId { get; set; }
        string Translation { get; set; }
        int SaveToDB(LanguageId langId, bool isAddTranslation);
        int SaveToDB(SqlCeConnection conn, LanguageId langId, bool isAddTranslation);
        string InsertTranslationQuery { get; }
        string InsertServConceptionQuery { get; }
        string UpdateTranslationQuery { get; }
        string DeleteTranslationQuery { get; }
        string SelectServConceptionIdQuery { get; }

        bool DeleteFromDB();

        string DBTableName { get; }

        string ServIdName { get; }

        string DBTranslatableTableName { get; }

        void GetAnyTranslation();
    }
}
