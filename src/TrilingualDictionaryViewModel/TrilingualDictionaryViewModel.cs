using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using TrilingualDictionaryCore;
using System.Collections;
using System.Windows;

namespace TrilingualDictionaryViewModel
{
    public class TrilingualDictionaryViewModel
    {
        static ObservableCollection<string> m_Languages = null;

        private const string m_DictionaryXmlFile = "dict.xml";
        //private ObservableCollection<ConceptionViewModel> m_Conceptions = new ObservableCollection<ConceptionViewModel>();
        //private ObservableCollection<ConceptionDescriptionViewModel> m_CurLangDescriptions = new ObservableCollection<ConceptionDescriptionViewModel>();
        private TrilingualDictionary m_Dictionary = new TrilingualDictionary();
        ObservableCollection<ConceptionDescriptionViewModel> m_Parents = null;
        //SortedList<int, int> m_ParentIds = new SortedList<int, int>();
        Dictionary<int, int> m_ParentIds =null;
        IEnumerator<ConceptionDescriptionViewModel> m_matchedConDesc = null;

        string m_DictionaryDataFolder = "";

        private LanguageId m_MainLanguage = LanguageId.Russian;
        private ConceptionDescriptionViewModel m_descVM = null;

        private SortedDictionary<LanguageId, Alphabet> m_Alphabets = new SortedDictionary<LanguageId, Alphabet>();
        string m_oldSearch = "";
        //Chapter lastSearchChapter = null;
        //ConceptionDescriptionViewModel lastSearchConDescWoParent = null;
        //ConceptionDescriptionViewModel lastSearchConDescWithParent = null;

        int lastSearchChapter = 0;
        int lastSearchConDescWoParent = 0;
        int lastSearchConDescWithParent = 0;
        bool lastSearchParentFound = false;
        ConceptionDescriptionViewModel lastFoundDescription = null;

        private void AddAlphabet(LanguageId lang)
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
            m_Dictionary.LoadFromDB();// SerializeFromXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile));
            m_ParentIds = new Dictionary<int, int>();//int[m_Dictionary.ConceptionsCount];

            //m_Dictionary.SaveToDB();
            //m_Dictionary.SerializeToXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile + "2.xml"));

            //AddRussian();
            //AddEnglish();
            //AddUkrainian();

            LoadAllDescriptions();
        }

        public ObservableCollection<Chapter> Chapters
        {
            get { return m_Alphabets[MainLanguage].Letters; }
        }

        public ObservableCollection<TranslationVM> ActiveTopics
        {
            get
            {
                return GetActiveTranslatables(m_Dictionary.AllTopics);
            }
        }

        public ObservableCollection<TranslationVM> ActiveSemantics
        {
            get
            {
                return GetActiveTranslatables(m_Dictionary.AllSemantics);
            }
        }

        public ObservableCollection<TranslationVM> ActivePartsOfSpeech
        {
            get
            {
                return GetActiveTranslatables(m_Dictionary.AllLangParts);
            }
        }

        public ObservableCollection<TranslationVM> ActiveChangeables
        {
            get
            {
                return GetActiveTranslatables(m_Dictionary.AllChangeables);
            }
        }

        public ObservableCollection<ConceptionDescriptionViewModel> AllParents
        {
            get
            {
                return GetParents();
            }
        }

        public ObservableCollection<ConceptionDescriptionViewModel> GetParents()
        {
            if (m_Parents == null)
            {
                m_Parents = new ObservableCollection<ConceptionDescriptionViewModel>();
                m_Parents.Add(new ConceptionDescriptionViewModel(new ConceptionDescription(null, ""), null));

                List<Conception> parents = m_Dictionary.GetParents();
                foreach (Conception con in parents)
                {
                    List<ConceptionDescription> descs = con.GetConceptionDescriptions(m_MainLanguage);
                    if (descs.Count > 0)
                        m_Parents.Add(new ConceptionDescriptionViewModel(descs[0], null));
                }
            }
            return m_Parents;
        }

