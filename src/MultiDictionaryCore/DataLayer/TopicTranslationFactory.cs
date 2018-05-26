using MultiDictionaryCore.DataLayer.Interfaces;
using MultiDictionaryCore.DBEntities;
using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.DataLayer
{
    class TopicTranslationFactory : IDBEntityFactory<TopicTranslation>
    {
        public TopicTranslation CreateDBEntity(SqlCeDataReader reader)
        {
            TopicTranslation item = new TopicTranslation
            {
                Id = (int)reader[0],
                TopicId = (int)reader[1],
                LangIdFor = (int)reader[2],
                Translation = (string)reader[3]
            };

            return item;                    
        }

        public TopicTranslation CreateDBEntity()
        {
            return new TopicTranslation();
        }
    }

    class SemanticTranslationFactory : IDBEntityFactory<SemanticTranslation>
    {
        public SemanticTranslation CreateDBEntity(SqlCeDataReader reader)
        {
            SemanticTranslation item = new SemanticTranslation
            {
                Id = (int)reader[0],
                SemId = (int)reader[1],
                LangForId = (int)reader[2],
                Translation = (string)reader[3]
            };

            return item;
        }

        public SemanticTranslation CreateDBEntity()
        {
            return new SemanticTranslation();
        }
    }

    class ChangeableTranslationFactory : IDBEntityFactory<ChangeableTranslation>
    {
        public ChangeableTranslation CreateDBEntity(SqlCeDataReader reader)
        {
            ChangeableTranslation item = new ChangeableTranslation
            {
                Id = (int)reader[0],
                ChangeableTypeId = (int)reader[1],
                LangForId = (int)reader[2],
                Translation = (string)reader[3]
            };

            return item;
        }

        public ChangeableTranslation CreateDBEntity()
        {
            return new ChangeableTranslation();
        }
    }

    class PartOfSpeechTranslationFactory : IDBEntityFactory<PartOfSpeechTranslation>
    {
        public PartOfSpeechTranslation CreateDBEntity(SqlCeDataReader reader)
        {
            PartOfSpeechTranslation item = new PartOfSpeechTranslation
            {
                Id = (int)reader[0],
                PartOfSpeechId = (int)reader[1],
                LangForId = (int)reader[2],
                Translation = (string)reader[3]
            };

            return item;
        }

        public PartOfSpeechTranslation CreateDBEntity()
        {
            return new PartOfSpeechTranslation();
        }
    }
}
