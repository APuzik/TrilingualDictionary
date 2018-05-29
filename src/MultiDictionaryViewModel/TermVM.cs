using MultiDictionaryCore.Core.Interfaces;
using MultiDictionaryCore.DBEntities;
using MultiDictionaryViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MultiDictionaryViewModel
{
    public class TermVM : INotifyPropertyChanged
    {
        public TermVM()
        {

            SaveTerm = new RelayCommand { ExecuteAction = UpdateTerm };
            AddTerm = new RelayCommand { ExecuteAction = AddNewTerm };
            AddTranslation = new RelayCommand { ExecuteAction = AddNewTranslation };
            EditTranslation = new RelayCommand { ExecuteAction = EdTranslation };
            DeleteTranslation = new RelayCommand { ExecuteAction = DelTranslation };
            SelectedTranslationChanged = new RelayCommand { ExecuteAction = SelectedTransChanged };
            SelectedTranslation = Languages[0];
        }

        Dictionary<int, TermTranslation> translationsToDelete = new Dictionary<int, TermTranslation>();
        private void DelTranslation(object parameter)
        {
            if (!translationsToDelete.ContainsKey(SelectedTranslation.Translation.Id))
            {
                translationsToDelete.Add(SelectedTranslation.Translation.Id, SelectedTranslation.Translation);
            }
            Languages[SelectedLanguage].Children.Remove(SelectedTranslation);
            if (Languages[SelectedLanguage].Children.Count == 0)
            {
                Languages[SelectedLanguage].Children.Add(CreateStubNode(Languages[SelectedLanguage]));
            }
            SelectedTranslation = Languages[SelectedLanguage];
        }

        private void EdTranslation(object parameter)
        {
            SelectedTranslation.Translation.Value = SelectedValue;
            SelectedTranslation.Name = SelectedTranslation.Translation.Value;
        }

        private void AddNewTranslation(object parameter)
        {
            TermTranslation newTT = new TermTranslation
            {
                ChangeablePart = SelectedTranslation.Translation.ChangeablePart,
                ChangeableType = SelectedTranslation.Translation.ChangeableType,
                LanguageId = SelectedLanguage,
                PartOfSpeech = SelectedTranslation.Translation.PartOfSpeech,
                TermId = SelectedTranslation.Translation.TermId,
                Value = SelectedValue
            };
            TreeNode tn = new TreeNode { Name = newTT.Value, Parent = Languages[newTT.LanguageId], Translation = newTT };
            if (Languages[newTT.LanguageId].Children.Count == 1 && Languages[newTT.LanguageId].Children[0].IsStub)
            {
                Languages[newTT.LanguageId].Children.RemoveAt(0);
            }
            for (int i = 0; i < Languages[newTT.LanguageId].Children.Count; i++)
            {
                if (string.Compare(tn.Name, Languages[newTT.LanguageId].Children[i].Name) < 0)
                {
                    Languages[newTT.LanguageId].Children.Insert(i, tn);
                    return;
                }
            }
            Languages[newTT.LanguageId].Children.Add(tn);
            Languages[newTT.LanguageId].Translation = new TermTranslation();
        }

        string selectedValue;
        public string SelectedValue
        {
            get { return selectedValue; }
            set
            {
                selectedValue = value;
                OnPropertyChanged("SelectedValue");
                OnPropertyChanged("IsTranslationSelected");
            }
        }
        public void ResetParentTranslation()
        {
            selectedParentTranslation = null;
        }
        string parentTermTranslation = "";
        public string ParentTermTranslation
        {
            get { return parentTermTranslation; }
            set
            {
                parentTermTranslation = value;
                if (value.Length < 3)
                {
                    selectedParentTranslation = null;
                }
                if (selectedParentTranslation == null)
                {
                    ParentTerms.Clear();
                    if (!string.IsNullOrEmpty(parentTermTranslation) && parentTermTranslation.Length > 2)
                    {
                        List<TermTranslation> potential = Dictionary.GetTopTranslations(1, parentTermTranslation);
                        foreach (TermTranslation tt in potential)
                        {
                            ParentTerms.Add(tt);
                        }
                    }
                    OnPropertyChanged("ParentTermTranslation");
                    OnPropertyChanged("ParentTerms");
                    IsDropDownOpened = true;
                    OnPropertyChanged("IsDropDownOpened");
                }
            }
        }

        TermTranslation selectedParentTranslation;
        public TermTranslation SelectedParentTranslation
        {
            get { return selectedParentTranslation; }
            set
            {
                selectedParentTranslation = value;

                if (selectedParentTranslation != null)
                {
                    parentTermTranslation = selectedParentTranslation.Value;
                    //                    OnPropertyChanged("ParentTermTranslation");
                    //IsDropDownOpened = false;
                    //OnPropertyChanged("IsDropDownOpened");
                }
                //OnPropertyChanged("SelectedParentTranslation");
            }
        }
        public bool IsDropDownOpened { get; set; }

        public ObservableCollection<TermTranslation> ParentTerms { get; set; } = new ObservableCollection<TermTranslation>();

        int TermId { get; set; }
        int selectedLanguage = 0;
        public int SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                selectedLanguage = value;
                OnPropertyChanged("SelectedLanguage");
            }
        }

        private void SelectedTransChanged(object parameter)
        {
            SelectedTranslation = parameter as TreeNode;
            SelectedValue = SelectedTranslation?.Translation?.Value;
            TreeNode curLang = SelectedTranslation;
            if (!SelectedTranslation.IsHeader)
            {
                curLang = curLang.Parent;
            }

            for (int i = 0; i < Languages.Count; i++)
            {
                if (string.Equals(curLang.Name, Languages[i].Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    SelectedLanguage = i;
                    break;
                }
            }
        }

        public bool IsTranslationSelected
        {
            get { return !string.IsNullOrWhiteSpace(SelectedValue) && SelectedTranslation != null && SelectedTranslation.IsTranslation; }
        }

        public ICommand SelectedTranslationChanged { get; set; }
        private void UpdateTerm(object wnd)
        {
            bool shouldSave = false;
            foreach (TreeNode node in Languages)
            {
                foreach (TreeNode child in node.Children)
                {
                    if (!child.IsTranslation)
                        continue;
                    shouldSave = true;
                }
            }
            if (shouldSave)
            {
                Term term = Dictionary.GetTermById(TermId);
                term.SemanticId = Semantic == null ? 0 : Semantic.Id;
                term.TopicId = Topic == null ? 0 : Topic.Id;

                Dictionary.UpdateTerm(term);
                foreach (TreeNode node in Languages)
                {
                    foreach (TreeNode child in node.Children)
                    {
                        if (!child.IsTranslation)
                            continue;

                        if (child.Translation.Id > 0)
                        {
                            Dictionary.UpdateTranslation(child.Translation);
                        }
                        else
                        {
                            child.Translation.TermId = TermId;
                            Dictionary.AddTranslation(child.Translation);
                        }
                    }
                }

                foreach (KeyValuePair<int, TermTranslation> key in translationsToDelete)
                {
                    Dictionary.DeleteTranslation(key.Key);
                }

                ((System.Windows.Window)wnd).DialogResult = true;
            }
            ((System.Windows.Window)wnd).Close();
        }

        private void AddNewTerm(object wnd)
        {
            bool shouldSave = false;
            foreach (TreeNode node in Languages)
            {
                foreach (TreeNode child in node.Children)
                {
                    if (!child.IsTranslation)
                        continue;
                    shouldSave = true;
                }
            }
            if (shouldSave)
            {
                Term term = new Term
                {
                    ParentId = SelectedParentTranslation == null ? 0 : SelectedParentTranslation.TermId,
                    SemanticId = Semantic == null ? 0 : Semantic.Id,
                    TopicId = Topic == null ? 0 : Topic.Id
                };
                term = Dictionary.AddTerm(term);

                foreach (TreeNode node in Languages)
                {
                    foreach (TreeNode child in node.Children)
                    {
                        if (!child.IsTranslation)
                            continue;

                        child.Translation.TermId = term.Id;
                        if (child.Translation.Id > 0)
                        {
                            Dictionary.UpdateTranslation(child.Translation);
                        }
                        else
                        {
                            Dictionary.AddTranslation(child.Translation);
                        }
                    }
                }
                ((System.Windows.Window)wnd).DialogResult = true;
            }

            ((System.Windows.Window)wnd).Close();
        }

        public IMultiLingualDictionary Dictionary { get; set; }
        public ObservableCollection<TreeNode> Languages { get; set; } = new ObservableCollection<TreeNode>
                                                                            {
                                                                                new TreeNode{ Name = "Русский", IsHeader = true, Translation = new TermTranslation() },
                                                                                new TreeNode{ Name = "Английский", IsHeader = true, Translation = new TermTranslation() },
                                                                                new TreeNode{ Name = "Украинский", IsHeader = true, Translation = new TermTranslation() }
                                                                            };
        public ObservableCollection<TreeNode> SelectedNodes { get; set; } = new ObservableCollection<TreeNode>();

        ObservableCollection<TopicTranslation> activeTopics;
        public ObservableCollection<TopicTranslation> ActiveTopics
        {
            get { return activeTopics; }
            set
            {
                activeTopics = value;
                OnPropertyChanged("ActiveTopics");
            }
        }

        ObservableCollection<SemanticTranslation> activeSemantics;
        public ObservableCollection<SemanticTranslation> ActiveSemantics
        {
            get { return activeSemantics; }
            set
            {
                activeSemantics = value;
                OnPropertyChanged("ActiveSemantics");
            }
        }

        ObservableCollection<ChangeableTranslation> activeChangeables;
        public ObservableCollection<ChangeableTranslation> ActiveChangeables
        {
            get { return activeChangeables; }
            set
            {
                activeChangeables = value;
                OnPropertyChanged("ActiveChangeables");
            }
        }

        ObservableCollection<PartOfSpeechTranslation> activeLangParts;
        public ObservableCollection<PartOfSpeechTranslation> ActiveLangParts
        {
            get { return activeLangParts; }
            set
            {
                activeLangParts = value;
                OnPropertyChanged("ActiveLangParts");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        TopicTranslation topic;
        public TopicTranslation Topic
        {
            get { return topic; }
            set
            {
                topic = value;
                OnPropertyChanged("Topic");
            }
        }

        SemanticTranslation semantic;
        public SemanticTranslation Semantic
        {
            get { return semantic; }
            set
            {
                semantic = value;
                OnPropertyChanged("Semantic");
            }
        }


        TreeNode selectedTranslation;
        public TreeNode SelectedTranslation
        {
            get { return selectedTranslation; }
            set
            {
                selectedTranslation = value;
                OnPropertyChanged("SelectedTranslation");
            }
        }

        public ICommand AddTerm { get; set; }
        public ICommand SaveTerm { get; set; }
        public ICommand AddTranslation { get; set; }
        public ICommand EditTranslation { get; set; }
        public ICommand DeleteTranslation { get; set; }

        public void LoadTranslations(int termId)
        {
            if (termId > 0)
            {
                List<TermTranslation> allTranslations = Dictionary.GetTranslationsForTerm(termId);

                GetNodes(allTranslations, Languages);
                TermId = termId;
                Semantic = Dictionary.GetTermSemantic(termId);
                Topic = Dictionary.GetTermTopic(termId);
            }
            else
            {
                FillAbsent(Languages);
                Semantic = null;
                Topic = null;
            }
        }

        public void LoadServiceData()
        {
            int langId = 0;
            List<TopicTranslation> rusTopics = Dictionary.GetTopics(langId);
            ActiveTopics = new ObservableCollection<TopicTranslation>(rusTopics);
            List<SemanticTranslation> rusSemantics = Dictionary.GetSemantics(langId);
            ActiveSemantics = new ObservableCollection<SemanticTranslation>(rusSemantics);
            List<ChangeableTranslation> rusChangeables = Dictionary.GetChangeables(langId);
            ActiveChangeables = new ObservableCollection<ChangeableTranslation>(rusChangeables);
            List<PartOfSpeechTranslation> rusLangParts = Dictionary.GetLangParts(langId);
            ActiveLangParts = new ObservableCollection<PartOfSpeechTranslation>(rusLangParts);


        }

        public void SetServiceDataForTerm(int termId)
        {

        }

        private ObservableCollection<TreeNode> GetNodes(List<TermTranslation> allTranslations, ObservableCollection<TreeNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Children.Clear();
            }
            foreach (TermTranslation tt in allTranslations)
            {
                TreeNode tn = new TreeNode { Name = tt.Value, Translation = tt, Parent = nodes[tt.LanguageId] };
                nodes[tt.LanguageId].Children.Add(tn);
            }

            FillAbsent(nodes);

            return nodes;
        }

        private void FillAbsent(ObservableCollection<TreeNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Children.Count == 0)
                {
                    TreeNode tn = CreateStubNode(nodes[i]);
                    nodes[i].Children.Add(tn);
                }
            }
        }

        private TreeNode CreateStubNode(TreeNode parentNode)
        {
            return new TreeNode { Name = "Отсутствует", Parent = parentNode, IsStub = true, Translation = new TermTranslation() };
        }

        public TermVM MakeCopy()
        {
            TermVM copy = new TermVM
            {
                Dictionary = Dictionary,
                Topic = Topic,
                Semantic = Semantic,
            };
            for (int i = 0; i < Languages.Count; i++)
            {
                TreeNode lang = Languages[i];
                TreeNode nodeCopy = copy.Languages[i];
                nodeCopy.IsExpanded = lang.IsExpanded;
                nodeCopy.IsHeader = lang.IsHeader;
                nodeCopy.IsSelected = lang.IsSelected;
                nodeCopy.Name = lang.Name;
                nodeCopy.Parent = lang.Parent;
                nodeCopy.IsStub = lang.IsStub;

                foreach (TreeNode tn in lang.Children)
                {
                    TreeNode tnCopy = new TreeNode
                    {
                        IsExpanded = tn.IsExpanded,
                        IsHeader = tn.IsHeader,
                        IsSelected = tn.IsSelected,
                        IsStub = tn.IsStub,
                        Name = tn.Name,
                        Parent = tn.Parent,
                        Translation = tn.Translation != null ? new TermTranslation
                        {
                            ChangeablePart = tn.Translation.ChangeablePart,
                            ChangeableType = tn.Translation.ChangeableType,
                            Id = 0,
                            LanguageId = tn.Translation.LanguageId,
                            PartOfSpeech = tn.Translation.PartOfSpeech,
                            TermId = 0,
                            Value = tn.Translation.Value
                        } : null
                    };
                    nodeCopy.Children.Add(tnCopy);
                }
            }

            return copy;
        }
    }
}