        private ObservableCollection<TranslationVM> GetActiveTranslatables(Dictionary<LanguageId, Dictionary<int, ITranslatable>> translatables)
        {
            ObservableCollection<TranslationVM> translatableValues =
                new ObservableCollection<TranslationVM>(GetTranslatables(translatables).Values);

            translatableValues.Insert(0, TranslationVM.CreateEmptyTopic());
            translatableValues.Add(TranslationVM.CreateAddItemTopic(MainLanguage));

            return translatableValues;
        }

        public Dictionary<int, TranslationVM> GetTranslatables(Dictionary<LanguageId, Dictionary<int, ITranslatable>> translatables)
        {
            Dictionary<int, TranslationVM> retVal = new Dictionary<int, TranslationVM>();
            foreach (LanguageId curLang in translatables.Keys)
            {
                //if (curLang == LanguageId.Undefined)
                //    continue;

                foreach (ITranslatable transl in translatables[curLang].Values)
                {
                    TranslationVM val;
                    bool isCurLang = (curLang == MainLanguage);
                    if (retVal.TryGetValue(transl.ServConceptionId, out val))
                    {
                        if (isCurLang)
                        {
                            retVal[transl.ServConceptionId] = new TranslationVM(transl, true);
                        }
                    }
                    else
                    {
                        retVal[transl.ServConceptionId] = new TranslationVM(transl, isCurLang);
                    }
                }
            }

            return retVal;
        }

        private void InitAlphabets()
        {
            foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
            {
                if (langId == LanguageId.Undefined)
                    continue;

                if (!m_Alphabets.ContainsKey(langId))
                    AddAlphabet(langId);
            }
        }

        public void LoadAllDescriptions()
        {
            InitAlphabets();

            foreach (Conception conception in m_Dictionary.Conceptions)
            {
                AddConceptionDescriptions(conception);
            }
        }

        private void AddConceptionDescriptions(Conception conception)
        {
            try
            {
                if (conception.ParentId == 0)
                {
                    AddParentDescriptionsIfAbsent(conception);
                }
                else
                {
                    AddChildDescriptions(conception);
                }
            }
            catch (Exception ex)
            {
                int s = 1;
            }
        }

        private void AddParentDescriptionsIfAbsent(Conception conception)
        {
            try
            {
                if (!IsParentAdded(conception.ConceptionId))
                {
                    AddParentDescriptions(conception);
                    m_ParentIds[conception.ConceptionId] = conception.ConceptionId;
                }
            }
            catch (Exception ex)
            {
                int j = 1;
            }
        }

        private bool IsParentAdded(int parentId)
        {
            if (m_ParentIds.ContainsKey(parentId))
                return m_ParentIds[parentId] != 0;
            else
                return false;
        }

        private void AddParentDescriptions(Conception conception)
        {
            try
            {
                foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
                {
                    if (langId == LanguageId.Undefined)
                        continue;

                    List<ConceptionDescription> descs = conception.GetConceptionDescriptions(langId);
                    if (descs.Count == 0)
                    {
                        AddAbsent(langId, conception);
                    }
                    else
                    {
                        AddReal(langId, descs);
                    }
                }
            }
            catch (Exception ex)
            {
                int j = 1;
            }
        }

        private void AddAbsent(LanguageId langId, Conception conception)
        {
            try
            {
                Chapter aChapter = m_Alphabets[langId].Absent;
                List<ConceptionDescription> descs = conception.GetAnyDescriptions();
                ConceptionDescriptionViewModel tmp = new ConceptionDescriptionViewModel(descs[0], null);
                tmp.IsAbsent = true;
                aChapter.AddChild(tmp);
            }
            catch(Exception ex)
            {
                int j = 1;
            }
        }

        private void AddReal(LanguageId langId, List<ConceptionDescription> descs)
        {
            try
            {
                foreach (ConceptionDescription desc in descs)
                {
                    Chapter aChapter = CreateChapter(langId, GetChapterName(desc));
                    aChapter.AddChild(new ConceptionDescriptionViewModel(desc, null));
                }
            }
            catch (Exception)
            {
                int j = 1;
            }
        }

