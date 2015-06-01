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
        static ObservableCollection<string> m_Languages = null;

        private const string m_DictionaryXmlFile = "dict.xml";
        //private ObservableCollection<ConceptionViewModel> m_Conceptions = new ObservableCollection<ConceptionViewModel>();
        //private ObservableCollection<ConceptionDescriptionViewModel> m_CurLangDescriptions = new ObservableCollection<ConceptionDescriptionViewModel>();
        private TrilingualDictionary m_Dictionary = new TrilingualDictionary();
        string m_DictionaryDataFolder = "";

        private LanguageId m_MainLanguage = LanguageId.Russian;
        private ConceptionDescriptionViewModel m_descVM = null;

        private Dictionary<LanguageId, Alphabet> m_Alphabets = new Dictionary<LanguageId, Alphabet>();
        public List<Alphabet> Alphabets
        {
            get { return m_Alphabets.Values.ToList(); }
        }

        public void AddAlphabet(LanguageId lang)
        {
            switch (lang)
            {
                case LanguageId.Russian:
                    AddRussian();
                    break;
                case LanguageId.English:
                    AddEnglish();
                    break;
                case LanguageId.Ukrainian:
                    AddUkrainian();
                    break;
            }
        }

        void AddRussian()
        {
            m_Alphabets[LanguageId.Russian] = new Alphabet(LanguageId.Russian);
            for (char c = 'А'; c <= 'Я'; c++)
                m_Alphabets[LanguageId.Russian].Add(c);

            m_Alphabets[LanguageId.Russian].Add('Ё');
            m_Alphabets[LanguageId.Russian].Letters = new ObservableCollection<Chapter>(m_Alphabets[LanguageId.Russian].Letters.OrderBy(a => a.Letter));
        }
        void AddEnglish()
        {
            m_Alphabets[LanguageId.English] = new Alphabet(LanguageId.English);
            for (char c = 'A'; c <= 'Z'; c++)
                m_Alphabets[LanguageId.English].Add(c);

            m_Alphabets[LanguageId.English].Letters = new ObservableCollection<Chapter>(m_Alphabets[LanguageId.English].Letters.OrderBy(a => a.Letter));
        }

        void AddUkrainian()
        {
            m_Alphabets[LanguageId.Ukrainian] = new Alphabet(LanguageId.Ukrainian);

            Alphabet alphabetUkr = m_Alphabets[LanguageId.Ukrainian];
            alphabetUkr.Add('А');
            alphabetUkr.Add('Б');
            alphabetUkr.Add('В');
            alphabetUkr.Add('Г');
            alphabetUkr.Add('Ґ');
            alphabetUkr.Add('Д');
            alphabetUkr.Add('Е');
            alphabetUkr.Add('Є');
            alphabetUkr.Add('Ж');
            alphabetUkr.Add('З');
            alphabetUkr.Add('И');
            alphabetUkr.Add('І');
            alphabetUkr.Add('Ї');
            alphabetUkr.Add('К');
            alphabetUkr.Add('Л');
            alphabetUkr.Add('М');
            alphabetUkr.Add('Н');
            alphabetUkr.Add('О');
            alphabetUkr.Add('П');
            alphabetUkr.Add('Р');
            alphabetUkr.Add('С');
            alphabetUkr.Add('Т');
            alphabetUkr.Add('У');
            alphabetUkr.Add('Ф');
            alphabetUkr.Add('Х');
            alphabetUkr.Add('Ц');
            alphabetUkr.Add('Ч');
            alphabetUkr.Add('Ш');
            alphabetUkr.Add('Щ');
            alphabetUkr.Add('Ь');
            alphabetUkr.Add('Ю');
            alphabetUkr.Add('Я');

            m_Alphabets[LanguageId.Ukrainian].Letters = new ObservableCollection<Chapter>(m_Alphabets[LanguageId.Ukrainian].Letters.OrderBy(a => a.Letter));
        }

        public TrilingualDictionaryViewModel(string dictionaryDataFolder)
        {
            m_DictionaryDataFolder = dictionaryDataFolder;
            //m_Dictionary.Load(m_DictionaryDataFolder);
            m_Dictionary.SerializeFromXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile));
            m_Dictionary.SaveToDB();
            //m_Dictionary.SerializeToXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile + "2.xml"));

            AddRussian();
            AddEnglish();
            AddUkrainian();

            LoadAllDescriptions();
        }

        public ObservableCollection<Chapter> Chapters
        {
            get { return m_Alphabets[MainLanguage].Letters; }
        }

        public void LoadAllDescriptions()
        {
            m_Alphabets[MainLanguage].Letters.Clear();
            AddRussian();
            AddEnglish();
            AddUkrainian();

            foreach (Conception conception in m_Dictionary.Conceptions)
            {
                bool isLangMatches = true;
                List<ConceptionDescription> descs = conception.GetAllConceptionDescriptions(MainLanguage);

                if (descs.Count == 0)
                {
                    isLangMatches = false;
                    descs = conception.GetAnyDescriptions();
                }

                if (conception.ParentConception == null)
                {
                    FillChapters(isLangMatches, descs);
                }
                else
                {
                    LanguageId curLang = MainLanguage;
                    string sLetter = conception.GetParentNameForSorting(MainLanguage);
                    if (string.IsNullOrEmpty(sLetter))
                        curLang = LanguageId.Russian;

                    sLetter = conception.GetParentNameForSorting(curLang);
                    sLetter = sLetter[0].ToString();
                    Chapter chap = null;
                    for (int i = 0; i < m_Alphabets[MainLanguage].Letters.Count; i++)
                    {
                        Chapter aChapter = m_Alphabets[MainLanguage].Letters[i];
                        if (!isLangMatches)
                        {
                            if (string.Compare(aChapter.Letter, "Отсутствует", true) == 0)
                            {
                                chap = aChapter;
                                break;
                            }
                        }

                        if (string.Compare(aChapter.Letter, sLetter, true) == 0)
                        {
                            chap = aChapter;
                            break;
                        }
                    }
                    if (chap != null)
                    {
                        if (m_descVM == null || (m_descVM.Conception.ConceptionId != conception.ParentId))
                        {
                            foreach (ConceptionDescriptionViewModel descVM in chap.Descriptions)
                            {
                                if (descVM.Conception.ConceptionId == conception.ParentId)
                                {
                                    m_descVM = descVM;
                                    //foreach (ConceptionDescription desc in descs)
                                    //{
                                    //    ConceptionDescriptionViewModel descVM2 = new ConceptionDescriptionViewModel(desc);
                                    //    descVM2.IsAbsent = !isLangMatches;
                                    //    descVM.AddChild(descVM2);
                                    //}
                                }
                            }
                        }

                        foreach (ConceptionDescription desc in descs)
                        {
                            ConceptionDescriptionViewModel descVM2 = new ConceptionDescriptionViewModel(desc);
                            descVM2.IsAbsent = !isLangMatches;
                            m_descVM.AddChild(descVM2);
                        }
                    }
                }
            }

        }

        private void FillChapters(bool isLangMatches, List<ConceptionDescription> descs)
        {
            foreach (ConceptionDescription desc in descs)
            {
                Chapter chap = null;
                for (int j = 0; j < m_Alphabets[MainLanguage].Letters.Count; j++)
                {
                    Chapter aChapter = m_Alphabets[MainLanguage].Letters[j];
                    if (!isLangMatches)
                    {
                        if (string.Compare(aChapter.Letter, "Отсутствует", true) == 0)
                        {
                            chap = aChapter;
                            break;
                        }
                    }

                    if (desc.ConceptionRegistryDescriptionWoAccents.Length == 0)
                        break;

                    int i = 0;
                    for (; i < desc.ConceptionRegistryDescriptionWoAccents.Length && !Char.IsLetter(desc.ConceptionRegistryDescriptionWoAccents[i]); i++)
                    {

                    }
                    int res = string.Compare(aChapter.Letter, desc.ConceptionRegistryDescriptionWoAccents[i].ToString(), true);
                    if (res == 0)
                    {
                        chap = aChapter;
                        break;
                    }
                }

                if (chap != null)
                {
                    ConceptionDescriptionViewModel conVM2 = new ConceptionDescriptionViewModel(desc);
                    conVM2.IsAbsent = !isLangMatches;
                    chap.Descriptions.Add(conVM2);
                }
            }
        }        

        private void Init()
        {
            if (m_Languages == null)
            {
                m_Languages = new ObservableCollection<string>();
                foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
                {
                    if (langId == LanguageId.Undefined)
                        continue;

                    m_Languages.Add(langId.ToString());
                }
            }
        }

        public ObservableCollection<string> Languages
        {
            get
            {
                Init();
                return m_Languages;
            }
        }

        //public ObservableCollection<ConceptionDescriptionViewModel> ConceptionDescriptions
        //{
        //    get { return m_CurLangDescriptions; }
        //}

        public LanguageId MainLanguage
        {
            get { return m_MainLanguage; }
            set
            {
                m_MainLanguage = value;
                //m_CurLangDescriptions.Clear();
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

        public void ChangeDescriptionOfConception(int conceptionId, string textDescription, LanguageId languageForEdit, int index)
        {
            m_Dictionary.ChangeDescriptionOfConception(conceptionId, textDescription, languageForEdit, index);
            Save();
        }

        public void ChangeDescriptionOfConception(int conceptionId, string textDescription, LanguageId languageForEdit, string textOld)
        {
            m_Dictionary.ChangeDescriptionOfConception(conceptionId, textDescription, languageForEdit, textOld);
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

            //m_CurLangDescriptions.Add(new ConceptionDescriptionViewModel(conception.GetConceptionDescription(MainLanguage, conception.GetDescriptionsCount(MainLanguage) - 1), true));

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
           // m_CurLangDescriptions.Remove(conceptionDesc);
            m_Dictionary.RemoveConception(conceptionDesc.Conception.ConceptionId);
            Save();
        }

        public void Refresh()
        {
            //CollectionViewSource.GetDefaultView(m_CurLangDescriptions).Refresh();
        }

        public Alphabet Alphabet
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ConceptionVM ConceptionVM
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ConceptionDescriptionVM ConceptionDescriptionVM
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

    }

    public class Alphabet
    {
        ObservableCollection<Chapter> m_Letters = new ObservableCollection<Chapter>();
        LanguageId m_Language;

        public Alphabet(LanguageId lang)
        {
            m_Language = lang;
            m_Letters.Add(new Chapter("Отсутствует"));
        }

        public void Add(char c)
        {
            m_Letters.Add(new Chapter(c));
        }

        public LanguageId Language
        {
            get { return m_Language; }
        }

        public ObservableCollection<Chapter> Letters
        {
            get { return m_Letters; }
            set { m_Letters = value; }
        }

        public Chapter Chapter
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }

    public class Chapter : IComparable<Chapter>
    {
        string m_Letter;
        ObservableCollection<ConceptionDescriptionViewModel> m_Descriptions = new ObservableCollection<ConceptionDescriptionViewModel>();

        public ObservableCollection<ConceptionDescriptionViewModel> Descriptions
        {
            get { return m_Descriptions; }
        }

        public string Letter
        {
            get { return m_Letter; }
        }

        public ConceptionDescriptionViewModel ConceptionDescriptionViewModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        public Chapter(char c)
        {
            m_Letter = c.ToString();
        }

        public Chapter(string name)
        {
            m_Letter = name;
        }

        public int CompareTo(Chapter other)
        {
            return string.Compare(this.Letter, other.Letter, true);
        }
    }
}
