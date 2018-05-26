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
    }
}