        private void AddChildDescriptions(Conception conception)
        {
            try
            {
                foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
                {
                    if (langId == LanguageId.Undefined)
                        continue;

                    Conception parent = m_Dictionary.GetConception(conception.ParentId);
                    AddParentDescriptionsIfAbsent(parent);

                    List<ConceptionDescription> descsParent = parent.GetConceptionDescriptions(langId);

                    if (descsParent.Count == 0)
                    {
                        AddChildrenToAbsentChapter(langId, conception);
                    }
                    else
                    {
                        AddChildrenToRealChapters(langId, descsParent, conception);
                    }
                }
            }
            catch (Exception ex)
            {
                int j = 1;
            }
        }

        private void AddChildrenToAbsentChapter(LanguageId langId, Conception conception)
        {
            try
            {
                Chapter absentChapter = m_Alphabets[langId].Absent;

                List<ConceptionDescriptionViewModel> descsChildren = GetAnyDescriptions(langId, conception);

                AddChildrenToChapter(absentChapter, descsChildren);
            }
            catch (Exception ex)
            {
                int j = 1;
            }
        }

        private void AddChildrenToRealChapters(LanguageId langId, List<ConceptionDescription> descsParent, Conception conception)
        {
            try
            {
                List<ConceptionDescriptionViewModel> descsChildren = GetAnyDescriptions(langId, conception);

                foreach (ConceptionDescription descParent in descsParent)
                {
                    char c = GetChapterName(descParent);
                    Chapter aChapter = CreateChapter(langId, c);

                    AddChildrenToChapter(aChapter, descsChildren);
                }
            }
            catch (Exception ex)
            {
                int j = 1;
            }
        }

        private List<ConceptionDescriptionViewModel> GetAnyDescriptions(LanguageId langId, Conception conception)
        {
            try
            {
                List<ConceptionDescription> descsChildren = conception.GetConceptionDescriptions(langId);
                bool isAbsent = false;
                if (descsChildren.Count == 0)
                {
                    descsChildren.Add(conception.GetAnyDescriptions()[0]);
                    isAbsent = true;
                }
                List<ConceptionDescriptionViewModel> descsVM = new List<ConceptionDescriptionViewModel>();
                foreach (ConceptionDescription desc in descsChildren)
                {
                    ConceptionDescriptionViewModel tmp = new ConceptionDescriptionViewModel(desc, null);
                    tmp.IsAbsent = isAbsent;
                    descsVM.Add(tmp);
                }
                return descsVM;
            }
            catch (Exception ex)
            {
                int j = 1;
            }
            return null;
        }

        private static void AddChildrenToChapter(Chapter aChapter, List<ConceptionDescriptionViewModel> descsChildren)
        {
            foreach (ConceptionDescriptionViewModel descChild in descsChildren)
            {
                AddChildToChapter(aChapter, descChild);
            }
        }

        private static void AddChildToChapter(Chapter aChapter, ConceptionDescriptionViewModel descChild)
        {
            Conception conception = descChild.Conception;

            List<ConceptionDescriptionViewModel> descs = aChapter.GetDescVM(conception.ParentId);
            foreach (ConceptionDescriptionViewModel descVM in descs)
            {
                descVM.AddChild(descChild);
            }
        }


        private char GetChapterName(ConceptionDescription desc)
        {
            char c = '!';
            int i = 0;

            while (!Char.IsLetter(c) && i < desc.ConceptionRegistryDescription.Length)
            {
                c = desc.ConceptionRegistryDescription[i];
                i++;
            }
            return c;
        }

        private Chapter CreateChapter(LanguageId langId, char chapterName)
        {
            Chapter aChapter = m_Alphabets[langId].Find(chapterName);
            if (aChapter == null)
            {
                aChapter = m_Alphabets[langId].Add(chapterName);
            }
            return aChapter;
        }

