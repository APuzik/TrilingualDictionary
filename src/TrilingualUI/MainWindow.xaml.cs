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

        public MainWindow()
        {
            try
            {
                m_DictionaryDataFolder = Properties.Settings.Default.DictionaryData;
                m_DictionaryViewModel = new TrilingualDictionaryViewModel.TrilingualDictionaryViewModel(m_DictionaryDataFolder);

                InitializeComponent();

                listConceptions.ItemsSource = CollectionViewSource.GetDefaultView(m_DictionaryViewModel.ConceptionDescriptions);
                //ListCollectionView view =
                //    (ListCollectionView)CollectionViewSource.GetDefaultView(listConceptions.ItemsSource);
                //string b = VirtualizingStackPanel.IsVirtualizingProperty.ToString();
                //view.CustomSort = new ConceptionViewModel.ConceptionsComparer(m_DictionaryViewModel.MainLanguage);
                // listConceptions.Items.SortDescriptions.Add(new SortDescription("ParentName", ListSortDirection.Ascending));
                // listConceptions.Items.SortDescriptions.Add(new SortDescription("OwnName", ListSortDirection.Ascending));
                UpdateAllControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkAllLanguages_Clicked(object sender, RoutedEventArgs e)
        {
            cmbLanguageForDescription.IsEnabled = chkAllLanguages.IsChecked != true;
            UpdateDescription();
        }

        private void cmbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_DictionaryViewModel.MainLanguage = (LanguageId)cmbLanguages.SelectedIndex;
            m_DictionaryViewModel.Refresh();
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
            lblCount.Content = string.Format("{0} терминов", listConceptions.Items.Count);
            lblCount.UpdateLayout();
            listConceptions.UpdateLayout();// Items.Refresh();
        }

        private void UpdateTreeDescription()
        {
            ConceptionDescriptionViewModel conceptionDescription = (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            if (conceptionDescription == null)
                return;
            
            trvDescriptions.Items.Clear();
            
            foreach (LanguageId langId in Enum.GetValues(typeof(LanguageId)))
            {
                TreeViewItem item1 = new TreeViewItem();
	            item1.Header = LanguageIdToSting.GetDescription(langId, langId);
                List<ConceptionDescription> descs = conceptionDescription.ObjectDescription.OwnConception.GetAllConceptionDescriptions(langId);
                if (descs.Count == 0)
                {
                    TreeViewItem item2 = new TreeViewItem();
                    item2.Header = "Осутствует";
                    item2.FontStyle = FontStyles.Italic;
                    item2.Foreground = Brushes.LightGray;
                    int pos = item1.Items.Add(item2);
                    item1.IsExpanded = true;
                    trvDescriptions.Items.Add(item1);
                }
                else
                {
                    foreach (ConceptionDescription desc in descs)
                    {
                        item1.Items.Add(desc.ConceptionRegistryDescription);
                        item1.IsExpanded = true;
                        trvDescriptions.Items.Add(item1);
                    }
                }
            }
        }

        private void UpdateDescription()
        {
            ConceptionDescriptionViewModel conceptionDescription = (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            if (conceptionDescription == null)
                return;

            Conception conception = conceptionDescription.ObjectDescription.OwnConception;
            if (chkAllLanguages.IsChecked == true)
                txtDescription.Text = GetAllDescriptions(conception);
            else
            {
                LanguageId languageForDescription =
                    (LanguageId)cmbLanguageForDescription.SelectedIndex;
                txtDescription.Text = GetConceptionDescription(conception, languageForDescription);
                //GetConceptionDescriptionsForLanguage(conception, languageForDescription);
            }

            chkHumanHandled.IsChecked = conception.IsHumanHandled;
        }

        private string GetConceptionDescriptionsForLanguage(Conception conception, LanguageId languageForDescription)
        {
            throw new NotImplementedException();
        }


        private void UpdateEditDescription()
        {
            ConceptionDescriptionViewModel conceptionDescription = (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            if (conceptionDescription == null)
            {
                if (btnRemoveConception != null)
                    btnRemoveConception.IsEnabled = false;
                return;
            }

            Conception conception = conceptionDescription.ObjectDescription.OwnConception;
            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;
            txtEditDescription.Text = GetConceptionDescription(conception, languageForEdit);

            ConceptionDescription description = conception.GetConceptionDescriptionOrEmpty(languageForEdit, 0);
            txtChangableType.Text = description.Changeable.Type;
            txtChangable.Text = description.Changeable.Value;
            txtTopic.Text = description.Topic;
            txtSemantic.Text = description.Semantic;
            txtLangPart.Text = description.LangPart;
            txtLink.Text = description.Link;


            bool isDescriptionExists = !string.IsNullOrWhiteSpace(txtEditDescription.Text);

            btnAddDescription.IsEnabled = !isDescriptionExists;
            btnChangeDescription.IsEnabled = isDescriptionExists;
            btnRemoveDescription.IsEnabled = isDescriptionExists;
            btnRemoveConception.IsEnabled = true;
        }

        private string GetAllDescriptions(Conception conception)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LanguageId lang in Enum.GetValues(typeof(LanguageId)))
            {
                List<ConceptionDescription> descriptions = conception.GetAllConceptionDescriptions(lang);
                foreach (ConceptionDescription desc in descriptions)
                {
                    sb.AppendFormat("{0}\n{1}. {2}\n\n", LanguageIdToSting.GetDescription(lang, lang), desc.DescriptionId, desc.ConceptionRegistryDescription);
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
            bool isFound = false;
            for (int i = listConceptions.SelectedIndex + 1; i < listConceptions.Items.Count; i++)
            {
                ConceptionDescriptionViewModel conception = (ConceptionDescriptionViewModel)listConceptions.Items[i];
                isFound = conception.ObjectDescription.OwnConception.Find(textToSearch, m_DictionaryViewModel.MainLanguage);
                if (isFound)
                {
                    ScrollToItem(i);
                    break;
                }
            }

            if (!isFound)
            {
                MessageBox.Show(string.Format(Properties.Resources.fmtSearchMessage, textToSearch));
            }
        }

        private void SelectItemById(int id)
        {
            for (int i = 0; i < listConceptions.Items.Count; i++)
            {
                ConceptionViewModel conception = (ConceptionViewModel)listConceptions.Items[i];
                if (id == conception.ConceptionId)
                {
                    ScrollToItem(i);
                    break;
                }
            }
        }

        private void ScrollToItem(int i)
        {
            listConceptions.SelectedIndex = i;
            listConceptions.ScrollIntoView(listConceptions.SelectedItem);
        }

        private void cmbChangeforEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEditDescription();
        }

        private void btnAddDescription_Click(object sender, RoutedEventArgs e)
        {
            ConceptionDescriptionViewModel conception = (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            TrilingualDictionaryCore.LanguageId languageForEdit = (TrilingualDictionaryCore.LanguageId)cmbChangeforEdit.SelectedIndex;

            ConceptionDescription desc = new ConceptionDescription(conception.ObjectDescription.OwnConception, "");// conception.GetConceptionDescription(languageForEdit);
            desc.LangPart = txtLangPart.Text;
            desc.Topic = txtTopic.Text;
            desc.Semantic = txtSemantic.Text;
            desc.Changeable.Type = txtChangableType.Text;
            desc.Changeable.Value = txtChangable.Text;
            desc.Link = txtLink.Text;

            m_DictionaryViewModel.AddDescriptionToConception(conception.ObjectDescription.OwnConception.ConceptionId, txtEditDescription.Text, languageForEdit);

            UpdateAllControls();
            //listConceptions.Items.Refresh();
        }

        private void btnChangeDescription_Click(object sender, RoutedEventArgs e)
        {
            ConceptionViewModel conception = (ConceptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;


            ConceptionDescription desc = new ConceptionDescription(conception.Conception, "");// conception.GetConceptionDescription(languageForEdit);
            desc.LangPart = txtLangPart.Text;
            desc.Topic = txtTopic.Text;
            desc.Semantic = txtSemantic.Text;
            desc.Changeable.Type = txtChangableType.Text;
            desc.Changeable.Value = txtChangable.Text;
            desc.Link = txtLink.Text;

            m_DictionaryViewModel.ChangeDescriptionOfConception(conception.ConceptionId, txtEditDescription.Text, languageForEdit);

            UpdateAllControls();
            //listConceptions.Items.Refresh();
        }

        private void btnRemoveDescription_Click(object sender, RoutedEventArgs e)
        {
            ConceptionDescriptionViewModel conception = (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;

            m_DictionaryViewModel.RemoveDescriptionFromConception(conception.ObjectDescription.OwnConception.ConceptionId, conception.RegistryDescription, languageForEdit);

            UpdateAllControls();
            //listConceptions.Items.Refresh();
        }

        private void btnAddConception_Click(object sender, RoutedEventArgs e)
        {
            LanguageId languageForEdit = (LanguageId)cmbChangeforEdit.SelectedIndex;

            int newConceptionId = m_DictionaryViewModel.AddConception(txtEditDescription.Text, languageForEdit);

            Conception conception = m_DictionaryViewModel.GetConception(newConceptionId);
            ConceptionDescription desc = conception.GetConceptionDescription(languageForEdit, 0);
            desc.LangPart = txtLangPart.Text;
            desc.Topic = txtTopic.Text;
            desc.Semantic = txtSemantic.Text;
            desc.Changeable.Type = txtChangableType.Text;
            desc.Changeable.Value = txtChangable.Text;
            desc.Link = txtLink.Text;

            m_DictionaryViewModel.Save();


            UpdateAllControls();
            //listConceptions.Items.Refresh();
            SelectItemById(newConceptionId);
        }

        private void btnRemoveConception_Click(object sender, RoutedEventArgs e)
        {
            ConceptionDescriptionViewModel conception = (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            m_DictionaryViewModel.RemoveConception(conception);

            UpdateAllControls();
            //listConceptions.Items.Refresh();
            listConceptions.SelectedIndex = -1;
        }

        private void chkFlatView_Checked(object sender, RoutedEventArgs e)
        {
            listConceptions.Visibility = Visibility.Visible;
            treeConceptions.Visibility = Visibility.Hidden;
        }

        private void chkFlatView_Unchecked(object sender, RoutedEventArgs e)
        {
            listConceptions.Visibility = Visibility.Hidden;
            treeConceptions.Visibility = Visibility.Visible;
        }

        private void treeConceptions_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

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
            if (chkBrackets.IsChecked == true)
                listConceptions.Items.Filter = ConceptionDescriptionViewModel.BracketsrFilter;
            else
                listConceptions.Items.Filter = null;
            UpdateAllControls();
        }

        private void ChkHumanHandled_OnClick(object sender, RoutedEventArgs e)
        {
            ConceptionDescriptionViewModel conception = (ConceptionDescriptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            conception.ObjectDescription.OwnConception.IsHumanHandled = chkHumanHandled.IsChecked == true;
            m_DictionaryViewModel.Save();

            if (chkBrackets.IsChecked == true)
                listConceptions.Items.Filter = ConceptionDescriptionViewModel.BracketsrFilter;
            else
                listConceptions.Items.Filter = null;

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
            if (chkSameDescriptions.IsChecked == true)
            {
                List<ConceptionDescriptionViewModel> temp = new List<ConceptionDescriptionViewModel>();
                List<ConceptionDescriptionViewModel> temp2 = new List<ConceptionDescriptionViewModel>();
                foreach (ConceptionDescriptionViewModel desc in m_DictionaryViewModel.ConceptionDescriptions)
                {
                    var selected = temp.Where(x => x.RegistryDescription == desc.RegistryDescription).ToList();
                    if (selected.Count > 0)
                    {
                        temp2.Add(desc);
                        selected.ForEach(item => temp.Remove(item));
                        temp2.AddRange(selected);
                    }
                    else
                        temp.Add(desc);
                }
                m_DictionaryViewModel.ConceptionDescriptions.Clear();
                temp2.ForEach(item => m_DictionaryViewModel.ConceptionDescriptions.Add(item));
            }
            else
            {
                m_DictionaryViewModel.LoadAllDescriptions();
                UpdateAllControls();
            }
        }
    }
}
