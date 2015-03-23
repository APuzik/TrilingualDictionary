using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryViewModel
{
    public class TrilingualDictionaryViewModel
    {
        private const string m_DictionaryXmlFile = "dict.xml";
        //private ObservableCollection<ConceptionViewModel> m_Conceptions = new ObservableCollection<ConceptionViewModel>();
        private ObservableCollection<ConceptionDescriptionViewModel> m_CurLangDescriptions = new ObservableCollection<ConceptionDescriptionViewModel>();
        private TrilingualDictionary m_Dictionary = new TrilingualDictionary();
        string m_DictionaryDataFolder = "";
        
        public TrilingualDictionaryViewModel(string dictionaryDataFolder)
        {
            m_DictionaryDataFolder = dictionaryDataFolder;
            //m_Dictionary.Load(m_DictionaryDataFolder);
            m_Dictionary.SerializeFromXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile));
            m_Dictionary.SerializeToXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile));// + ".xml"));

            LoadAllDescriptions();
        }

        public void LoadAllDescriptions()
        {
            foreach (Conception conception in m_Dictionary.Conceptions)
            {
                bool isLangMatches = true;
                List<ConceptionDescription> descs = conception.GetAllConceptionDescriptions(MainLanguage);
                if (descs.Count == 0)
                {
                    isLangMatches = false;
                    foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
                    {
                        descs = conception.GetAllConceptionDescriptions(langId);
                        if (descs.Count > 0)
                            break;
                    }
                }

                foreach (ConceptionDescription desc in descs)
                    m_CurLangDescriptions.Add(new ConceptionDescriptionViewModel(desc, isLangMatches));
            }
        }

        public ObservableCollection<ConceptionDescriptionViewModel> ConceptionDescriptions
        {
            get { return m_CurLangDescriptions; }
        }

        public LanguageId MainLanguage
        {
            get { return ConceptionViewModel.MainLanguage; }
            set 
            {
                ConceptionViewModel.MainLanguage = value;
                m_CurLangDescriptions.Clear();
                LoadAllDescriptions();
                //if (value != ConceptionViewModel.MainLanguage)
                //{
                //    ConceptionViewModel.MainLanguage = value;
                //    m_CurLangDescriptions.Clear();
                //    foreach (Conception conception in m_Dictionary.Conceptions)
                //    {
                //        List<ConceptionDescription> descriptions = conception.GetAllConceptionDescriptions(ConceptionViewModel.MainLanguage);
                //        foreach (ConceptionDescription desc in descriptions)
                //        {
                //            m_CurLangDescriptions.Add(new ConceptionDescriptionViewModel(desc));
                //        }
                //    }
                    
                //}
            }
        }

        public void AddDescriptionToConception(int conceptionId, string textDescription, LanguageId languageId)
        {
            m_Dictionary.AddDescriptionToConception(conceptionId, textDescription, languageId);
            Save();
        }

        public void ChangeDescriptionOfConception(int conceptionId, string textDescription, LanguageId languageForEdit)
        {
            m_Dictionary.ChangeDescriptionOfConception(conceptionId, textDescription, languageForEdit);
            Save();
        }

        public void RemoveDescriptionFromConception(int conceptionId, string descriptionText, LanguageId languageId)
        {
            m_Dictionary.RemoveDescriptionFromConception(conceptionId, descriptionText, languageId);
            Save();
        }

        public int AddConception(string textDescription, LanguageId languageId)
        {
            int newConceptionId = m_Dictionary.AddConception(textDescription, languageId);
            Conception conception = m_Dictionary.GetConception(newConceptionId);

            m_CurLangDescriptions.Add(new ConceptionDescriptionViewModel(conception.GetConceptionDescription(MainLanguage, conception.GetDescriptionsCount(MainLanguage)), true));

            return newConceptionId;
        }

        public Conception GetConception(int conceptionId)
        {
            return m_Dictionary.GetConception(conceptionId);
        }

        public void Save()
        {
            m_Dictionary.SerializeToXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile));
        }

        public void RemoveConception(ConceptionDescriptionViewModel conceptionDesc)
        {
            m_CurLangDescriptions.Remove(conceptionDesc);
            m_Dictionary.RemoveConception(conceptionDesc.ObjectDescription.OwnConception.ConceptionId);
            Save();
        }

        public void Refresh()
        {
            CollectionViewSource.GetDefaultView(m_CurLangDescriptions).Refresh();
        }

    }
}
