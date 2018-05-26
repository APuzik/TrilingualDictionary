using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.DBEntities
{
    public class TopicTranslation
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public int LangIdFor { get; set; }
        public string Translation { get; set; }
        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            TopicTranslation tt = (TopicTranslation)obj;
            return Id == tt.Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
