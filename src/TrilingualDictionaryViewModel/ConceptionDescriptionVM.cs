using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryViewModel
{
    public class ConceptionDescriptionVM
    {
        private LanguageId m_Language;
        List<ConceptionDescription> m_Descriptions;
        
        public ConceptionDescriptionVM(LanguageId langId, List<ConceptionDescription> descs)
        {
            m_Language = langId;
            m_Descriptions = descs;
        }
        
        public LanguageId Language
        {
            get { return m_Language; }
        }

        public List<ConceptionDescription> Descriptions
        {
            get { return m_Descriptions; }
        }
    }
}
