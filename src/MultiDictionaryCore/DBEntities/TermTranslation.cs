using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.DBEntities
{
    public class TermTranslation
    {
        public int Id { get; set; }
        public int TermId { get; set; }
        public string Value { get; set; }
        public string CleanValue
        {
            get
            {
                if (string.IsNullOrEmpty(m_Value))
                {
                    m_Value = new string(Value.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray());
                }

                return m_Value;
            }
        }
        public int LanguageId { get; set; }
        public string ChangeablePart { get; set; }
        public int ChangeableType { get; set; }
        public int PartOfSpeech { get; set; }
        public override string ToString()
        {
            return Value;
        }

        private string m_Value;
    }
}
