using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore
{
    class EntityTranslation : Entity, ITranslation
    {
        public Entity Entity { get; set; }
        public string Translation { get; set; }
        public Language Language { get; set; }

        public ITranslationDBOperations DBOperations { get; set; }

        public int EntityId
        {
            get
            {
                return Entity.Id;
            }
            set
            {
                Entity.Id = value;
            }
        }
    }
}
