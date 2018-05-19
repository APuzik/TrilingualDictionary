using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    class LanguageTranslation : AbstractTranslation
    {
        public override int Id { get; set; }
        
        public LanguageId LangIdNeeded { get; set; }
        public override int ServConceptionId { get { return (int)LangIdNeeded; } set { LangIdNeeded = (LanguageId)value; } }

        public override LanguageId LangForId { get; set; }
        public override string Translation { get; set; }

        public override string InsertTranslationQuery
        {
            get
            {
                return "INSERT INTO LangTranslation (LangIdNeeded, LangIdFor, Translation) VALUES(@ServConceptionId, @LangId, @Translation)";
            }
        }
        public override string InsertServConceptionQuery
        {
            get
            {
                return "INSERT INTO Languages (DefaultName) VALUES(NULL)";
            }
        }

        public override string UpdateTranslationQuery
        {
            get { return "UPDATE LangTranslation SET Translation=@Translation WHERE Id=@Id"; }
        }

        public override string DeleteTranslationQuery
        {
            get { return "DELETE FROM LangTranslation WHERE Id=@Id"; }
        }

        public override string SelectServConceptionIdQuery
        {
            get { return "SELECT LangIdNeeded FROM LangTranslation WHERE LangIdFor={0} AND Translation='{1}'"; }
        }

        public override string DBTableName
        {
            get { return "LangTranslation"; }
        }

        public override string ServIdName
        {
            get { return "LangIdNeeded"; }
        }

        public override string DBTranslatableTableName
        {
            get { return "Languages"; }
        }
    }
}
