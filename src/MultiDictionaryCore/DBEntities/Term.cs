using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.DBEntities
{
    public class Term
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int TopicId { get; set; }
        public int SemanticId { get; set; }
    }
}
