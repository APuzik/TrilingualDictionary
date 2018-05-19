using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class PartOfSpeechTranslation : AbstractTranslation
    {
        public override int Id { get; set; }

        public int PartOfSpeechId { get; set; }
        public override int ServConceptionId { get { return PartOfSpeechId; } set { PartOfSpeechId = value; } }

        public override LanguageId LangForId { get; set; }
        public override string Translation { get; set; }

        public override string InsertTranslationQuery
        {
            get
            {
                return "INSERT INTO PartOfSpeechTranslation (PartOfSpeechId, LangForId, Translation) VALUES(@ServConceptionId, @LangId, @Translation)";
            }
        }
        public override string InsertServConceptionQuery
        {
            get
            {
                return "INSERT INTO PartOfSpeech (DefaultName) VALUES(NULL)";
            }
        }

        public override string UpdateTranslationQuery
        {
            get { return "UPDATE PartOfSpeechTranslation SET Translation=@Translation WHERE Id=@Id"; }
        }

        public override string DeleteTranslationQuery
        {
            get { return "DELETE FROM PartOfSpeechTranslation WHERE Id=@Id"; }
        }

        public override string SelectServConceptionIdQuery
        {
            get { return "SELECT PartOfSpeechId FROM PartOfSpeechTranslation WHERE LangForId={0} AND Translation='{1}'"; }
        }

        public override string DBTableName
        {
            get { return "PartOfSpeechTranslation"; }
        }

        public override string ServIdName
        {
            get { return "PartOfSpeechId"; }
        }
        public override string DBTranslatableTableName
        {
            get { return "PartOfSpeech"; }
        }
    }
}
