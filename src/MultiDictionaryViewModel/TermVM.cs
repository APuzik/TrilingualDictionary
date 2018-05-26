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
            SelectedTranslationChanged = new RelayCommand { ExecuteAction = SelectedTransChanged };
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

        string parentTermTranslation;
        string ParentTermTranslation
        {
            get { return parentTermTranslation; }
            set
            {
                parentTermTranslation = value;
                OnPropertyChanged("ParentTermTranslation");
            }
        }

        int paremtTermId;

        private void SelectedTransChanged(object parameter)
        {
            SelectedTranslation = parameter as TreeNode;
            SelectedValue = SelectedTranslation?.Translation?.Value;                       
        }

        public bool IsTranslationSelected
        {
            get { return !string.IsNullOrWhiteSpace(SelectedValue) && SelectedTranslation != null && SelectedTranslation.IsTranslation; }
        }

        public ICommand SelectedTranslationChanged { get; set; }
        private void UpdateTerm(object parameter)
        {
            
            //Dictionary.AddTranslation
        }

        private void AddNewTerm(object parameter)
        {
            //Dictionary.Get

            
            //Dictionary.AddTerm(term);
        }

        public IMultiLingualDictionary Dictionary { get; set; }
        public ObservableCollection<TreeNode> Languages { get; set; } = new ObservableCollection<TreeNode>
                                                                            {
                                                                                new TreeNode{ Name = "Русский", IsHeader = true },
                                                                                new TreeNode{ Name = "Английский", IsHeader = true },
                                                                                new TreeNode{ Name = "Украинский", IsHeader = true }
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

        public void LoadTranslations(int termId)
        {
            if (termId > 0)
            {
                List<TermTranslation> rusTranslations = Dictionary.GetTranslationsForTerm(termId);
                //List<TermTranslation> engTranslations = Dictionary.GetTranslationsForTerm(2, termId);
                //List<TermTranslation> ukrTranslations = Dictionary.GetTranslationsForTerm(3, termId);

                GetNodes(rusTranslations, Languages);

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

        private ObservableCollection<TreeNode> GetNodes(List<TermTranslation> rusTranslations, ObservableCollection<TreeNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Children.Clear();
            }
            foreach (TermTranslation tt in rusTranslations)
            {
                TreeNode tn = new TreeNode { Name = tt.Value, Translation = tt };
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
                    TreeNode tn = new TreeNode { Name = "Отсутствует" };
                    nodes[i].Children.Add(tn);
                }
            }
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

                foreach (TreeNode tn in lang.Children)
                {
                    TreeNode tnCopy = new TreeNode
                    {
                        IsExpanded = tn.IsExpanded,
                        IsHeader = tn.IsHeader,
                        IsSelected = tn.IsSelected,
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
                                                                } :
                                                                null
                    };
                    nodeCopy.Children.Add(tnCopy);
                }
            }

            return copy;
        }
    }
}
