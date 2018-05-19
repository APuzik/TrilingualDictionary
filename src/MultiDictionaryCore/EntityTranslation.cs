using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore
{
    /// <summary>
    /// This class describes certain entity
    /// </summary>
    public class EntityTranslation : Entity, ITranslation
    {
        public Entity Entity { get; set; }
        public string Value { get; set; }
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
