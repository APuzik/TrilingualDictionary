using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiDictionaryCore.Core.Interfaces;
using MultiDictionaryCore.Core;
using MultiDictionaryCore.DBEntities;
using System.Windows.Input;
using MultiDictionaryViewModel.Commands;
using System.Windows;
using System.ComponentModel;

namespace MultiDictionaryViewModel
{
    public class MultiDictionaryVM : INotifyPropertyChanged
    {
        public ObservableCollection<string> Languages { get; set; } = new ObservableCollection<string> { "Русский", "English", "Українська" };
        ObservableCollection<TreeNode> letters = new ObservableCollection<TreeNode>();
        public ObservableCollection<TreeNode> Letters
        {
            get { return letters; }
            set
            {
                letters = value;
                OnPropertyChanged("Letters");
            }
        }
        public Dictionary<int, ObservableCollection<TreeNode>> AllLetters { get; set; } = new Dictionary<int, ObservableCollection<TreeNode>>();

        public IMultiLingualDictionary Dictionary { get; set; } = new MultiLingualDictionary();

        public ICommand ExpandCommand { get; set; }
        public ICommand SwitchLanguage { get; set; }
        public ICommand SelectedTranslationChanged { get; set; }

        public ICommand AddTerm { get; set; }
        public ICommand DuplicateTerm { get; set; }
        public ICommand MergeTerms { get; set; }
        public ICommand EditTerm { get; set; }
        public ICommand DeleteTerm { get; set; }
        public ICommand FindNextTranslation { get; set; }

        public int SelectedLanguage { get { return SelectedItem + 1; } }
        public int SelectedItem { get; set; } = 0;
        TreeNode sel = null;
        public TreeNode SelectedTranslation { get; set; }
        //{
        //    get { return sel; }
        //    set { sel = value; OnPropertyChanged("qwe"); }
        //}

        public TermVM SelectedTerm { get; set; } = new TermVM();
        public string Topic { get; set; } = "topic";
        public string Semantic { get; set; } = "sem";
        public ObservableCollection<TreeNode> Langs { get; set; }

        public ObservableCollection<TreeNode> SelectedNodes { get; set; }
        public ObservableCollection<TreeNode> SelectedTermNodes { get; set; }

