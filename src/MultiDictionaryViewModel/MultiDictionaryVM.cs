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
        public ObservableCollection<TreeNode> Letters { get; set; } = new ObservableCollection<TreeNode>();
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

            LoadTranslations();
            SelectedTerm.Dictionary = Dictionary;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void LoadTranslations()
        {
            Letters.Clear();
            //List<TermTranslation> translations = Dictionary.GetAllTranslations(SelectedLanguage);
            List<Term> terms = Dictionary.GetTopTerms(SelectedLanguage);
            SortedDictionary<int, Term> dicTerms = new SortedDictionary<int, Term>();
            foreach (Term term in terms)
            {
                dicTerms.Add(term.Id, term);
            }
            List<TermTranslation> translations = Dictionary.GetTopTranslations(SelectedLanguage);
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

            if (!AllLetters.ContainsKey(SelectedItem))
            {
                AllLetters.Add(SelectedItem, Letters);
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
            LoadTranslations();
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
            MessageBoxResult confirmResult = MessageBox.Show("Вы уверены, что хотите удалить выбранный термин целиком?", "Удаление термина", MessageBoxButton.YesNo);
            if (confirmResult == MessageBoxResult.Yes)
            {
                //...
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
                        string clear = parent.Name.Replace("#", "");
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
                        string clear = child.Name.Replace("#", "");
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

        private void SelectedTransChanged(object parameter)
        {
            if (SelectedNodes.Count > 0)
            {
                SelectedNodes.RemoveAt(0);
            }
            SelectedNodes.Add(parameter as TreeNode);
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
            return SelectedNodes.FirstOrDefault((x) => x.Translation != null) != null;
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
        public string Name { get; set; }
        public ObservableCollection<TreeNode> Children { get; set; } = new ObservableCollection<TreeNode>();

        public bool IsHeader { get; set; } = false;
        public bool IsTranslation { get { return !IsHeader && Translation != null; } }
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
