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

                listConceptions.ItemsSource = CollectionViewSource.GetDefaultView(m_DictionaryViewModel.Conceptions); 
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
            m_DictionaryViewModel.MainLanguage = (Conception.LanguageId)cmbLanguages.SelectedIndex;
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
            UpdateEditDescription();
            lblCount.Content = string.Format("{0} терминов", listConceptions.Items.Count);
            lblCount.UpdateLayout();
            listConceptions.UpdateLayout();// Items.Refresh();
        }

        private void UpdateDescription()
        {
            ConceptionViewModel conception = (ConceptionViewModel) listConceptions.SelectedItem;
            if (conception == null)
                return;

            if (chkAllLanguages.IsChecked == true)
                txtDescription.Text = GetAllDescriptions(conception);
            else
            {
                Conception.LanguageId languageForDescription =
                    (Conception.LanguageId) cmbLanguageForDescription.SelectedIndex;
                txtDescription.Text = GetConceptionDescription(conception, languageForDescription);
            }

            chkHumanHandled.IsChecked = conception.Conception.IsHumanHandled;
        }


        private void UpdateEditDescription()
        {
            ConceptionViewModel conception = (ConceptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
            {
                if (btnRemoveConception != null)
                    btnRemoveConception.IsEnabled = false;
                return;
            }                      

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            txtEditDescription.Text = GetConceptionDescription(conception, languageForEdit);
            
            ConceptionDescription description = conception.GetConceptionDescriptionOrEmpty(languageForEdit);
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

        private string GetAllDescriptions(ConceptionViewModel conception)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Conception.LanguageId lang in Enum.GetValues(typeof(Conception.LanguageId)))
            {
                sb.AppendLine(GetConceptionDescription(conception, lang));
            }
            return sb.ToString();
        }

        private string GetConceptionDescription(ConceptionViewModel conception, Conception.LanguageId languageForDescription)
        {
            return conception.GetConceptionRegistryDescriptionOrEmpty(languageForDescription);
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
                ConceptionViewModel conception = (ConceptionViewModel)listConceptions.Items[i];
                isFound = conception.Find(textToSearch);
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
                if( id == conception.ConceptionId)
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
            ConceptionViewModel conception = (ConceptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            
            ConceptionDescription desc = conception.GetConceptionDescription(languageForEdit);
            desc.LangPart = txtLangPart.Text;
            desc.Topic = txtTopic.Text;
            desc.Semantic = txtSemantic.Text;
            desc.Changeable.Type = txtChangableType.Text;
            desc.Changeable.Value = txtChangable.Text;
            desc.Link = txtLink.Text;

            m_DictionaryViewModel.AddDescriptionToConception(conception.ConceptionId, txtEditDescription.Text, languageForEdit);
            
            UpdateAllControls();
            //listConceptions.Items.Refresh();
        }

        private void btnChangeDescription_Click(object sender, RoutedEventArgs e)
        {
            ConceptionViewModel conception = (ConceptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;


            ConceptionDescription desc = conception.GetConceptionDescription(languageForEdit);
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
            ConceptionViewModel conception = (ConceptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;

            m_DictionaryViewModel.RemoveDescriptionFromConception(conception.ConceptionId, languageForEdit);

            UpdateAllControls();
            //listConceptions.Items.Refresh();
        }

        private void btnAddConception_Click(object sender, RoutedEventArgs e)
        {
            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;

            int newConceptionId = m_DictionaryViewModel.AddConception(txtEditDescription.Text, languageForEdit);

            Conception conception = m_DictionaryViewModel.GetConception(newConceptionId);
            ConceptionDescription desc = conception.GetConceptionDescription(languageForEdit);
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
            ConceptionViewModel conception = (ConceptionViewModel)listConceptions.SelectedItem;
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
                listConceptions.Items.Filter = ConceptionViewModel.BracketsrFilter;
            else
                listConceptions.Items.Filter = null;
            UpdateAllControls();
        }

        private void ChkHumanHandled_OnClick(object sender, RoutedEventArgs e)
        {
            ConceptionViewModel conception = (ConceptionViewModel)listConceptions.SelectedItem;
            if (conception == null)
                return;

            conception.Conception.IsHumanHandled = chkHumanHandled.IsChecked == true;
            m_DictionaryViewModel.Save();

            if (chkBrackets.IsChecked == true)
                listConceptions.Items.Filter = ConceptionViewModel.BracketsrFilter;
            else
                listConceptions.Items.Filter = null;

            UpdateAllControls();
        }

        private void btnSplitConception_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
