using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryViewModel
{
    public class ConceptionLanguageVM
    {
        private LanguageId m_Language;
        List<ConceptionDescriptionViewModel> m_Descriptions;

        public ConceptionLanguageVM(LanguageId langId, List<ConceptionDescriptionViewModel> descs)
        {
            m_Language = langId;
            m_Descriptions = descs;
        }
        
        public LanguageId Language
        {
            get { return m_Language; }
        }

        public List<ConceptionDescriptionViewModel> Descriptions
        {
            get { return m_Descriptions; }
        }
    }
}
