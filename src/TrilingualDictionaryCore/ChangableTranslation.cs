using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    class ChangableTranslation : AbstractTranslation
    {
        public override int Id { get; set; }
        
        public int ChangeableTypeId { get; set; }
        public override int ServConceptionId { get { return ChangeableTypeId; } set { ChangeableTypeId = value; } }
        
        public override LanguageId LangForId { get; set; }
        public override string Translation { get; set; }

        
        public override string InsertTranslationQuery 
        {
            get
            {
                return "INSERT INTO ChangableTranslation (ChangableTypeId, LangForId, Translation) VALUES(@ServConceptionId, @LangId, @Translation)";
            }
        }
        public override string InsertServConceptionQuery
        {
            get
            {
                return "INSERT INTO ChangablePartType (DefaultName) VALUES(NULL)";
            }
        }

        public override string UpdateTranslationQuery
        {
            get { return "UPDATE ChangableTranslation SET Translation=@Translation WHERE Id=@Id"; }
        }

        public override string DeleteTranslationQuery
        {
            get { return "DELETE FROM ChangableTranslation WHERE Id=@Id"; }
        }

        public override string SelectServConceptionIdQuery
        {
            get { return "SELECT ChangableTypeId FROM ChangableTranslation WHERE LangForId={0} AND Translation='{1}'"; }
        }

        public override string DBTableName
        {
            get { return "ChangableTranslation"; }
        }

        public override string ServIdName
        {
            get { return "ChangableTypeId"; }
        }
        public override string DBTranslatableTableName
        {
            get { return "ChangablePartType"; }
        }
    }
}
