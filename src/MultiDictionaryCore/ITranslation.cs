using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace MultiDictionaryCore
{
    public interface ITranslation
    {
        int Id { get; set; }
        //Id of entity to translate
        int EntityId { get; set; }
        Language Language { get; set; }
        string Translation { get; set; }
    }

    public interface ITranslationDBOperations
    {
        string InsertTranslationQuery { get; }
        string InsertEntityQuery { get; }
        string UpdateTranslationQuery { get; }
        string DeleteTranslationQuery { get; }
        string DeleteTranslationEntityQuery { get; }
        string SelectEntityIdQuery { get; }
        string SelectTranslationQuery { get; }
        string SelectAnyTranslationQuery { get; }               
    }
}
