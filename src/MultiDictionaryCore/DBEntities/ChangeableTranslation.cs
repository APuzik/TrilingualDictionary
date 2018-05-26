﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.DBEntities
{
    public class ChangeableTranslation
    {
        public int Id { get; set; }
        public int ChangeableTypeId { get; set; }
        public int LangForId { get; set; }
        public string Translation { get; set; }
        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            ChangeableTranslation tt = (ChangeableTranslation)obj;
            return Id == tt.Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
