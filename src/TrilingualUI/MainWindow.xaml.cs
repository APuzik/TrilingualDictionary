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

namespace TrilingualUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrilingualDictionaryViewModel.TrilingualDictionaryViewModel m_DictionaryViewModel = null;
        string m_DictionaryDataFolder = "";

        private ConceptionVM curConception = null;
        int m_I = 0;
        int m_J = 0;

        public MainWindow()
        {
            //try
            {
                m_DictionaryDataFolder = Properties.Settings.Default.DictionaryData;
                m_DictionaryViewModel = new TrilingualDictionaryViewModel.TrilingualDictionaryViewModel(m_DictionaryDataFolder);

                InitializeComponent();

                DataContext = new
                {
                    ActiveLanguage = m_DictionaryViewModel.Chapters,
                    ActiveConception = curConception == null ? null : curConception.Descriptions
                };
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

        private void chkAllLanguages_Clicked(object sender, RoutedEventArgs e)
        {
            //cmbLanguageForDescription.IsEnabled = chkAllLanguages.IsChecked != true;
            UpdateDescription();
        }

        private void cmbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_DictionaryViewModel.MainLanguage = (LanguageId)cmbLanguages.SelectedIndex;

            DataContext = new
            {
                ActiveLanguage = m_DictionaryViewModel.Chapters,
                ActiveConception = curConception == null ? null : curConception.Descriptions
            };
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
            DataContext = new
            {
                ActiveLanguage = m_DictionaryViewModel.Chapters,
                ActiveConception = curConception.Descriptions
                //
            };
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
                return;

            ConceptionDescriptionViewModel conceptionDescription = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (conceptionDescription == null)
            {
                if (btnRemoveConception != null)
                    btnRemoveConception.IsEnabled = false;
                return;
            }

            Conception conception = conceptionDescription.Conception;
            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;
            txtEditDescription.Text = GetSelectedConceptionDescription(trvDescriptions.SelectedItem);// GetConceptionDescription(conception, languageForEdit);

            ConceptionDescription description = conception.GetConceptionDescriptionOrEmpty(languageForEdit, 0);
            txtChangableType.Text = description.Changeable.Type;
            txtChangable.Text = description.Changeable.Value;
            txtTopic.Text = conception.Topic;
            txtSemantic.Text = conception.Semantic;
            txtLangPart.Text = description.LangPart;
            txtLink.Text = conception.Link;


            bool isDescriptionExists = !string.IsNullOrWhiteSpace(txtEditDescription.Text);

            btnAddDescription.IsEnabled = true;// !isDescriptionExists;
            btnChangeDescription.IsEnabled = isDescriptionExists;
            btnRemoveDescription.IsEnabled = isDescriptionExists;
            btnRemoveConception.IsEnabled = true;

            LanguageId langNeeded = GetSelectedDescriptionLanguage(trvDescriptions.SelectedItem);
            if (langNeeded != LanguageId.Undefined)
                cmbChangeforEdit.SelectedIndex = (int)langNeeded;
        }

        private LanguageId GetSelectedDescriptionLanguage(object item)
        {
            if (item == null)
                return LanguageId.Undefined;

            ConceptionDescriptionVM desc = item as ConceptionDescriptionVM;
            if (desc != null)
            {
                return desc.Language;
            }

            ConceptionDescription desc1 = item as ConceptionDescription;
            int j = 0;
            if (desc1 != null)
            {
                TreeViewItem it = null;

                while (it == null && j < trvDescriptions.Items.Count)
                {
                    TreeViewItem it1 = (TreeViewItem)trvDescriptions.ItemContainerGenerator.ContainerFromItem(trvDescriptions.Items[j]);
                    it = (TreeViewItem)(it1.ItemContainerGenerator.ContainerFromItem(desc1));
                    if (it != null)
                        return ((ConceptionDescriptionVM)it1.Header).Language;
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

            ConceptionDescription desc = item as ConceptionDescription;
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
                    return ((ConceptionDescription)it.Items[0]).ConceptionRegistryDescription;
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

        private string GetAllDescriptions(Conception conception)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
            {
                if (langId == LanguageId.Undefined)
                    continue;

                List<ConceptionDescription> descriptions = conception.GetAllConceptionDescriptions(langId);
                foreach (ConceptionDescription desc in descriptions)
                {
                    sb.AppendFormat("{0}\n{1}. {2}\n\n", LanguageIdToSting.GetDescription(langId, langId), desc.DescriptionId, desc.ConceptionRegistryDescription);
                    //GetConceptionDescription(conception, lang));
                }
            }
            return sb.ToString();
        }

        private string GetConceptionDescription(Conception conception, LanguageId languageForDescription)
        {
            List<ConceptionDescription> descriptions = conception.GetAllConceptionDescriptions(languageForDescription);
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
            //for (int i = 0;  i < treeConceptions.Items.Count; i++)
            //{
                TreeViewItem it1 = (TreeViewItem)treeConceptions.ItemContainerGenerator.ContainerFromItem(treeConceptions.Items[0]);
                
                if (m_I > 2)
                {
                    TreeViewItem it2 = (TreeViewItem)it1.ItemContainerGenerator.ContainerFromItem(it1.Items[m_I-1]);
                    TreeViewItem it3 = (TreeViewItem)it2.ItemContainerGenerator.ContainerFromItem(it2.Items[m_J]);
                    it3.IsSelected = true;
                    m_J++;
                }
                else
                {
                    TreeViewItem it2 = (TreeViewItem)it1.ItemContainerGenerator.ContainerFromItem(it1.Items[m_I]);
                    it2.IsSelected = true;
                    m_I++;
                }
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
            UpdateEditDescription();
        }

        private void btnAddDescription_Click(object sender, RoutedEventArgs e)
        {
            //ConceptionDescriptionViewModel conception = null;//TODO (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            //if (conception == null)
            //    return;


            ConceptionDescriptionViewModel descVM = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (descVM == null)
                return;

            TrilingualDictionaryCore.LanguageId languageForEdit = (TrilingualDictionaryCore.LanguageId)cmbChangeforEdit.SelectedIndex;

            ConceptionDescription desc = new ConceptionDescription(descVM.Conception, "");// conception.GetConceptionDescription(languageForEdit);
            desc.LangPart = txtLangPart.Text;
//            desc.Topic = txtTopic.Text;
//            desc.Semantic = txtSemantic.Text;
            desc.Changeable.Type = txtChangableType.Text;
            desc.Changeable.Value = txtChangable.Text;
//            desc.Link = txtLink.Text;

            m_DictionaryViewModel.AddDescriptionToConception(descVM.Conception.ConceptionId, txtEditDescription.Text, languageForEdit);

            //m_DictionaryViewModel.LoadAllDescriptions();
            //DataContext = new
            //{
            //    ActiveLanguage = m_DictionaryViewModel.Chapters,
            //    ActiveConception = curConception == null ? null : curConception.Descriptions
            //};
            UpdateAllControls();
            //listConceptions.Items.Refresh();
        }

        private void btnChangeDescription_Click(object sender, RoutedEventArgs e)
        {
            //ConceptionDescriptionViewModel conception = null;//TODO (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            //if (conception == null)
            //    return;
            
            ConceptionDescriptionViewModel descVM = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (descVM == null)
                return;

            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;


            ConceptionDescription desc = new ConceptionDescription(descVM.Conception, "");// conception.GetConceptionDescription(languageForEdit);
            desc.LangPart = txtLangPart.Text;
            //desc.Topic = txtTopic.Text;
            //desc.Semantic = txtSemantic.Text;
            desc.Changeable.Type = txtChangableType.Text;
            desc.Changeable.Value = txtChangable.Text;
            //desc.Link = txtLink.Text;

            m_DictionaryViewModel.ChangeDescriptionOfConception(descVM.Conception.ConceptionId, txtEditDescription.Text, languageForEdit, GetTreeSelectedDescription());

            //m_DictionaryViewModel.LoadAllDescriptions();
            //DataContext = new
            //{
            //    ActiveLanguage = m_DictionaryViewModel.Chapters,
            //    ActiveConception = curConception == null ? null : curConception.Descriptions
            //};
            UpdateAllControls();
            //listConceptions.Items.Refresh();
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
            ConceptionDescription desc = trvDescriptions.SelectedItem as ConceptionDescription;
            if (desc == null)
                return string.Empty;

            return desc.ConceptionRegistryDescription;
        }

        private void btnRemoveDescription_Click(object sender, RoutedEventArgs e)
        {
            //ConceptionDescriptionViewModel conception = null;//TODO (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            //if (conception == null)
            //    return;

            ConceptionDescriptionViewModel descVM = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (descVM == null)
                return;

            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;

            m_DictionaryViewModel.RemoveDescriptionFromConception(descVM.Conception.ConceptionId, txtEditDescription.Text, languageForEdit);

            //m_DictionaryViewModel.LoadAllDescriptions();
            //DataContext = new
            //{
            //    ActiveLanguage = m_DictionaryViewModel.Chapters,
            //    ActiveConception = curConception == null ? null : curConception.Descriptions
            //};
            UpdateAllControls();
            //listConceptions.Items.Refresh();
        }

        private void btnAddConception_Click(object sender, RoutedEventArgs e)
        {
            AddConception wnd = new AddConception(m_DictionaryViewModel);
            if( wnd.ShowDialog() == true && wnd.m_NewConception != null)
            {
                UpdateAllControls();
                m_DictionaryViewModel.LoadAllDescriptions();
                SelectItemById(wnd.m_NewConception.ConceptionId);

                DataContext = new
                {
                    ActiveLanguage = m_DictionaryViewModel.Chapters,
                    ActiveConception = curConception == null ? null : curConception.Descriptions
                };
            }
            //LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;

            //int newConceptionId = m_DictionaryViewModel.AddConception(txtEditDescription.Text, languageForEdit);

            //Conception conception = m_DictionaryViewModel.GetConception(newConceptionId);
            //ConceptionDescription desc = conception.GetConceptionDescription(languageForEdit, 0);
            //desc.LangPart = txtLangPart.Text;
            //conception.Topic = txtTopic.Text;
            //conception.Semantic = txtSemantic.Text;
            //desc.Changeable.Type = txtChangableType.Text;
            //desc.Changeable.Value = txtChangable.Text;
            //conception.Link = txtLink.Text;

            //m_DictionaryViewModel.Save();


            //UpdateAllControls();
            ////listConceptions.Items.Refresh();
            //SelectItemById(newConceptionId);
        }

        private void btnRemoveConception_Click(object sender, RoutedEventArgs e)
        {
            ConceptionDescriptionViewModel desc = treeConceptions.SelectedItem as ConceptionDescriptionViewModel;
            if (desc == null)
                return;

            m_DictionaryViewModel.RemoveConception(desc);
            m_DictionaryViewModel.LoadAllDescriptions();
            DataContext = new
            {
                ActiveLanguage = m_DictionaryViewModel.Chapters,
                ActiveConception = curConception == null ? null : curConception.Descriptions
            };

            UpdateAllControls();

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
            //TreeViewItem item = (TreeViewItem)trvDescriptions.SelectedItem;
            //if (item == null)
            //    return;

            //if (item.Parent == trvDescriptions)
            //{
            //    ((TreeViewItem)trvDescriptions.Items[1]).IsSelected = true;
            //}
            //    DependencyObject dObject = trvDescriptions.ItemContainerGenerator.ContainerFromItem(item.Items[0]);
            //    ((TreeViewItem)dObject).IsSelected = true;
            //}

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
                DataContext = new
                {
                    ActiveLanguage = m_DictionaryViewModel.Chapters,
                    ActiveConception = curConception.Descriptions
                    //
                };
            }
        }
    }
}
