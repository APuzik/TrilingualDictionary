using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryViewModel
{
    public class ConceptionVM
    {
        List<ConceptionLanguageVM> m_LanguagesTreeItems = new List<ConceptionLanguageVM>();
        ObservableCollection<ConceptionDescriptionViewModel> m_Descriptions = new ObservableCollection<ConceptionDescriptionViewModel>();
        Conception m_Conception = null;
        
        public ConceptionVM(Conception conception)
        {
            m_Conception = conception;
            foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
            {
                if (langId == LanguageId.Undefined)
                    continue;

                List<ConceptionDescription> descs = conception.GetConceptionDescriptions(langId);
                if (descs.Count == 0)
                    descs = ConceptionDescription.EmptyList;

                List<ConceptionDescriptionViewModel> descsVM = new List<ConceptionDescriptionViewModel>();
                foreach(ConceptionDescription desc in descs)
                {
                    ConceptionDescriptionViewModel tmp = new ConceptionDescriptionViewModel(desc, null);
                    if (descs == ConceptionDescription.EmptyList)
                        tmp.IsAbsent = true;

                    descsVM.Add(tmp);
                }
                m_LanguagesTreeItems.Add(new ConceptionLanguageVM(langId, descsVM));
            }
        }

        public List<ConceptionLanguageVM> Descriptions
        {
            get { return m_LanguagesTreeItems; }
        }

        public Conception Conception
        {
            get { return m_Conception; }
        }
    }

    //public class ConceptionViewModel
    //{
    //    public class ConceptionsComparer : IComparer
    //    {
    //        private LanguageId m_LanguageId;

    //        public ConceptionsComparer(LanguageId languageId)
    //        {
    //            m_LanguageId = languageId;
    //        }

    //        public int Compare(Conception x, Conception y)
    //        {
    //            try
    //            {

    //                if (x.ParentConception != null && y.ParentConception != null && x.ParentConception != y.ParentConception)
    //                {
    //                    return String.Compare(x.GetParentNameForSorting(m_LanguageId), y.GetParentNameForSorting(m_LanguageId), StringComparison.InvariantCultureIgnoreCase);
    //                }
    //                else
    //                {
    //                    return String.Compare(x.GetOwnNameForSorting(m_LanguageId), y.GetOwnNameForSorting(m_LanguageId), StringComparison.InvariantCultureIgnoreCase);
    //                }

    //            }
    //            catch (Exception ex)
    //            {

    //                throw;
    //            }
    //        }

    //        public int Compare(object x, object y)
    //        {
    //            ConceptionViewModel x1 = x as ConceptionViewModel;
    //            ConceptionViewModel y1 = y as ConceptionViewModel;
    //            return Compare(x1.Conception, y1.Conception);
    //        }
    //    };

    //    public static bool BracketsrFilter(object item)
    //    {
    //        ConceptionViewModel conception = item as ConceptionViewModel;
    //        return conception.Conception.IsPotentialUndefinded();
    //    }

    //    private Conception m_Conception = null;
    //    private static LanguageId s_MainLanguage = LanguageId.Russian;

    //    public ConceptionViewModel(Conception conception)
    //    {
    //        m_Conception = conception;
    //    }

    //    //public string ActiveConceptionRegistryDescription
    //    //{
    //    //    get
    //    //    {
    //    //        return m_Conception.GetConceptionDescriptionOrEmpty(MainLanguage).ConceptionRegistryDescription;
    //    //    }
    //    //}

    //    public static LanguageId MainLanguage
    //    {
    //        get { return s_MainLanguage; }
    //        set { s_MainLanguage = value; }
    //    }

    //    public string ParentName
    //    {
    //        get { return m_Conception.GetParentNameForSorting(MainLanguage); }
    //    }

    //    public string OwnName
    //    {
    //        get { return m_Conception.GetOwnNameForSorting(MainLanguage); }
    //    }

    //    public bool Find(string textToSearch)
    //    {
    //        return m_Conception.Find(textToSearch, MainLanguage);
    //    }

    //    //public string GetConceptionRegistryDescriptionOrEmpty(LanguageId languageId)
    //    //{
    //    //    return GetConceptionDescriptionOrEmpty(languageId).ConceptionRegistryDescription;
    //    //}

    //    public int ConceptionId
    //    {
    //        get { return m_Conception.ConceptionId; }
    //    }

    //    //public ConceptionDescription GetConceptionDescriptionOrEmpty(LanguageId languageId)
    //    //{
    //    //    return m_Conception.GetConceptionDescriptionOrEmpty(languageId);
    //    //}

    //    //public ConceptionDescription GetConceptionDescription(LanguageId languageId)
    //    //{
    //    //    return m_Conception.GetConceptionDescription(languageId);
    //    //}

    //    public Conception Conception
    //    {
    //        get { return m_Conception; }
    //    }
    //}
}
