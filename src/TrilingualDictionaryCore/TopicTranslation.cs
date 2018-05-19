using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class TopicTranslation : AbstractTranslation
    {
        public override int Id { get; set; }
        
        public int TopicId { get; set; }
        public override int ServConceptionId { get { return TopicId; } set { TopicId = value; } }

        public override LanguageId LangForId { get; set; }
        public override string Translation { get; set; }

        public override string InsertTranslationQuery
        {
            get
            {
                return "INSERT INTO TopicTranslation (TopicId, LangIdFor, Translation) VALUES(@ServConceptionId, @LangId, @Translation)";
            }
        }
        public override string InsertServConceptionQuery
        {
            get
            {
                return "INSERT INTO Topic (DefaultName) VALUES(NULL)";
            }
        }

        public override string UpdateTranslationQuery
        {
            get { return "UPDATE TopicTranslation SET Translation=@Translation WHERE Id=@Id"; }
        }

        public override string DeleteTranslationQuery
        {
            get { return "DELETE FROM TopicTranslation WHERE Id=@Id"; }
        }

        public override string SelectServConceptionIdQuery
        {
            get { return "SELECT TopicId FROM TopicTranslation WHERE LangIdFor={0} AND Translation='{1}'"; }
        }

        public override string DBTableName
        {
            get { return "TopicTranslation"; }
        }

        public override string ServIdName
        {
            get { return "TopicId"; }
        }

        public override string DBTranslatableTableName
        {
            get { return "Topic"; }
        }
    }
}
