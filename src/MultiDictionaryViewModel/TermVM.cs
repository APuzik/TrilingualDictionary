using MultiDictionaryCore.Core.Interfaces;
using MultiDictionaryCore.DBEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryViewModel
{
    public class TermVM : INotifyPropertyChanged
    {

        public IMultiLingualDictionary Dictionary { get; set; }
        public ObservableCollection<TreeNode> Languages { get; set; } = new ObservableCollection<TreeNode>
                                                                            {
                                                                                new TreeNode{ Name = "Русский", IsHeader = true },
                                                                                new TreeNode{ Name = "Английский", IsHeader = true },
                                                                                new TreeNode{ Name = "Украинский", IsHeader = true }
                                                                            };
        public ObservableCollection<TreeNode> SelectedNodes { get; set; } = new ObservableCollection<TreeNode>();

        string topic = "Test topic";

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public string Topic
        {
            get { return topic; }
            set
            {
                topic = value;
                OnPropertyChanged("Topic");
            }
        }
        string semantic = "Test semantic";
        public string Semantic
        {
            get { return semantic; }
            set
            {
                semantic = value;
                OnPropertyChanged("Semantic");
            }
        }

        public void LoadTranslations(int termId)
        {
            if (termId != 0)
            {
                List<TermTranslation> rusTranslations = Dictionary.GetTranslationsForTerm(termId);
                //List<TermTranslation> engTranslations = Dictionary.GetTranslationsForTerm(2, termId);
                //List<TermTranslation> ukrTranslations = Dictionary.GetTranslationsForTerm(3, termId);

                GetNodes(rusTranslations, Languages);

                Semantic = Dictionary.GetTermSemantic(termId);
                Topic = Dictionary.GetTermTopic(termId);
            }
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

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Children.Count == 0)
                {
                    TreeNode tn = new TreeNode { Name = "Отсутствует" };
                    nodes[i].Children.Add(tn);
                }
            }

            return nodes;
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
                        Translation = new TermTranslation
                        {
                            ChangeablePart = tn.Translation.ChangeablePart,
                            ChangeableType = tn.Translation.ChangeableType,
                            Id = 0,
                            LanguageId = tn.Translation.LanguageId,
                            PartOfSpeech = tn.Translation.PartOfSpeech,
                            TermId = 0,
                            Value = tn.Translation.Value
                        }
                    };
                    nodeCopy.Children.Add(tnCopy);
                }
            }

            return copy;
        }
    }
}