        public MultiDictionaryVM()
        {
            SwitchLanguage = new RelayCommand { ExecuteAction = LoadTranslations };
            AddTerm = new RelayCommand { ExecuteAction = AddNewTerm };
            DuplicateTerm = new RelayCommand { ExecuteAction = MakeTermCopy, CanExecutePredicate = IsOneItemSelected };
            MergeTerms = new RelayCommand { ExecuteAction = MergeTwoTerms, CanExecutePredicate = CanMerge };
            EditTerm = new RelayCommand { ExecuteAction = EditSelectedTerm, CanExecutePredicate = IsOneItemSelected };
            DeleteTerm = new RelayCommand { ExecuteAction = DeleteWholeTerm, CanExecutePredicate = IsTermSelected };
            //FindNextTranslation = new RelayCommand { ExecuteAction = SearchTranslation };
            SelectedTranslationChanged = new RelayCommand { ExecuteAction = SelectedTransChanged };

            SelectedNodes = new ObservableCollection<TreeNode>();
            SelectedNodes.CollectionChanged += SelectedNodes_CollectionChanged;

            LoadTranslations(false);
            SelectedTerm.Dictionary = Dictionary;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void LoadTranslations(bool force)
        {
            //Letters.Clear();
            if (force)
            {
                AllLetters.Clear();
                Letters.Clear();
            }
            int lang = SelectedLanguage - 1;
            if (AllLetters.ContainsKey(lang))
            {
                Letters = AllLetters[lang];
                return;
            }
            else
            {
                ObservableCollection<TreeNode> newLang = new ObservableCollection<TreeNode>();
                AllLetters.Add(lang, newLang);
                Letters = newLang;
            }
            //List<TermTranslation> translations = Dictionary.GetAllTranslations(SelectedLanguage);
            List<Term> terms = Dictionary.GetTopTerms(SelectedLanguage);
            SortedDictionary<int, Term> dicTerms = new SortedDictionary<int, Term>();
            foreach (Term term in terms)
            {
                dicTerms.Add(term.Id, term);
            }
            List<TermTranslation> translations = Dictionary.GetTopTranslations(SelectedLanguage);
            translations.Sort((a, b) => { return string.Compare(a.CleanValue, b.CleanValue); });
            SortedDictionary<string, List<TermTranslation>> sorted = new SortedDictionary<string, List<TermTranslation>>();
            for (int i = 0; i < translations.Count; i++)
            {
                string key = GetKey(translations[i].Value);

                if (!sorted.ContainsKey(key))
                {
                    sorted.Add(key, new List<TermTranslation>());
                }

                sorted[key].Add(translations[i]);
            }

            List<TermTranslation> childTranslations = Dictionary.GetChildrenTranslations(SelectedLanguage);
            childTranslations.Sort((a, b) => { return string.Compare(a.CleanValue, b.CleanValue); });
            List<Term> childTerms = Dictionary.GetAllChildrenTerms();

            SortedDictionary<int, List<Term>> dicChildTerms = new SortedDictionary<int, List<Term>>();
            SortedDictionary<int, int> termsMapping = new SortedDictionary<int, int>();
            foreach (Term term in childTerms)
            {
                termsMapping.Add(term.Id, term.ParentId);
                if (!dicChildTerms.ContainsKey(term.ParentId))
                {
                    dicChildTerms.Add(term.ParentId, new List<Term>());
                }
                dicChildTerms[term.ParentId].Add(term);
            }

            //int = term id
            SortedDictionary<int, List<TreeNode>> treeNodes = new SortedDictionary<int, List<TreeNode>>();

            foreach (KeyValuePair<string, List<TermTranslation>> node in sorted)
            {
                var children = new ObservableCollection<TreeNode>();
                TreeNode item = new TreeNode { Translation = null, Name = node.Key.ToString(), Children = children, IsHeader = true };
                foreach (TermTranslation tt in node.Value)
                {
                    //List<TermTranslation> ttChilds = Dictionary.GetChildrenTranslations(SelectedLanguage, tt.TermId);
                    //var children2 = new ObservableCollection<TreeNode>();
                    //foreach (TermTranslation tt2 in ttChilds)
                    //{
                    //    children2.Add(new TreeNode { Name = tt2.Value });
                    //}
                    TreeNode tn = new TreeNode { Name = tt.Value, Translation = tt };
                    if (!treeNodes.ContainsKey(tt.TermId))
                    {
                        treeNodes.Add(tt.TermId, new List<TreeNode>());
                    }
                    treeNodes[tt.TermId].Add(tn);
                    tn.Parent = item;
                    children.Add(tn);
                }
                Letters.Add(item);
            }

            foreach (TermTranslation tt in childTranslations)
            {
                int nodeId = termsMapping[tt.TermId];
                TreeNode tn = new TreeNode { Name = tt.Value, Translation = tt };
                if (treeNodes.ContainsKey(nodeId))
                {
                    foreach (TreeNode tnParent in treeNodes[nodeId])
                    {
                        tn.Parent = tnParent;
                        tnParent.Children.Add(tn);
                    }
                }
            }
        }

        string GetKey(string value)
        {
            char key = Char.ToUpperInvariant(value[0]);
            int j = 1;
            while (!Char.IsLetter(key) && j < value.Length)
            {
                key = Char.ToUpperInvariant(value[j]);
                j++;
            }
            if (j == value.Length)
            {
                key = Char.ToUpperInvariant(value[0]);
            }

            return key.ToString(); ;
        }

        private void LoadTranslations(object parameter)
        {
            SelectedNodes.Clear();
            LoadTranslations(false);
        }

        private void AddNewTerm(object parameter)
        {

        }
        private void MakeTermCopy(object parameter)
        {
        }
        private void MergeTwoTerms(object parameter)
        {
            if (SelectedNodes.Count != 2)
            {
                MessageBox.Show("Для объединения необходимо выбрать два термина", "Объединение терминов", MessageBoxButton.OK);
                return;
            }
        }
        private void EditSelectedTerm(object parameter)
        {
        }
        private void DeleteWholeTerm(object parameter)
        {
            MessageBoxResult confirmResult = MessageBox.Show("Вы уверены, что хотите удалить выбранный термин, все его переводы и дочерние термины целиком?", "Удаление термина", MessageBoxButton.YesNo);
            if (confirmResult == MessageBoxResult.Yes)
            {
                int termId = SelectedNodes[0].Translation.TermId;
                foreach (KeyValuePair<int, ObservableCollection<TreeNode>> kv in AllLetters)
                {
                    DeleteTranslations(kv.Key, termId);
                }
                Dictionary.DeleteTerm(termId);
            }
        }

        string lastSearched = "";
        SearchData lastSearch = new SearchData();
        public TreeNode SearchTranslation(string textToSearch)
        {
            if (textToSearch != lastSearched)
            {
                lastSearch = new SearchData();
            }
            lastSearched = textToSearch;

            for (int i = lastSearch.ChapterIndex; i < Letters.Count; i++)
            {
                TreeNode chapter = Letters[i];
                for (int j = lastSearch.TopTranslationIndex; j < chapter.Children.Count; j++)
                {
                    TreeNode parent = chapter.Children[j];
                    if (!lastSearch.IsParentFound)
                    {
                        string clear = parent.Name;
                        if(!textToSearch.Contains("#"))
                            clear = parent.Name.Replace("#", "");
                        if (clear.Contains(textToSearch))
                        {
                            lastSearch.ChapterIndex = i;
                            lastSearch.TopTranslationIndex = j;
                            lastSearch.ChildTranslationIndex = -1;
                            lastSearch.IsParentFound = true;
                            parent.IsExpanded = true;
                            parent.IsSelected = true;
                            return parent;
                        }
                    }
                    for (int k = lastSearch.ChildTranslationIndex + 1; k < parent.Children.Count; k++)
                    {
                        TreeNode child = parent.Children[k];
                        string clear = child.Name;
                        if (!textToSearch.Contains("#"))
                            clear = child.Name.Replace("#", "");
                        if (clear.Contains(textToSearch))
                        {
                            lastSearch.ChapterIndex = i;
                            lastSearch.TopTranslationIndex = j;
                            lastSearch.ChildTranslationIndex = k;
                            lastSearch.IsParentFound = true;
                            child.IsExpanded = true;
                            child.IsSelected = true;
                            return child;
                        }
                    }
                    lastSearch.IsParentFound = false;
                    lastSearch.ChildTranslationIndex = -1;
                }
                lastSearch.TopTranslationIndex = 0;
            }
            lastSearch.ChapterIndex = 0;
            return null;
        }

        public void UpdateTranslations(TermVM newVM)
        {
            int lang = SelectedLanguage - 1;
            foreach (KeyValuePair<int, ObservableCollection<TreeNode>> kv in AllLetters)
            {
                UpdateTranslations(kv.Key, newVM);
            }

            //LoadTranslations(true);
            //var translations = newVM.Languages[SelectedLanguage].Children;
            ////for (int k = 0; k < translations.Count; k++)
            ////{
            ////    if (translations[k].Translation.Id == tt.Id)
            ////    {
            ////        Letters[i].Children[j].Translation = tt;
            ////        Letters[i].Children[j].Name = tt.Value;
            ////        break;
            ////    }
            ////}
            //for (int i = 0; i < Letters.Count; i++)
            //{
            //    for(int j = 0; j < Letters[i].Children.Count; j++)
            //    {
            //        TermTranslation tt = Letters[i].Children[j].Translation;
            //        for (int k = 0; k < translations.Count; k++)
            //        {
            //            if (translations[k].Translation.Id == tt.Id)
            //            {
            //                Letters[i].Children[j].Translation = tt;
            //                Letters[i].Children[j].Name = tt.Value;
            //                break;
            //            }
            //        }

            //        for(int k = 0; k < Letters[i].Children[j].Children.Count; k++)
            //        {

            //        }
            //    }
            //}
        }

        private void DeleteTranslations(int lang, int termId)
        {
            List<TermTranslation> translations = new List<TermTranslation>();
            Term term = Dictionary.GetTermById(termId);
            List<TreeNode> toCheck = new List<TreeNode>();
            for (int i = 0; i < AllLetters[lang].Count; i++)
            {
                if (term.ParentId > 0)
                {
                    for (int j = 0; j < AllLetters[lang][i].Children.Count; j++)
                    {
                        if (term.ParentId == AllLetters[lang][i].Children[j].Translation.TermId)
                            toCheck.Add(AllLetters[lang][i].Children[j]);
                    }
                }
                else
                {
                    toCheck.Add(AllLetters[lang][i]);
                }
            }

            for (int i = 0; i < toCheck.Count; i++)
            {
                for (int j = 0; j < toCheck[i].Children.Count; j++)
                {
                    if (termId == toCheck[i].Children[j].Translation.TermId)
                    {
                        toCheck[i].Children.RemoveAt(j);
                        j--;
                    }
                }
            }            
        }

        private void UpdateTranslations(int lang, TermVM newVM)
        {
            List<TermTranslation> translations = new List<TermTranslation>();
            foreach (var node in newVM.Languages[lang].Children)
            {
                if (!node.IsTranslation)
                    return;
                translations.Add(node.Translation);
            }
            translations.Sort((a, b) => { return string.Compare(a.CleanValue, b.CleanValue); });

            List<TreeNode> toCheck = new List<TreeNode>();
            for (int i = 0; i < AllLetters[lang].Count; i++)
            {
                if (newVM.SelectedParentTranslation != null)
                {
                    for (int j = 0; j < AllLetters[lang][i].Children.Count; j++)
                    {
                        if (newVM.SelectedParentTranslation.TermId == AllLetters[lang][i].Children[j].Translation.TermId)
                            toCheck.Add(AllLetters[lang][i].Children[j]);
                    }
                }
                else
                {
                    toCheck.Add(AllLetters[lang][i]);
                }
            }

            List<TreeNode> children = new List<TreeNode>();
            for (int i = 0; i < toCheck.Count; i++)
            {
                for (int j = 0; j < toCheck[i].Children.Count; j++)
                {
                    if (translations[0].TermId == toCheck[i].Children[j].Translation.TermId)
                    {
                        children.AddRange(toCheck[i].Children[j].Children);
                        toCheck[i].Children.RemoveAt(j);
                        j--;
                    }
                }
            }

            int transPos = 0;
            for (int i = 0; i < toCheck.Count; i++)
            {
                if (newVM.SelectedParentTranslation != null)
                {
                    ObservableCollection<TreeNode> chld = null;
                    if (chld == null)
                    {
                        chld = toCheck[i].Children;
                        for (int k = 0, j = 0; k < translations.Count && j < chld.Count;)
                        {
                            TreeNode tn = new TreeNode { Name = translations[k].Value, Parent = toCheck[i], Translation = translations[k] };
                            if (string.Compare(toCheck[i].Children[j].Name, translations[k].Value, true) >= 0)
                            {
                                chld.Insert(j, tn);
                                k++;
                                j++;
                                AddChildren(tn, children);
                            }
                            else
                            {
                                j++;
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < toCheck[i].Children.Count; j++)
                    {
                        for (int k = transPos; k < translations.Count; k++)
                        {
                            TreeNode tn = new TreeNode { Name = translations[k].Value, Parent = toCheck[i], Translation = translations[k] };
                            if (string.Compare(toCheck[i].Name, translations[k].Value.Substring(0, 1), true) == 0 &&
                                string.Compare(toCheck[i].Children[j].Name, translations[k].Value, true) >= 0)
                            {
                                toCheck[i].Children.Insert(j, tn);
                                transPos++;
                                j++;
                                AddChildren(tn, children);
                            }
                        }
                    }
                }
            }
        }

        void AddChildren(TreeNode parent, List<TreeNode> children)
        {
            parent.Children.Clear();
            foreach (TreeNode tn in children)
            {
                tn.Parent = parent;
                parent.Children.Add(tn);
            }
        }

        public TermVM GetSelectedTermVM()
        {
            //Term term = Dictionary.GetTermByWord(SelectedTranslation.Translation);
            SelectedTerm.SelectedParentTranslation = SelectedNodes[0].Parent.IsHeader ? null : SelectedNodes[0].Parent.Translation;
            SelectedTerm.LoadServiceData();
            return SelectedTerm;
        }

        public TermVM GetCopySelectedTermVM()
        {
            TermVM copy = SelectedTerm.MakeCopy();
            copy.SelectedParentTranslation = SelectedNodes[0].Parent.IsHeader ? null : SelectedNodes[0].Parent.Translation;
            copy.LoadServiceData();

            return copy;
        }

        public TermVM GetNewTermVM()
        {
            TermVM termVM = new TermVM
            {
                Dictionary = Dictionary
            };
            termVM.LoadTranslations(-1);
            termVM.LoadServiceData();

            return termVM;
        }

        private bool isSelectedChanged;
        public bool IsSelectedChanged
        {
            get { return isSelectedChanged; }
            set
            {
                isSelectedChanged = value;
                OnPropertyChanged("IsSelectedChanged");
            }
        }
        private void SelectedTransChanged(object parameter)
        {
            if (SelectedNodes.Count > 0)
            {
                SelectedNodes.RemoveAt(0);
            }
            if (parameter != null)
            {
                SelectedNodes.Add(parameter as TreeNode);
                IsSelectedChanged = SelectedNodes[0].IsTranslation;
            }
        }

        private void SelectedNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ((RelayCommand)DuplicateTerm).RaiseCanExecuteChanged();
            ((RelayCommand)EditTerm).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteTerm).RaiseCanExecuteChanged();
            ((RelayCommand)MergeTerms).RaiseCanExecuteChanged();
            if (SelectedNodes.Count == 1 && SelectedNodes[0].Translation != null)
            {
                SelectedTerm.LoadTranslations(SelectedNodes[0].Translation.TermId);
            }
        }

        private bool IsTermSelected(object parameter)
        {
            if (SelectedNodes.Count == 0)
                return false;
            return SelectedNodes.FirstOrDefault((x) => x?.Translation != null) != null;
            //return SelectedTranslation != null && SelectedTranslation.Translation != null;
        }

        private bool CanMerge(object parameter)
        {
            return SelectedNodes.Count == 2 && SelectedNodes[0].Translation != null && SelectedNodes[1].Translation != null;
        }

        private bool IsOneItemSelected(object parameter)
        {
            return SelectedNodes.Count == 1 && SelectedNodes[0].Translation != null;
        }
    }

    class SearchData
    {
        public int ChapterIndex { get; set; } = 0;
        public int TopTranslationIndex { get; set; } = 0;
        public int ChildTranslationIndex { get; set; } = 0;
        public bool IsParentFound { get; set; } = false;
    }

    public class TreeNode : TreeViewItemBase
    {
        public TreeNode Parent { get; set; }
        public TermTranslation Translation { get; set; }
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public ObservableCollection<TreeNode> Children { get; set; } = new ObservableCollection<TreeNode>();

        public bool IsHeader { get; set; } = false;
        public bool IsStub { get; set; } = false;
        public bool IsTranslation { get { return !IsHeader && !IsStub; } }

        bool shouldDelete = false;
        public bool ShouldDelete
        {
            get { return shouldDelete; }
            set
            {
                shouldDelete = value;
                NotifyPropertyChanged("ShouldDelete");
            }
        }
    }

    public class TreeViewItemBase : INotifyPropertyChanged
    {
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (value != isExpanded)
                {
                    this.isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private bool isSelected;
        private bool isExpanded;
    }
}