        private void AddAbsent(LanguageId langId, List<ConceptionDescription> first, List<ConceptionDescription> second)
        {
            Chapter absentChapter = m_Alphabets[langId].Absent;

            ConceptionDescription descTmp = null;
            if (first.Count != 0)
                descTmp = first[0];
            else if (second.Count != 0)
                descTmp = second[0];
            if (descTmp != null)
                absentChapter.Descriptions.Add(new ConceptionDescriptionViewModel(descTmp, null));
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
                    ConceptionDescriptionViewModel conVM2 = new ConceptionDescriptionViewModel(desc, null);
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
                ////LoadAllDescriptions();
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

        public void AddDescriptionToConception(int conceptionId, string textDescription, LanguageId languageId,
                    int chnageablePartType,//string chnageablePartType,
                    string chnageablePartText,
                    int langPartId,//string langPartText,
                    bool bSave)
        {
            if (string.IsNullOrEmpty(textDescription))
                return;

            m_Dictionary.AddDescriptionToConception(conceptionId, textDescription, languageId,
                chnageablePartType,
                chnageablePartText,
                langPartId,
                bSave);
            Conception concept = m_Dictionary.GetConception(conceptionId);
            if (concept.ParentId == 0)//null)
            {
                ConceptionDescriptionViewModel newConeptionDesc = new ConceptionDescriptionViewModel(concept.GetConceptionDescription(languageId, textDescription), null);
                AddConceptionDescription(newConeptionDesc, languageId);
            }
            //Save();
        }

        private void AddConceptionDescription(ConceptionDescriptionViewModel newConeptionDescVM, LanguageId languageId)
        {
            GetChapter(newConeptionDescVM, languageId).AddChild(newConeptionDescVM);
        }

        //public void ChangeDescriptionOfConception(int conceptionId, string textDescription, LanguageId languageForEdit, int index)
        //{
        //    m_Dictionary.ChangeDescriptionOfConception(conceptionId, textDescription, languageForEdit, index);
        //    //Save();
        //}

        public void ChangeDescriptionOfConception(ConceptionDescriptionViewModel coneptionDescVM, string textDescription, LanguageId languageId,
                    int chnageablePartType,//string chnageablePartType,
                    string chnageablePartText,
                    int langPartId,//string langPartText,
            bool bSave)
        {
            Conception concept = m_Dictionary.GetConception(coneptionDescVM.Conception.ConceptionId);
            if (concept.ParentId == 0)
            {
                RemoveFromChapter(coneptionDescVM, languageId);
            }

            m_Dictionary.ChangeDescriptionOfConception(coneptionDescVM.Conception.ConceptionId, textDescription,
                                                        languageId, coneptionDescVM.ConceptionRegistryDescription,
                chnageablePartType,
                chnageablePartText,
                langPartId, 
                                                        bSave);


            if (concept.ParentId == 0)
            {
                AddConceptionDescription(coneptionDescVM, languageId);
            }

            //Save();
        }

        public void RemoveDescriptionFromConception(ConceptionDescriptionViewModel coneptionDescVM, LanguageId languageId, bool bSave)//int conceptionId, string textDescription, 
        {
            try
            {
                //bool hasParent = coneptionDescVM.Conception.ParentId != 0;
                m_Dictionary.RemoveDescriptionFromConception(coneptionDescVM.Conception.ConceptionId, coneptionDescVM.ConceptionRegistryDescription, languageId, bSave);
                if (bSave)
                {
                    Conception concept = m_Dictionary.GetConception(coneptionDescVM.Conception.ConceptionId);
                    if (concept == null || concept.ParentId == 0)
                    {
                        if (coneptionDescVM.Parent == null)
                            RemoveFromChapter(coneptionDescVM, languageId);
                        else
                            coneptionDescVM.Parent.RemoveChild(coneptionDescVM);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveFromChapter(ConceptionDescriptionViewModel coneptionDescVM, LanguageId languageId)
        {
            GetChapter(coneptionDescVM, languageId).RemoveChild(coneptionDescVM);
        }

        ////public int AddConception(string textDescription, LanguageId languageId)
        ////{
        ////    int newConceptionId = m_Dictionary.AddConception(textDescription, languageId);
        ////    Conception conception = m_Dictionary.GetConception(newConceptionId);

        ////    //m_CurLangDescriptions.Add(new ConceptionDescriptionViewModel(conception.GetConceptionDescription(MainLanguage, conception.GetDescriptionsCount(MainLanguage) - 1), true));

        ////    return newConceptionId;
        ////}

        public Conception GetConception(int conceptionId)
        {
            return m_Dictionary.GetConception(conceptionId);
        }

        //public void Save()
        //{
        //    m_Dictionary.SerializeToXML(Path.Combine(m_DictionaryDataFolder, m_DictionaryXmlFile));
        //}

        public void RemoveConception(ConceptionDescriptionViewModel conceptionDesc)
        {
            // m_CurLangDescriptions.Remove(conceptionDesc);
            m_Dictionary.RemoveConception(conceptionDesc.Conception.ConceptionId);
            RemoveFromChapter(conceptionDesc, MainLanguage);
            //for (int i = 0; i < conceptionDesc.Conception.DescriptionsCount; i++)
            //{
              //  RemoveDescriptionFromConception(conceptionDesc.Conception.RemoveDescription(.coneptionDescVM, languageId).RemoveChild(coneptionDescVM);
            //}
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

        public ConceptionLanguageVM ConceptionDescriptionVM
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }


        public void ReloadPartOfSpeech()
        {
            m_Dictionary.AllLangParts.Clear();
            m_Dictionary.LoadPartOfSpeech();
        }

        public void ReloadSemantics()
        {
            m_Dictionary.AllSemantics.Clear();
            m_Dictionary.LoadSemantic();
        }

        public void ReloadChangeables()
        {
            m_Dictionary.AllChangeables.Clear();
            m_Dictionary.LoadChangeable();
        }

        public void ReloadTopics()
        {
            m_Dictionary.AllTopics.Clear();
            m_Dictionary.LoadTopic();
        }
        public Chapter GetAnyChapter(ConceptionDescriptionViewModel descVM)
        {
            foreach (LanguageId lang in Enum.GetValues(typeof(LanguageId)))
            {
                if (lang == LanguageId.Undefined)
                    continue;

                Chapter c = GetChapter(descVM, lang);
                if (m_Alphabets[lang].Absent != c)
                    return c;
            }

            return null;
        }

        public Chapter GetChapter(ConceptionDescriptionViewModel descVM, LanguageId languageId)
        {
            string s = descVM.ConceptionRegistryDescription.Replace("«", "");
            ObservableCollection<Chapter> chapters = m_Alphabets[languageId].Letters;
            for (int i = 0; i < chapters.Count; i++)
            {
                if (string.Compare(chapters[i].Letter, s[0].ToString(), true) == 0)
                {
                    return chapters[i];
                }

            }
            return m_Alphabets[languageId].Absent;
        }

        public void AddConception(Conception conception)
        {
            m_Dictionary.AddConception(conception);
            AddConceptionDescriptions(conception);
        }

        public bool PerformSearch(string textToSearch, ConceptionDescriptionViewModel selectedItem)
        {
            if (m_oldSearch != textToSearch || lastFoundDescription == null || selectedItem != lastFoundDescription)
                //selectedItem != null && selectedItem.ConceptionDescription.DescriptionId != lastFoundDescription.ConceptionDescription.DescriptionId)
            {
                lastSearchChapter = 0;
                lastSearchConDescWoParent = 0;
                lastSearchConDescWithParent = -1;
                lastSearchParentFound = false;
                m_oldSearch = textToSearch;
            }
            ConceptionDescriptionViewModel description = FindMatches(textToSearch);
            if(description != null)
            {
                if (lastFoundDescription != null)
                    lastFoundDescription.IsSelected = false;
                
                description.IsExpanded = true;
                description.IsSelected = true;
                lastFoundDescription = description;
                return true;
            }
            else 
            {
                m_oldSearch = "";
            }
            //int counter = 0;
            //if (m_oldSearch != textToSearch)
            //{
            //    m_matchedConDesc = null;
            //    m_oldSearch = textToSearch;
            //}
            //while (counter < 2)
            //{
            //    if (m_matchedConDesc == null || !m_matchedConDesc.MoveNext())
            //        this.VerifyMatchingPeopleEnumerator(textToSearch);

            //    var description = m_matchedConDesc.Current;

            //    if (description == null)
            //    {
            //        if (counter > 0)
            //            return false;
            //        else
            //        {
            //            counter++;
            //            continue;
            //        }
            //    }

            //    // Ensure that this person is in view.
            //    if (description.Parent != null)
            //        description.Parent.IsExpanded = true;

            //    description.IsExpanded = true;
            //    description.IsSelected = true;

            //    return true;
            //}
            return false;
        }

        private void VerifyMatchingPeopleEnumerator(string textToSearch)
        {
          
            IEnumerable<ConceptionDescriptionViewModel> matches = FindMatches1(textToSearch);//, _rootPerson);
            m_matchedConDesc = matches.GetEnumerator();

            //if (!m_matchedConDesc.MoveNext())
            //{
            //    MessageBox.Show(
            //        "Не найдено.",
            //        "Try Again",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Information
            //        );
            //}
        }

        private ConceptionDescriptionViewModel FindMatches(string searchText)//, ConceptionDescriptionViewModel description)
        {
            //foreach (Chapter chapter in m_Alphabets[m_MainLanguage].Letters)
            for (int i = lastSearchChapter; i < m_Alphabets[m_MainLanguage].Letters.Count; i++)
            {
                Chapter chapter = m_Alphabets[m_MainLanguage].Letters[i];
                //foreach (ConceptionDescriptionViewModel description in chapter.Descriptions)
                for (int j = lastSearchConDescWoParent; j < chapter.Descriptions.Count; j++)
                {
                    ConceptionDescriptionViewModel description = chapter.Descriptions[j];
                    if (!lastSearchParentFound)
                    {
                        if (description.ContainsText(searchText))
                        {
                            lastSearchChapter = i;
                            lastSearchConDescWoParent = j;
                            lastSearchConDescWithParent = -1;
                            lastSearchParentFound = true;
                            chapter.IsExpanded = true;
                            return description;
                        }
                    }
                    for (int k = lastSearchConDescWithParent + 1; k < description.Children.Count; k++)
                    //foreach (ConceptionDescriptionViewModel child in description.Children)
                    {
                        ConceptionDescriptionViewModel child = description.Children[k];
                        //foreach (ConceptionDescriptionViewModel match in this.FindMatches2(searchText, child))
                        if (child.ContainsText(searchText))
                        {
                            lastSearchChapter = i;
                            lastSearchConDescWoParent = j;
                            lastSearchConDescWithParent = k;
                            lastSearchParentFound = true;
                            chapter.IsExpanded = true;
                            return child;
                        }
                    }
                    lastSearchParentFound = false;
                    lastSearchConDescWithParent = -1;
                }
                lastSearchConDescWoParent = 0;                
            }
            lastSearchChapter = 0;
            return null;
        }

        private IEnumerable<ConceptionDescriptionViewModel> FindMatches1(string searchText)//, ConceptionDescriptionViewModel description)
        {
            foreach (Chapter chapter in m_Alphabets[m_MainLanguage].Letters)
            {
                foreach (ConceptionDescriptionViewModel description in chapter.Descriptions)
                {
                    if (description.ContainsText(searchText))
                    {
                        chapter.IsExpanded = true;
                        yield return description;
                    }

                    foreach (ConceptionDescriptionViewModel child in description.Children)
                    {
                        foreach (ConceptionDescriptionViewModel match in this.FindMatches2(searchText, child))
                        {
                            chapter.IsExpanded = true;
                            yield return match;
                        }
                    }
                }
            }
        }

        private IEnumerable<ConceptionDescriptionViewModel> FindMatches2(string searchText, ConceptionDescriptionViewModel description)
        {
            if (description.ContainsText(searchText))
                yield return description;

            foreach (ConceptionDescriptionViewModel child in description.Children)
                foreach (ConceptionDescriptionViewModel match in this.FindMatches2(searchText, child))
                    yield return match;
        }
    }
    //public List<Alphabet> Alphabets
    //{
    //    get { return m_Alphabets.Values.ToList(); }
    //}
}
