using MultiDictionaryCore.Core.Interfaces;
using MultiDictionaryCore.DBEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryViewModel
{
    public class TermVM
    {

        public IMultiLingualDictionary Dictionary { get; set; }
        public ObservableCollection<TreeNode> Languages { get; set; } = new ObservableCollection<TreeNode>
                                                                            {
                                                                                new TreeNode{ Name = "Русский", IsHeader = true },
                                                                                new TreeNode{ Name = "Английский", IsHeader = true },
                                                                                new TreeNode{ Name = "Украинский", IsHeader = true }
                                                                            };
        public ObservableCollection<TreeNode> SelectedNodes { get; set; } = new ObservableCollection<TreeNode>();

        public void LoadTranslations(int termId)
        {
            if (termId != 0)
            {
                List<TermTranslation> rusTranslations = Dictionary.GetTranslationsForTerm(termId);
                //List<TermTranslation> engTranslations = Dictionary.GetTranslationsForTerm(2, termId);
                //List<TermTranslation> ukrTranslations = Dictionary.GetTranslationsForTerm(3, termId);

                GetNodes(rusTranslations, Languages);
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
                if(nodes[i].Children.Count == 0)
                {
                    TreeNode tn = new TreeNode { Name = "Отсутствует" };
                    nodes[i].Children.Add(tn);
                }
            }

            return nodes;
        }
    }
}
