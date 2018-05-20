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

namespace MultiDictionaryViewModel
{
    public class MultiDictionaryVM
    {
        public ObservableCollection<string> Languages { get; set; } = new ObservableCollection<string> { "Русский", "English", "Українська" };
        public ObservableCollection<TreeNode> Letters { get; set; } = new ObservableCollection<TreeNode>();
        public Dictionary<int, ObservableCollection<TreeNode>> AllLetters { get; set; } = new Dictionary<int, ObservableCollection<TreeNode>>();
        //{
        //    new TreeNode { Name = "A", Children = new ObservableCollection<TreeNode>
        //                                {
        //                                    new TreeNode { Name = "A1", Children = new ObservableCollection<TreeNode>{ new TreeNode { Name = "A11" } } }
        //                                }
        //                 },
        //    new TreeNode { Name = "B" },
        //    new TreeNode { Name = "C" },
        //    new TreeNode { Name = "D" }
        //};

        public IMultiLingualDictionary Dictionary { get; set; } = new MultiLingualDictionary();

        public ICommand ExpandCommand { get; set; }
        public ICommand SwitchLanguage { get; set; }

        public int SelectedLanguage { get { return SelectedItem + 1; } }
        public int SelectedItem { get; set; } = 0;
        public int ChangeLang
        {
            set
            {
                LoadTranslations();
            }
        }

        public MultiDictionaryVM()
        {
            SwitchLanguage = new RelayCommand { ExecuteAction = LoadTranslations };
            LoadTranslations();
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
                foreach (TermTranslation tt in node.Value)
                {
                    //List<TermTranslation> ttChilds = Dictionary.GetChildrenTranslations(SelectedLanguage, tt.TermId);
                    //var children2 = new ObservableCollection<TreeNode>();
                    //foreach (TermTranslation tt2 in ttChilds)
                    //{
                    //    children2.Add(new TreeNode { Name = tt2.Value });
                    //}
                    TreeNode tn = new TreeNode { Name = tt.Value };
                    if (!treeNodes.ContainsKey(tt.TermId))
                    {
                        treeNodes.Add(tt.TermId, new List<TreeNode>());
                    }
                    treeNodes[tt.TermId].Add(tn);
                    children.Add(tn);
                }

                TreeNode item = new TreeNode { Translation = null, Name = node.Key.ToString(), Children = children };
                Letters.Add(item);
            }

            foreach (TermTranslation tt in childTranslations)
            {
                int nodeId = termsMapping[tt.TermId];
                TreeNode tn = new TreeNode { Name = tt.Value };
                if (treeNodes.ContainsKey(nodeId))
                {
                    foreach (TreeNode tnParent in treeNodes[nodeId])
                    {
                        tnParent.Children.Add(tn);
                    }
                }
            }

            if(!AllLetters.ContainsKey(SelectedItem))
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

    }

    public class TreeNode
    {
        public TermTranslation Translation { get; set; }
        public string Name { get; set; }
        public ObservableCollection<TreeNode> Children { get; set; } = new ObservableCollection<TreeNode>();
    }

}
