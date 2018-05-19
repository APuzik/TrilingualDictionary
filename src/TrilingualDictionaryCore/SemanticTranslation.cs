using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class SemanticTranslation : AbstractTranslation
    {
        //Database id of translation entity.        
        public override int Id { get; set; }
        
        //Id of transatable entity of Semantic table        
        public int SemId { get; set; }
        //Id of transatable entity. Needed for Interface methods.
        public override int ServConceptionId { get { return SemId; } set { SemId = value; } }

        public override LanguageId LangForId { get; set; }
        public override string Translation { get; set; }

        public override string InsertTranslationQuery
        {
            get
            {
                return "INSERT INTO SemanticTranslation (SemId, LangForId, Translation) VALUES(@ServConceptionId, @LangId, @Translation)";
            }
        }
        public override string InsertServConceptionQuery
        {
            get
            {
                return "INSERT INTO Semantic (DefaultName) VALUES(NULL)";
            }
        }

        public override string UpdateTranslationQuery
        {
            get { return "UPDATE SemanticTranslation SET Translation=@Translation WHERE Id=@Id"; }
        }

        public override string DeleteTranslationQuery
        {
            get { return "DELETE FROM SemanticTranslation WHERE Id=@Id"; }
        }

        public override string SelectServConceptionIdQuery
        {
            get { return "SELECT SemId FROM SemanticTranslation WHERE LangForId={0} AND Translation='{1}'"; }
        }
        public override string DBTableName
        {
            get { return "SemanticTranslation"; }
        }

        public override string ServIdName
        {
            get { return "SemId"; }
        }
        public override string DBTranslatableTableName
        {
            get { return "Semantic"; }
        }
    }
}
