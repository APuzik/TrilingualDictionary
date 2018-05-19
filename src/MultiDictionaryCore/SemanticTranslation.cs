using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore
{
    class SemanticTranslationDBOperation : ITranslationDBOperations
    {
        public string InsertTranslationQuery
        {
            get { return "INSERT INTO SemanticTranslation (SemId, LangForId, Translation) VALUES(@ServConceptionId, @LangId, @Translation)"; }
        }

        public string InsertEntityQuery
        {
            get { return "INSERT INTO Semantic (DefaultName) VALUES(NULL)"; }
        }

        public string UpdateTranslationQuery
        {
            get { return "UPDATE SemanticTranslation SET Translation=@Translation WHERE Id=@Id"; }
        }

        public string DeleteTranslationQuery
        {
            get { return "DELETE FROM SemanticTranslation WHERE Id=@Id"; }
        }

        public string SelectEntityIdQuery
        {
            get { return "SELECT SemId FROM SemanticTranslation WHERE LangForId=@LangForId AND Translation=@Translation"; }
        }

        public string SelectTranslationQuery
        {
            get { return "SELECT Translation FROM SemanticTranslation WHERE LangForId=@LangForId AND SemId=@SemId"; }
        }

        public string SelectAnyTranslationQuery
        {
            get { return "SELECT Id FROM SemanticTranslation WHERE SemId=@SemId"; }
        }

        public string DeleteTranslationEntityQuery
        {
            get { return "DELETE FROM Semantic WHERE Id=@Id"; }

        }
    }
}
