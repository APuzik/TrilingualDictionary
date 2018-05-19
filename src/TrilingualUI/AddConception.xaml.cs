using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrilingualDictionaryCore;
using TrilingualDictionaryViewModel;

namespace TrilingualUI
{
    /// <summary>
    /// Interaction logic for AddConception.xaml
    /// </summary>
    public partial class AddConception : Window
    {
        public Conception m_NewConception = null;
        public ConceptionVM m_NewConceptionVM = null;
        TrilingualDictionaryViewModel.TrilingualDictionaryViewModel m_DictionaryViewModel = null;
        int m_PrevTopic = 0;
        int m_PrevSemantic = 0;
        int m_PrevLangPart = 0;
        int m_PrevChangeable = 0;

        private void SwitchDataContext()
        {
            DataContext = new
            {
                //ActiveLanguage = m_DictionaryViewModel.Chapters,
                ActiveConception = m_NewConceptionVM == null ? null : m_NewConceptionVM.Descriptions,
                ActiveTopics = m_DictionaryViewModel.ActiveTopics,
                ActiveSemantics = m_DictionaryViewModel.ActiveSemantics,
                ActiveLangParts = m_DictionaryViewModel.ActivePartsOfSpeech,
                ActiveChangeables = m_DictionaryViewModel.ActiveChangeables,
                AllParents = m_DictionaryViewModel.AllParents
            };
        }

        public AddConception(TrilingualDictionaryViewModel.TrilingualDictionaryViewModel dictionary)
        {
            InitializeComponent();
            m_DictionaryViewModel = dictionary;
            btnAddDescription.IsEnabled = true;
            SwitchDataContext();
        }

