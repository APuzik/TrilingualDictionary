using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using TrilingualDictionaryCore;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using TrilingualDictionaryViewModel;
using System.Collections.ObjectModel;

namespace TrilingualUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrilingualDictionaryViewModel.TrilingualDictionaryViewModel m_DictionaryViewModel = null;
        string m_DictionaryDataFolder = "";

        int m_PrevTopic = 0;
        int m_PrevSemantic = 0;
        int m_PrevLangPart = 0;
        int m_PrevChangeable = 0;

        ConceptionVM curConception = null;
        public ConceptionVM CurConception
        {
            get { return curConception; }
        }
        int m_I = 0;
        int m_J = 0;

        public TrilingualDictionaryViewModel.TrilingualDictionaryViewModel DictionaryViewModel
        {
            get { return m_DictionaryViewModel; }
        }
        public MainWindow()
        {
            //try
            {
                m_DictionaryDataFolder = Properties.Settings.Default.DictionaryData;
                m_DictionaryViewModel = new TrilingualDictionaryViewModel.TrilingualDictionaryViewModel(m_DictionaryDataFolder);

                InitializeComponent();

                SwitchDataContext();
                //listConceptions.ItemsSource = CollectionViewSource.GetDefaultView(m_DictionaryViewModel.ConceptionDescriptions);
                ////ListCollectionView view =
                ////(ListCollectionView)CollectionViewSource.GetDefaultView(listConceptions.ItemsSource);
                //string b = VirtualizingStackPanel.IsVirtualizingProperty.ToString();
                ////view.CustomSort = new ConceptionDescriptionViewModel.ConceptionDescriptionComparer(m_DictionaryViewModel.MainLanguage);
                // listConceptions.Items.SortDescriptions.Add(new SortDescription("ParentName", ListSortDirection.Ascending));
                //listConceptions.Items.SortDescriptions.Add(new SortDescription("OwnName", ListSortDirection.Ascending));
                UpdateAllControls();
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void SwitchDataContext()
        {
            DataContext = this;
            if (m_DictionaryViewModel != null && treeConceptions != null)
            {
                treeConceptions.ItemsSource = m_DictionaryViewModel.Chapters;
            }

            if (trvDescriptions != null)
            {
                if (CurConception != null)
                {
                    trvDescriptions.ItemsSource = CurConception.Descriptions;
                    //trvDescriptions.Items.[0]
                }
                else
                    trvDescriptions.ItemsSource = null;
            }

            if (cmbTopic != null)
            {
                cmbTopic.ItemsSource = m_DictionaryViewModel.ActiveTopics;
                cmbTopic.SelectedIndex = m_PrevTopic;
            }

            if (cmbSemantic != null)
            {
                cmbSemantic.ItemsSource = m_DictionaryViewModel.ActiveSemantics;
                cmbSemantic.SelectedIndex = m_PrevSemantic;
            }

            if (cmbLangPart != null)
            {
                cmbLangPart.ItemsSource = m_DictionaryViewModel.ActivePartsOfSpeech;
                cmbLangPart.SelectedIndex = m_PrevLangPart;
            }

            if (cmbChangableType != null)
            {
                cmbChangableType.ItemsSource = m_DictionaryViewModel.ActiveChangeables;
                cmbChangableType.SelectedIndex = m_PrevChangeable;
            }
            /*new
        {
            ActiveChapters = m_DictionaryViewModel.Chapters,
            ActiveConception = curConception == null ? null : curConception.Descriptions,
            ActiveTopics = m_DictionaryViewModel.ActiveTopics,
            ActiveSemantics = m_DictionaryViewModel.ActiveSemantics,
            ActiveLangParts = m_DictionaryViewModel.ActivePartsOfSpeech,
            ActiveChangeables = m_DictionaryViewModel.ActiveChangeables
        };*/
        }

        private void chkAllLanguages_Clicked(object sender, RoutedEventArgs e)
        {
            //cmbLanguageForDescription.IsEnabled = chkAllLanguages.IsChecked != true;
            UpdateDescription();
        }

        private void cmbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_DictionaryViewModel.MainLanguage = (LanguageId)cmbLanguages.SelectedIndex;

            SwitchDataContext();
            //m_DictionaryViewModel.Refresh();
            //if (listConceptions != null)
            //  listConceptions.Items.Refresh();
        }

        private void listConceptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAllControls();
        }

        private void UpdateAllControls()
        {
            UpdateDescription();
            UpdateTreeDescription();
            UpdateEditDescription();
            //lblCount.Content = string.Format("{0} терминов", listConceptions.Items.Count);
            //lblCount.UpdateLayout();
            //listConceptions.UpdateLayout();// Items.Refresh();
        }

        private void UpdateTreeDescription()
        {
            //DataContext = null;

            //ConceptionDescriptionViewModel conceptionDescription = null;// (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            //if (conceptionDescription == null)
            //    return;

            ConceptionDescriptionViewModel conceptionDescription = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (conceptionDescription == null)
                return;

            curConception = new ConceptionVM(conceptionDescription.Conception);
            SwitchDataContext();

            //trvDescriptions.ItemsSource = conceptionDescription.ObjectDescription.OwnConception.Languages;

            //trvDescriptions.Items.Clear();

            //foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
            //{
            //    if (langId == LanguageId.Undefined)
            //        continue;

            //    TreeViewItem item1 = new TreeViewItem();
            //    item1.Header = LanguageIdToSting.GetDescription(langId, langId);
            //    List<ConceptionDescription> descs = conceptionDescription.ObjectDescription.OwnConception.GetAllConceptionDescriptions(langId);
            //    if (descs.Count == 0)
            //    {
            //        TreeViewItem item2 = new TreeViewItem();
            //        item2.Header = "Осутствует";
            //        item2.FontStyle = FontStyles.Italic;
            //        item2.Foreground = Brushes.LightGray;
            //        int pos = item1.Items.Add(item2);
            //        item1.IsExpanded = true;
            //        trvDescriptions.Items.Add(item1);
            //    }
            //    else
            //    {
            //        foreach (ConceptionDescription desc in descs)
            //        {
            //            TreeViewItem item2 = new TreeViewItem();
            //            item2.Header = desc.ConceptionRegistryDescription;
            //            //TreeViewItem item3 = new TreeViewItem();
            //            //item3.SetValue()
            //            //item1.Items.Add(desc);//.ConceptionRegistryDescription);
            //            item1.Items.Add(item2);
            //            item1.IsExpanded = true;
            //        }
            //        trvDescriptions.Items.Add(item1);
            //    }
            //}
        }

        private void UpdateDescription()
        {
            ConceptionDescriptionViewModel conceptionDescription = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (conceptionDescription == null)
                return;

            Conception conception = conceptionDescription.Conception;
            //if (chkAllLanguages.IsChecked == true)
            //    txtDescription.Text = GetAllDescriptions(conception);
            //else
            //{
            //    LanguageId languageForDescription =
            //        (LanguageId)cmbLanguageForDescription.SelectedIndex;
            //    txtDescription.Text = GetConceptionDescription(conception, languageForDescription);
            //    //GetConceptionDescriptionsForLanguage(conception, languageForDescription);
            //}

            chkHumanHandled.IsChecked = conception.IsHumanHandled;
        }

        private string GetConceptionDescriptionsForLanguage(Conception conception, LanguageId languageForDescription)
        {
            throw new NotImplementedException();
        }


        private void UpdateEditDescription()
        {
            //ConceptionDescriptionViewModel conceptionDescription = null;//TODO (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            //if (conceptionDescription == null)
            //{
            //    if (btnRemoveConception != null)
            //        btnRemoveConception.IsEnabled = false;
            //    return;
            //}

            if (treeConceptions == null)
            {
                cmbChangableType.SelectedIndex = 0;
                txtChangable.Text = "";
                cmbLangPart.SelectedIndex = 0;
                return;
            }
                

            ConceptionDescriptionViewModel conceptionDescription = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
            if (conceptionDescription == null)
            {
                //ConceptionLanguageVM concLanguage = trvDescriptions.SelectedItem as ConceptionLanguageVM;
                //if (concLanguage == null)
                //{
                    if (btnRemoveConception != null)
                        btnRemoveConception.IsEnabled = false;

                    cmbChangableType.SelectedIndex = 0;
                    txtChangable.Text = "";
                    cmbLangPart.SelectedIndex = 0;

                    return;
                //}
                //else 
                //{
                //    TreeViewItem it = (TreeViewItem)(trvDescriptions.ItemContainerGenerator.ContainerFromItem(trvDescriptions.SelectedItem));
                //    if (it != null)
                //    {
                //        TreeViewItem it1 = (TreeViewItem)(it.ItemContainerGenerator.ContainerFromIndex(0));
                //        it1.IsSelected = true;
                //    }
                //}
            }
            conceptionDescription.IsSelected = true;

            Conception conception = conceptionDescription.Conception;
            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;
            
            string activeDesc = GetSelectedConceptionDescription(trvDescriptions.SelectedItem);
            if (string.Compare(activeDesc, "Отсутствует", true) == 0)
                txtEditDescription.Text = "";
            else
                txtEditDescription.Text = activeDesc;

            //txtEditDescription.Text = GetSelectedConceptionDescription(trvDescriptions.SelectedItem);// GetConceptionDescription(conception, languageForEdit);

            ConceptionDescription description = conceptionDescription.ConceptionDescription;// conception.GetConceptionDescriptionOrEmpty(languageForEdit, 2);
            //cmbChangableType.Text = 
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

            txtChangable.Text = description.ChangeableDB.Value;
            //cmbTopic.Text = conception.Topic;
            if (conception != null)
            {
                for (int i = 0; i < cmbTopic.Items.Count; i++)
                {
                    TranslationVM tr = cmbTopic.Items[i] as TranslationVM;
                    if (tr != null && tr.Item != null && tr.Item.ServConceptionId == conception.TopicId)
                    {
                        cmbTopic.SelectedItem = cmbTopic.Items[i];
                        break;
                    }
                }
            }
            //cmbSemantic.SelectedItem = () =>
            if (conception != null)
            {
                for (int i = 0; i < cmbSemantic.Items.Count; i++)
                {
                    TranslationVM tr = cmbSemantic.Items[i] as TranslationVM;
                    if (tr != null && tr.Item != null && tr.Item.ServConceptionId == conception.SemanticId)
                    {
                        cmbSemantic.SelectedItem = cmbSemantic.Items[i];
                        break;
                    }
                }
            }
            bFound = false;
            foreach (TranslationVM tr in cmbLangPart.ItemsSource)
            {
                if (tr.Item != null && description.LangPartId == tr.Item.Id)
                {
                    cmbLangPart.SelectedItem = tr;
                    bFound = true;
                    break;
                }
            }
            if (!bFound)
                cmbLangPart.SelectedIndex = 0;
            //cmbLangPart.Text = description.LangPart;
            //txtLink.Text = conception.Link;


            bool isDescriptionExists = !string.IsNullOrWhiteSpace(txtEditDescription.Text);

            btnAddDescription.IsEnabled = true;// !isDescriptionExists;
            btnChangeDescription.IsEnabled = isDescriptionExists;
            btnRemoveDescription.IsEnabled = isDescriptionExists;
            btnRemoveConception.IsEnabled = true;

            LanguageId langNeeded = GetSelectedDescriptionLanguage(trvDescriptions.SelectedItem);
            if (langNeeded != LanguageId.Undefined)
            {
                cmbChangeforEdit.SelectedIndex = (int)langNeeded;
            }
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
                    //ConceptionDescriptionViewModel item2 = item1.Items[0] as ConceptionDescriptionViewModel;
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
            //    ConceptionDescriptionViewModel desc = item as ConceptionDescriptionViewModel;
            //    //string desc = item as string;
            //    if( desc != null)
            //        return desc.ConceptionRegistryDescription;
            //}

            return string.Empty;
        }

        ////private string GetAllDescriptions(Conception conception)
        ////{
        ////    StringBuilder sb = new StringBuilder();
        ////    foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
        ////    {
        ////        if (langId == LanguageId.Undefined)
        ////            continue;

        ////        List<ConceptionDescription> descriptions = conception.GetAllConceptionDescriptions(langId);
        ////        foreach (ConceptionDescription desc in descriptions)
        ////        {
        ////            sb.AppendFormat("{0}\n{1}. {2}\n\n", LanguageIdToSting.GetDescription(langId, langId), desc.OverallDescriptionId, desc.ConceptionRegistryDescription);
        ////            //GetConceptionDescription(conception, lang));
        ////        }
        ////    }
        ////    return sb.ToString();
        ////}

        private ConceptionDescriptionViewModel FindDescriptionInTree(string description)
        {
            throw new NotImplementedException();
        }

        private string GetConceptionDescription(Conception conception, LanguageId languageForDescription)
        {
            List<ConceptionDescription> descriptions = conception.GetConceptionDescriptions(languageForDescription);
            StringBuilder sb = new StringBuilder();
            foreach (ConceptionDescription desc in descriptions)
            {
                sb.AppendFormat("{0}. {1}\n", desc.DescriptionId, desc.ConceptionRegistryDescription);
            }
            return sb.ToString();
            //return conception.GetConceptionRegistryDescriptionOrEmpty(languageForDescription);
        }

        private void cmbLanguageForDescription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDescription();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string textToSearch = txtSearch.Text;
            bool isFound = m_DictionaryViewModel.PerformSearch(textToSearch, treeConceptions.SelectedItem as ConceptionDescriptionViewModel);
            if (!isFound)
            {
                MessageBox.Show("Заданный текст не найден.");
            }
            else
            {
                //DependencyObject dObject = treeConceptions.ItemContainerGenerator.ContainerFromItem(treeConceptions.SelectedItem);
                //TreeViewItem item = dObject as TreeViewItem;
                //if (item != null)
                //{
                //    item.BringIntoView();
                //}
            }
            //Chapter c = m_DictionaryViewModel.Alphabet.Letters.FirstOrDefault(x =>
            //    x.Letter == textToSearch[0].ToString());

            ////treeConceptions.


            ////for (int i = 0;  i < treeConceptions.Items.Count; i++)
            ////{
            //TreeViewItem it1 = (TreeViewItem)treeConceptions.ItemContainerGenerator.ContainerFromItem(treeConceptions.Items[0]);

            //if (m_I > 2)
            //{
            //    TreeViewItem it2 = (TreeViewItem)it1.ItemContainerGenerator.ContainerFromItem(it1.Items[m_I - 1]);
            //    TreeViewItem it3 = (TreeViewItem)it2.ItemContainerGenerator.ContainerFromItem(it2.Items[m_J]);
            //    it3.IsSelected = true;
            //    m_J++;
            //}
            //else
            //{
            //    TreeViewItem it2 = (TreeViewItem)it1.ItemContainerGenerator.ContainerFromItem(it1.Items[m_I]);
            //    it2.IsSelected = true;
            //    m_I++;
            //}
            //    //bool isFound = ((string)it1.Header).Contains(textToSearch);
            //    //if (isFound)
            //    //    it1.IsSelected = true;
            //    //else
            //    {
            //        for (int j = 0;  j < it1.Items.Count; j++)
            //        {
            //            TreeViewItem it2 = (TreeViewItem)it1.ItemContainerGenerator.ContainerFromItem(it1.Items[j]);
            //            bool isFound = ((string)it2.Header).Contains(textToSearch);
            //            if (isFound)
            //                it2.IsSelected = true;
            //            else
            //            {
            //                for (int k = 0;  k < it2.Items.Count; k++)
            //                {
            //                    TreeViewItem it3 = (TreeViewItem)it2.ItemContainerGenerator.ContainerFromItem(it2.Items[k]);
            //                    isFound = ((string)it3.Header).Contains(textToSearch);
            //                    if (isFound)
            //                        it3.IsSelected = true;
            //                }
            //            }
            //        }
            //    }
            //}
            //    it = (TreeViewItem)(it1.ItemContainerGenerator.ContainerFromItem(desc1));
            //    Chapter item = treeConceptions.Items[i] as Chapter;
            //    for (int j = 0; j < item.Descriptions.Count; j++)
            //    {
            //        bool isFound = item.Descriptions[j].Conception.Find(textToSearch, m_DictionaryViewModel.MainLanguage);
            //        if (isFound)
            //    }
            //}
            //bool isFound = false;
            //for (int i = treeConceptions.SelectedItem + 1; i < listConceptions.Items.Count; i++)
            //{
            //    ConceptionDescriptionViewModel conception = (ConceptionDescriptionViewModel)listConceptions.Items[i];
            //    isFound = conception.ObjectDescription.OwnConception.Find(textToSearch, m_DictionaryViewModel.MainLanguage);
            //    if (isFound)
            //    {
            //        ScrollToItem(i);
            //        break;
            //    }
            //}
            //TODO
            ////string textToSearch = txtSearch.Text;
            ////bool isFound = false;
            ////for (int i = listConceptions.SelectedIndex + 1; i < listConceptions.Items.Count; i++)
            ////{
            ////    ConceptionDescriptionViewModel conception = (ConceptionDescriptionViewModel)listConceptions.Items[i];
            ////    isFound = conception.ObjectDescription.OwnConception.Find(textToSearch, m_DictionaryViewModel.MainLanguage);
            ////    if (isFound)
            ////    {
            ////        ScrollToItem(i);
            ////        break;
            ////    }
            ////}

            //if (!isFound)
            //{
            //    MessageBox.Show(string.Format(Properties.Resources.fmtSearchMessage, textToSearch));
            //}
        }

        private void SelectItemById(int id)
        {
            //TODO
            //for (int i = 0; i < listConceptions.Items.Count; i++)
            //{
            //    ConceptionDescriptionViewModel conception = (ConceptionDescriptionViewModel)listConceptions.Items[i];
            //    if (id == conception.ObjectDescription.OwnConception.ConceptionId)
            //    {
            //        ScrollToItem(i);
            //        break;
            //    }
            //}
        }

        private void ScrollToItem(int i)
        {
            //listConceptions.SelectedIndex = i;
            //listConceptions.ScrollIntoView(listConceptions.SelectedItem);
        }

        private void cmbChangeforEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trvDescriptions == null)
                return;

            TreeViewItem it = (TreeViewItem)(trvDescriptions.ItemContainerGenerator.ContainerFromIndex(cmbChangeforEdit.SelectedIndex));
            if (it != null)
            {
                TreeViewItem it1 = (TreeViewItem)(it.ItemContainerGenerator.ContainerFromIndex(0));
                //foreach (ConceptionDescriptionViewModel it1 in it.Items)
                {
                    it1.IsSelected = true;
                    return;
                    //if (it1.IsSelected)
                      //  return;
                }
                it.IsSelected = true;
            }

            //UpdateEditDescription();
        }

        private void btnAddDescription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEditDescription.Text))
                {
                    MessageBox.Show("Введите текст переводного эквивалента");
                    txtEditDescription.Focus();
                    return;
                }

                txtEditDescription.Text = txtEditDescription.Text.Trim();

                ConceptionDescriptionViewModel descVM = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
                if (descVM == null)
                    return;

                LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;
                TranslationVM chType = cmbChangableType.SelectedItem as TranslationVM;
                TranslationVM langPart = cmbLangPart.SelectedItem as TranslationVM;

                m_DictionaryViewModel.AddDescriptionToConception(curConception.Conception.ConceptionId, txtEditDescription.Text, languageForEdit,
                    (chType == null || chType.Item == null) ? 0 : chType.Item.ServConceptionId,//"" : chType.Translation,
                    txtChangable.Text,
                    (langPart == null || langPart.Item == null) ? 0 : langPart.Item.ServConceptionId,//Translation,
                true);

                ConceptionDescriptionViewModel desc2 = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
                if (desc2 != null)
                {
                    curConception = new ConceptionVM(desc2.Conception);
                }

                SwitchDataContext();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnChangeDescription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEditDescription.Text))
                {
                    MessageBox.Show("Введите текст переводного эквивалента");
                    txtEditDescription.Focus();
                    return;
                }

                txtEditDescription.Text = txtEditDescription.Text.Trim();

                ConceptionDescriptionViewModel descVM = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
                if (descVM == null)
                    return;

                ConceptionDescriptionViewModel descVM2 = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;

                bool b = descVM == descVM2;

                LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;
                if (descVM.ConceptionRegistryDescription == "Отсутствует")
                {
                    btnAddDescription_Click(sender, e);
                }
                else
                {
                    TranslationVM chType = cmbChangableType.SelectedItem as TranslationVM;
                    TranslationVM langPart = cmbLangPart.SelectedItem as TranslationVM;

                    m_DictionaryViewModel.ChangeDescriptionOfConception(descVM, txtEditDescription.Text, languageForEdit,
                    (chType == null || chType.Item == null) ? 0 : chType.Item.ServConceptionId,//"" : chType.Translation,
                    txtChangable.Text,
                    (langPart == null || langPart.Item == null) ? 0 : langPart.Item.ServConceptionId,//Translation,
                    true);

                    //if (descVM.ConceptionRegistryDescription == descVM2.ConceptionRegistryDescription)
                    SelectItem(descVM);

                    ConceptionDescriptionViewModel desc2 = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
                    if (desc2 != null)
                    {
                        curConception = new ConceptionVM(desc2.Conception);
                    }

                    SwitchDataContext();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectItem(ConceptionDescriptionViewModel descVM)
        {
            try
            {
                if (descVM.Conception.ParentId == 0)
                {

                }
                treeConceptions.UpdateLayout();

                Chapter chapter = m_DictionaryViewModel.GetAnyChapter(descVM);
                DependencyObject dObject = treeConceptions.ItemContainerGenerator.ContainerFromItem(chapter);
                TreeViewItem itChapter = dObject as TreeViewItem;
                if (itChapter != null)
                {
                    itChapter.IsExpanded = true;
                    for (int i = 0; i < itChapter.Items.Count; i++)
                    {
                        ConceptionDescriptionViewModel o1 = itChapter.Items[i] as ConceptionDescriptionViewModel;
                        if (o1 != null)
                        {
                            if (string.Compare(descVM.ConceptionRegistryDescription, o1.ConceptionRegistryDescription, true) == 0)
                            {
                                TreeViewItem subItem = (TreeViewItem)(itChapter.ItemContainerGenerator.ContainerFromItem(itChapter.Items[i]));
                                if (subItem != null)
                                {
                                    subItem.IsSelected = true;
                                    subItem.BringIntoView();
                                }
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //object obj = ((TreeViewItem)dObject).ItemContainerGenerator.ContainerFromItem(descVM);
            //TreeViewItem it = obj as TreeViewItem;
            //if (it != null)
            //    it.IsSelected = true;
        }

        //private int GetTreeSelectedIndex()
        //{
        //    TreeViewItem item = trvDescriptions.SelectedItem as TreeViewItem;
        //    if (item == null)
        //        return 0;

        //    if (item.Parent == trvDescriptions)
        //        return 0;

        //    TreeViewItem itemParent = item.Parent as TreeViewItem;
        //    for (int i = 0; i < itemParent.Items.Count; i++)
        //        if (item == itemParent.Items[i])
        //            return i;

        //    return 0;
        //}

        private string GetTreeSelectedDescription()
        {
            ConceptionDescriptionViewModel desc = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
            if (desc == null)
                return string.Empty;

            return desc.ConceptionRegistryDescription;
        }

        private void btnRemoveDescription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConceptionDescriptionViewModel descVM = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
                if (descVM == null)
                    return;

                LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;
                m_DictionaryViewModel.RemoveDescriptionFromConception(descVM, languageForEdit, true);


                ConceptionDescriptionViewModel desc2 = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
                if (desc2 != null)
                {
                    curConception = new ConceptionVM(desc2.Conception);
                }
                else
                {
                    curConception = null;
                }

                SwitchDataContext();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddConception_Click(object sender, RoutedEventArgs e)
        {
            AddConception wnd = new AddConception(m_DictionaryViewModel);
            if (wnd.ShowDialog() == true && wnd.m_NewConception != null)
            {
                
                m_DictionaryViewModel.AddConception(wnd.m_NewConception);
                UpdateAllControls();
                //m_DictionaryViewModel.LoadAllDescriptions();
                SelectItemById(wnd.m_NewConception.ConceptionId);
            }
            SwitchDataContext();
        }

        private void btnRemoveConception_Click(object sender, RoutedEventArgs e)
        {
            ConceptionDescriptionViewModel desc = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (desc == null)
                return;

            m_DictionaryViewModel.RemoveConception(desc);
            UpdateAllControls();
 //           m_DictionaryViewModel.LoadAllDescriptions();
            SwitchDataContext();

            

            //ConceptionDescriptionViewModel conception = null;//TODO (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            //if (conception == null)
            //    return;

            //m_DictionaryViewModel.RemoveConception(conception);

            //UpdateAllControls();
            //listConceptions.Items.Refresh();
            //listConceptions.SelectedIndex = -1;
        }

        private void chkFlatView_Checked(object sender, RoutedEventArgs e)
        {
            //listConceptions.Visibility = Visibility.Visible;
            //treeConceptions.Visibility = Visibility.Hidden;
        }

        private void chkFlatView_Unchecked(object sender, RoutedEventArgs e)
        {
            //listConceptions.Visibility = Visibility.Hidden;
            ///treeConceptions.Visibility = Visibility.Visible;
        }

        private void treeConceptions_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateEditDescription();
        }

        private void listConceptions_KeyUp(object sender, KeyEventArgs e)
        {
            //int increment = 0;
            //switch (e.Key)
            //{
            //    case Key.Down:
            //        increment++;
            //        //if (!listConceptions.Items.MoveCurrentToNext()) 
            //        //    listConceptions.Items.MoveCurrentToLast();
            //        break;

            //    case Key.Up:
            //        increment--;
            //        //if (!listConceptions.Items.MoveCurrentToPrevious())
            //        //    listConceptions.Items.MoveCurrentToFirst();
            //        break;
            //}

            //listConceptions.SelectedIndex += increment;
            //e.Handled = true;
            //if (listConceptions.SelectedItem != null)
            //{
            //    (Keyboard.FocusedElement as UIElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //}
        }

        private void ChkBrackets_OnClick(object sender, RoutedEventArgs e)
        {

            //if (chkBrackets.IsChecked == true)
            //    listConceptions.Items.Filter = ConceptionDescriptionViewModel.BracketsrFilter;
            //else
            //    listConceptions.Items.Filter = null;
            //UpdateAllControls();
        }

        private void ChkHumanHandled_OnClick(object sender, RoutedEventArgs e)
        {
            ConceptionDescriptionViewModel conception = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (conception == null)
                return;

            //conception.ObjectDescription.OwnConception.IsHumanHandled = chkHumanHandled.IsChecked == true;
            //m_DictionaryViewModel.Save();

            //if (chkBrackets.IsChecked == true)
            //    listConceptions.Items.Filter = ConceptionDescriptionViewModel.BracketsrFilter;
            //else
            //    listConceptions.Items.Filter = null;

            UpdateAllControls();
        }

        private void btnSplitConception_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMergeConceptions_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChkSameDescriptions_OnClick(object sender, RoutedEventArgs e)
        {
            //if (chkSameDescriptions.IsChecked == true)
            //{
            //    List<ConceptionDescriptionViewModel> temp = new List<ConceptionDescriptionViewModel>();
            //    List<ConceptionDescriptionViewModel> temp2 = new List<ConceptionDescriptionViewModel>();
            //    foreach (ConceptionDescriptionViewModel desc in m_DictionaryViewModel.ConceptionDescriptions)
            //    {
            //        var selected = temp.Where(x => x.RegistryDescription == desc.RegistryDescription).ToList();
            //        if (selected.Count > 0)
            //        {
            //            temp2.Add(desc);
            //            selected.ForEach(item => temp.Remove(item));
            //            temp2.AddRange(selected);
            //        }
            //        else
            //            temp.Add(desc);
            //    }
            //    m_DictionaryViewModel.ConceptionDescriptions.Clear();
            //    temp2.ForEach(item => m_DictionaryViewModel.ConceptionDescriptions.Add(item));
            //}
            //else
            //{
            //    m_DictionaryViewModel.LoadAllDescriptions();
            //    UpdateAllControls();
            //}
        }

        private void treeAllConceptions_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Chapter chapter = treeConceptions.SelectedItem as Chapter;
            if (chapter != null)
            {
                curConception = null;
                return;
            }

            ConceptionDescriptionViewModel desc = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (desc != null)
            {
                curConception = new ConceptionVM(desc.Conception);
                SwitchDataContext();
            }
            //UpdateEditDescription();
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
                
                if (oldSelectedIndex >= combo.Items.Count - 2)
                    oldSelectedIndex = -1;
                
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConceptionDescriptionViewModel descVM = trvDescriptions.SelectedItem as ConceptionDescriptionViewModel;
            if (descVM == null || descVM.Conception == null)
                return;

            Conception conception = descVM.Conception;
            TranslationVM topic = cmbTopic.SelectedItem as TranslationVM;
            TranslationVM semantic = cmbSemantic.SelectedItem as TranslationVM;
            if (topic.Item != null)
            {
                conception.TopicId = topic.Item.ServConceptionId;
                conception.Topic = topic.Item.Translation;
            }
            else
            {
                conception.TopicId = 0;
                conception.Topic = "";
            }
            if (semantic.Item != null)
            {
                conception.SemanticId = semantic.Item.ServConceptionId;
                conception.Semantic = semantic.Item.Translation;
            }
            else
            {
                conception.SemanticId = 0;
                conception.Semantic = "";
            }

            conception.SaveConception();
        }
    }
}
