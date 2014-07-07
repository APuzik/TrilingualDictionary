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
        private ObservableCollection<ConceptionViewModel> m_Conceptions = new ObservableCollection<ConceptionViewModel>();
        private TrilingualDictionary m_Dictionary = new TrilingualDictionary();
        string m_DictionaryDataFolder = "";
        
        public TrilingualDictionaryViewModel(string dictionaryDataFolder)
        {
            m_DictionaryDataFolder = dictionaryDataFolder;
            //m_Dictionary.Load(m_DictionaryDataFolder);
            m_Dictionary.SerializeFromXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile));
            m_Dictionary.SerializeToXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile + ".xml"));

            foreach (Conception conception in m_Dictionary.Conceptions)
            {
                m_Conceptions.Add(new ConceptionViewModel(conception));
            }
        }

        public ObservableCollection<ConceptionViewModel> Conceptions
        {
            get { return m_Conceptions; }
        }

        public Conception.LanguageId MainLanguage
        {
            get { return ConceptionViewModel.MainLanguage; }
            set { ConceptionViewModel.MainLanguage = value; }
        }

        public void AddDescriptionToConception(int conceptionId, string textDescription, Conception.LanguageId languageId)
        {
            m_Dictionary.AddDescriptionToConception(conceptionId, textDescription, languageId);
            Save();
        }

        public void ChangeDescriptionOfConception(int conceptionId, string textDescription, Conception.LanguageId languageForEdit)
        {
            m_Dictionary.ChangeDescriptionOfConception(conceptionId, textDescription, languageForEdit);
            Save();
        }

        public void RemoveDescriptionFromConception(int conceptionId, Conception.LanguageId languageId)
        {
            m_Dictionary.RemoveDescriptionFromConception(conceptionId, languageId);
            Save();
        }

        public int AddConception(string textDescription, Conception.LanguageId languageId)
        {
            int newConceptionId = m_Dictionary.AddConception(textDescription, languageId);
            m_Conceptions.Add(new ConceptionViewModel(m_Dictionary.GetConception(newConceptionId)));

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

        public void RemoveConception(ConceptionViewModel conception)
        {
            m_Conceptions.Remove(conception);
            m_Dictionary.RemoveConception(conception.ConceptionId);
            Save();
        }

        public void Refresh()
        {
            CollectionViewSource.GetDefaultView(m_Conceptions).Refresh();
        }
    }
}