        private void btnAddDescription_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEditDescription.Text))
            {
                MessageBox.Show("Введите текст переводного эквивалента");
                txtEditDescription.Focus();
                return;
            }

            txtEditDescription.Text = txtEditDescription.Text.Trim();

            if (m_NewConception == null)
            {
                int topicId = cmbTopic.SelectedIndex <= 0  ? 0 : ((TranslationVM)cmbTopic.SelectedItem).Item.ServConceptionId;
                int semanticId = cmbSemantic.SelectedIndex <= 0 ? 0 : ((TranslationVM)cmbSemantic.SelectedItem).Item.ServConceptionId;
                int parentId = cmbParents.SelectedIndex <= 0 ? 0 : ((ConceptionDescriptionViewModel)cmbParents.SelectedItem).Conception.ConceptionId;

                m_NewConception = new Conception(0, parentId, topicId, semanticId);//, 0, 0);  
                //m_NewConception.SaveConception();
            }

            TranslationVM chType = cmbChangableType.SelectedItem as TranslationVM;
            TranslationVM langPart = cmbLangPart.SelectedItem as TranslationVM;

            m_NewConception.AddDescription(txtEditDescription.Text, (LanguageId)cmbChangeforEdit.SelectedIndex,
                    (chType == null || chType.Item == null) ? 0 : chType.Item.ServConceptionId,//"" : chType.Translation,
                    txtChangable.Text,
                    (langPart == null || langPart.Item == null) ? 0 : langPart.Item.ServConceptionId,//Translation,
                    false);
            m_NewConceptionVM = new ConceptionVM(m_NewConception);
            //int id = m_DictionaryViewModel.AddConception(txtEditDescription.Text, (LanguageId)cmbChangeforEdit.SelectedIndex);
            //m_NewConception = m_DictionaryViewModel.GetConception(id);
            //m_NewConception.ParentId = 3;
            //m_NewConception.ParentConception = m_DictionaryViewModel.GetConception(m_NewConception.ParentId);
            btnChangeDescription.IsEnabled = true;
            btnRemoveDescription.IsEnabled = true;
            SwitchDataContext();
        }

        private void btnChangeDescription_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEditDescription.Text))
            {
                MessageBox.Show("Введите текст переводного эквивалента");
                txtEditDescription.Focus();
                return;
            }

            txtEditDescription.Text = txtEditDescription.Text.Trim();

            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;

            ConceptionDescriptionViewModel desc = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
            if (desc != null)
            {
                TranslationVM chType = cmbChangableType.SelectedItem as TranslationVM;
                TranslationVM langPart = cmbLangPart.SelectedItem as TranslationVM;

                m_NewConception.ChangeDescription(txtEditDescription.Text, (LanguageId)cmbChangeforEdit.SelectedIndex, desc.ConceptionRegistryDescription,
                    (chType == null || chType.Item == null) ? 0 : chType.Item.ServConceptionId,//"" : chType.Translation,
                    txtChangable.Text,
                    (langPart == null || langPart.Item == null) ? 0 : langPart.Item.ServConceptionId,//Translation, 
                    false);
                //            ConceptionDescription desc = new ConceptionDescription(.Conception, "");// conception.GetConceptionDescription(languageForEdit);
                //desc..LangPart = cmbLangPart.Text;
                ////desc.Topic = txtTopic.Text;
                ////desc.Semantic = txtSemantic.Text;
                //desc.Changeable.Type = cmbChangableType.Text;
                //desc.Changeable.Value = txtChangable.Text;
                //desc.Link = txtLink.Text;

                m_NewConceptionVM = new ConceptionVM(m_NewConception);
            }
            SwitchDataContext();
        }

        private void btnRemoveDescription_Click(object sender, RoutedEventArgs e)
        {
            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;

            ConceptionDescriptionViewModel desc = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
            if (desc != null)
            {
                m_NewConception.RemoveDescription(languageForEdit, desc.ConceptionRegistryDescription, false);
                m_NewConceptionVM = new ConceptionVM(m_NewConception);
            }
            SwitchDataContext();
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            if (m_NewConception != null)
                m_NewConception.SaveConception();

            DialogResult = true;
            Close();
        }

        private void ChkHumanHandled_OnClick(object sender, RoutedEventArgs e)
        {
            if (m_NewConception != null)
                m_NewConception.IsHumanHandled = (bool)chkHumanHandled.IsChecked;
        }

        private void cmbChangeforEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trvDescriptions == null)
                return;

            TreeViewItem it = (TreeViewItem)(trvDescriptions.ItemContainerGenerator.ContainerFromIndex(cmbChangeforEdit.SelectedIndex));
            if (it != null)
            {
                foreach (ConceptionDescriptionViewModel it1 in it.Items)
                {
                    if (it1.IsSelected)
                        return;
                }
                it.IsSelected = true;
            }
            int i = 1;
        }

        private void CmbSelectedLangPart(object sender, RoutedEventArgs e)
        {
            bool shouldReload = SelectTranslationComboAction(cmbLangPart, m_DictionaryViewModel.ActivePartsOfSpeech, new LangPartTranslationFactory(), ref m_PrevLangPart);
            if (shouldReload)
            {
                m_DictionaryViewModel.ReloadPartOfSpeech();
                SwitchDataContext(); 
            }
        }

        private void CmbSelectedTopic(object sender, RoutedEventArgs e)
        {
            bool shouldReload = SelectTranslationComboAction(cmbTopic, m_DictionaryViewModel.ActiveTopics, new TopicTranslationFactory(), ref m_PrevTopic);
            if (shouldReload)
            {
                m_DictionaryViewModel.ReloadTopics();
                SwitchDataContext(); 
            }
        }

        private void CmbSelectedSemantic(object sender, RoutedEventArgs e)
        {
            bool shouldReload = SelectTranslationComboAction(cmbSemantic, m_DictionaryViewModel.ActiveSemantics, new SemanticTranslationFactory(), ref m_PrevSemantic);
            if (shouldReload)
            {
                m_DictionaryViewModel.ReloadSemantics();
                SwitchDataContext(); 
            }
        }

        private void CmbSelectedChangeable(object sender, RoutedEventArgs e)
        {
            bool shouldReload = SelectTranslationComboAction(cmbChangableType, m_DictionaryViewModel.ActiveChangeables, new ChangeableTranslationFactory(), ref m_PrevChangeable);
            if (shouldReload)
            {
                m_DictionaryViewModel.ReloadChangeables();
                SwitchDataContext(); 
            }
        }

        private bool SelectTranslationComboAction(ComboBox combo, ObservableCollection<TranslationVM> collection, ITranslationFactory factory, ref int oldSelectedIndex)
        {
            if (combo.SelectedIndex == combo.Items.Count - 1)
            {
                OpenChangeTranlationsWindow(collection, factory);

                combo.SelectedIndex = oldSelectedIndex;

                return true;
            }
            else
            {
                if (combo.SelectedIndex != -1)
                    oldSelectedIndex = combo.SelectedIndex;

                return false;
            }
        }

        private void OpenChangeTranlationsWindow(ObservableCollection<TranslationVM> observableCollection, ITranslationFactory factory)
        {
            ChangeTranslatableWnd wnd = new ChangeTranslatableWnd(observableCollection, factory, m_DictionaryViewModel);
            wnd.ShowDialog();

            this.Activate();
        }

        private void UpdateEditDescription()
        {
            
            //ConceptionDescriptionViewModel conceptionDescription = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            //if (conceptionDescription == null)
            //{
            //    if (btnRemoveConception != null)
            //        btnRemoveConception.IsEnabled = false;
            //    return;
            //}

            if (m_NewConception == null)
            {
                cmbChangableType.SelectedIndex = 0;
                txtChangable.Text = "";
                cmbLangPart.SelectedIndex = 0;
                return;
            }
            
            ConceptionDescriptionViewModel conceptionDescription = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
            if (conceptionDescription == null)
            {
                cmbChangableType.SelectedIndex = 0;
                txtChangable.Text = "";
                cmbLangPart.SelectedIndex = 0;
                return;
            }

            Conception conception = m_NewConception;
            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;
            string activeDesc = GetSelectedConceptionDescription(trvDescriptions.SelectedItem);
            if (string.Compare(activeDesc, "Отсутствует", true) == 0)
                txtEditDescription.Text = "";
            else
                txtEditDescription.Text = activeDesc;

            ConceptionDescription description = conceptionDescription.ConceptionDescription;
            bool bFound = false;
            foreach (TranslationVM tr in cmbChangableType.ItemsSource)
            {
                if (tr.Item != null && description.ChangeableDB.Type == tr.Item.Id)
                {
                    cmbChangableType.SelectedItem = tr;
                    bFound = true;
                    break;
                }
            }
            if (!bFound)
                cmbChangableType.SelectedIndex = 0;

            //cmbChangableType.Text = description.Changeable.Type;
            txtChangable.Text = description.ChangeableDB.Value;

            cmbTopic.Text = conception.Topic;
            cmbSemantic.Text = conception.Semantic;
            
            bFound = false;
            foreach (TranslationVM tr in cmbLangPart.ItemsSource)
            {
                if (tr.Item != null && description.LangPartId == tr.Item.Id)
                {
                    bFound = true;
                    cmbLangPart.SelectedItem = tr;
                    break;
                }
            }
            if (!bFound)
                cmbLangPart.SelectedIndex = 0;
            //cmbLangPart.Text = description.LangPart;
            txtLink.Text = conception.Link;


            bool isDescriptionExists = !string.IsNullOrWhiteSpace(txtEditDescription.Text);

            btnAddDescription.IsEnabled = true;// !isDescriptionExists;
            btnChangeDescription.IsEnabled = isDescriptionExists;
            btnRemoveDescription.IsEnabled = isDescriptionExists;            

            LanguageId langNeeded = GetSelectedDescriptionLanguage(trvDescriptions.SelectedItem);
            if (langNeeded != LanguageId.Undefined)
                cmbChangeforEdit.SelectedIndex = (int)langNeeded;
        }

        private LanguageId GetSelectedDescriptionLanguage(object item)
        {
            if (item == null)
                return LanguageId.Undefined;

            ConceptionLanguageVM desc = item as ConceptionLanguageVM;
            if (desc != null)
            {
                return desc.Language;
            }

            ConceptionDescriptionViewModel desc1 = item as ConceptionDescriptionViewModel;
            int j = 0;
            if (desc1 != null)
            {
                TreeViewItem it = null;

                while (it == null && j < trvDescriptions.Items.Count)
                {
                    TreeViewItem it1 = (TreeViewItem)trvDescriptions.ItemContainerGenerator.ContainerFromItem(trvDescriptions.Items[j]);
                    it = (TreeViewItem)(it1.ItemContainerGenerator.ContainerFromItem(desc1));
                    if (it != null)
                        return ((ConceptionLanguageVM)it1.Header).Language;
                    j++;
                }



            }
            //ConceptionDescriptionVM descVM = item as ConceptionDescriptionVM;

            //if (desc != null)
            //{

            //    return desc.ConceptionRegistryDescription;
            //}


            TreeViewItem item1 = item as TreeViewItem;
            if (item1 != null)
            {
                if (item1.Parent == trvDescriptions)
                {
                    for (int i = 0; i < trvDescriptions.Items.Count; i++)
                        if (item1 == trvDescriptions.Items[i])
                            return (LanguageId)i;
                }
                else
                {
                    TreeViewItem itemParent = item1.Parent as TreeViewItem;
                    for (int i = 0; i < trvDescriptions.Items.Count; i++)
                        if (itemParent == trvDescriptions.Items[i])
                            return (LanguageId)i;
                }
            }

            return LanguageId.Undefined;
        }

        private void treeConceptions_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateEditDescription();
        }

        private string GetSelectedConceptionDescription(object item)
        {
            if (item == null)
                return string.Empty;

            ConceptionDescriptionViewModel desc = item as ConceptionDescriptionViewModel;
            if (desc != null)
            {
                return desc.ConceptionRegistryDescription;
            }

            //ConceptionDescriptionVM descVM = item as ConceptionDescriptionVM;

            //if (desc != null)
            //{

            //    return desc.ConceptionRegistryDescription;
            //}

            TreeViewItem item1 = item as TreeViewItem;
            object a = trvDescriptions.SelectedValue;
            TreeViewItem it = (TreeViewItem)(trvDescriptions.ItemContainerGenerator.ContainerFromItem(item));
            if (it != null && it.HasItems)
            {

                TreeViewItem subItem = (TreeViewItem)(it.ItemContainerGenerator.ContainerFromItem(it.Items[0]));
                if (subItem != null)
                {
                    subItem.IsSelected = true;
                    return ((ConceptionDescriptionViewModel)it.Items[0]).ConceptionRegistryDescription;
                }
            }
            if (item1 != null)
            {
                if (item1.Parent == trvDescriptions)
                {
                    //return (string)item1.Items[0];
                    //ConceptionDescription item2 = item1.Items[0] as ConceptionDescription;
                    //if(item2 != null)
                    //    return item2.ConceptionRegistryDescription;
                    if (item1.HasItems)
                    {
                        TreeViewItem item2 = item1.Items[0] as TreeViewItem;
                        if (item2 != null)
                        {
                            item2.IsSelected = true;
                            return (string)item2.Header;
                        }
                    }

                }
                else
                {
                    item1.IsSelected = true;
                    return (string)item1.Header;
                }
            }
            //else
            //{
            //    ConceptionDescription desc = item as ConceptionDescription;
            //    //string desc = item as string;
            //    if( desc != null)
            //        return desc.ConceptionRegistryDescription;
            //}

            return string.Empty;
        }

        private void btnApplyTerm_Click(object sender, RoutedEventArgs e)
        {
            if (m_NewConception == null)
                return;

            TranslationVM topic = cmbTopic.SelectedItem as TranslationVM;
            TranslationVM semantic = cmbSemantic.SelectedItem as TranslationVM;
            if (topic != null && topic.Item != null)
            {
                m_NewConception.TopicId = topic.Item.ServConceptionId;
                m_NewConception.Topic = topic.Item.Translation;
            }
            if (semantic != null && semantic.Item != null)
            {
                m_NewConception.SemanticId = semantic.Item.ServConceptionId;
                m_NewConception.Semantic = semantic.Item.Translation;
            }
        }
    }
}
