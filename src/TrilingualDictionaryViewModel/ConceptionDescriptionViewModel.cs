using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryViewModel
{
    public class ConceptionDescriptionViewModel
    {
        public class ConceptionDescriptionComparer : IComparer
        {
            private LanguageId m_LanguageId;

            public ConceptionDescriptionComparer(LanguageId languageId)
            {
                m_LanguageId = languageId;
            }

            public int Compare(ConceptionDescription desc1, ConceptionDescription desc2)
            {
                try
                {
                    Conception x = desc1.OwnConception;
                    Conception y = desc2.OwnConception;
                    if (x.ParentConception != null && y.ParentConception != null && x.ParentConception != y.ParentConception)
                    {
                        return String.Compare(x.GetParentNameForSorting(m_LanguageId), y.GetParentNameForSorting(m_LanguageId), StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        return String.Compare(x.GetOwnNameForSorting(m_LanguageId), y.GetOwnNameForSorting(m_LanguageId), StringComparison.InvariantCultureIgnoreCase);
                    }

                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            public int Compare(object x, object y)
            {
                ConceptionDescriptionViewModel x1 = x as ConceptionDescriptionViewModel;
                ConceptionDescriptionViewModel y1 = y as ConceptionDescriptionViewModel;
                return Compare(x1.ObjectDescription, y1.ObjectDescription);
            }
        };

        private ConceptionDescription m_Description;
        private bool m_isDecriptionMatchesLanguage;

        public ConceptionDescriptionViewModel(ConceptionDescription desc, bool isDecriptionMatchesLanguage)
        {            
            m_Description = desc;
            m_isDecriptionMatchesLanguage = isDecriptionMatchesLanguage;
        }
        public ConceptionDescription ObjectDescription
        {
            get { return m_Description; }
        }

        public string RegistryDescription
        {
            get { return m_Description.ConceptionRegistryDescription; }
        }

        public bool IsDescriptionMatchesLanguage
        {
            get { return m_isDecriptionMatchesLanguage; }
        }

        public string RegistryDescriptionWoAccent
        {
            get { return m_Description.ConceptionRegistryDescriptionWoAccents; }
        }

        public static bool BracketsrFilter(object item)
        {
            ConceptionDescriptionViewModel conceptionDescription = item as ConceptionDescriptionViewModel;

            return conceptionDescription.ObjectDescription.OwnConception.IsPotentialUndefinded();
        }

        public static bool SameDescriptionsFilter(object item)
        {
            ConceptionDescriptionViewModel conceptionDescription = item as ConceptionDescriptionViewModel;

            return conceptionDescription.ObjectDescription.OwnConception.IsPotentialUndefinded();
        }
    }
}
