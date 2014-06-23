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

namespace TrilingualUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrilingualDictionary m_Dictionary = new TrilingualDictionary();

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                string dictionaryDataFolder = Properties.Settings.Default.DictionaryData;
                m_Dictionary.Load(dictionaryDataFolder);
                listConceptions.ItemsSource = m_Dictionary.GetConceptions();
                btnChangeDescription.IsEnabled = false;
                //treeConceptions.ItemsSource = listConceptions.ItemsSource;
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
            m_Dictionary.MainLanguage = (Conception.LanguageId)cmbLanguages.SelectedIndex;
            if (listConceptions != null)
                listConceptions.Items.Refresh();

        }

        private void listConceptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAllControls();
        }

        private void UpdateAllControls()
        {            
            UpdateDescription();
            UpdateEditDescription();

            listConceptions.UpdateLayout();// Items.Refresh();
        }

        private void UpdateDescription()
        {
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
                return;

            if (chkAllLanguages.IsChecked == true)
                txtDescription.Text = GetAllDescriptions(conception);
            else
            {
                Conception.LanguageId languageForDescription = (Conception.LanguageId)cmbLanguageForDescription.SelectedIndex;
                txtDescription.Text = GetConceptionDescription(conception, languageForDescription);
            }
        }

        private void UpdateEditDescription()
        {
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
            {
                if (btnRemoveConception != null)
                    btnRemoveConception.IsEnabled = false;
                return;
            }                      

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            txtEditDescription.Text = GetConceptionDescription(conception, languageForEdit);

            bool isDescriptionExists = !string.IsNullOrWhiteSpace(txtEditDescription.Text);
            
            btnAddDescription.IsEnabled = !isDescriptionExists;
            btnChangeDescription.IsEnabled = false;// isDescriptionExists;
            btnRemoveDescription.IsEnabled = isDescriptionExists;
            btnRemoveConception.IsEnabled = true;
        }

        private string GetAllDescriptions(Conception conception)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Conception.LanguageId lang in Enum.GetValues(typeof(Conception.LanguageId)))
            {
                sb.AppendLine(GetConceptionDescription(conception, lang));
            }
            return sb.ToString();
        }

        private string GetConceptionDescription(Conception conception, Conception.LanguageId languageForDescription)
        {
            return conception.GetConceptionDescription(languageForDescription, 0).ConceptionRegistryDescription;
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
                Conception conception = (Conception)listConceptions.Items[i];
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
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
                return;

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            m_Dictionary.AddDescriptionToConception(conception.ConceptionId, txtEditDescription.Text, languageForEdit);
            
            UpdateAllControls();
            listConceptions.Items.Refresh();
        }

        private void btnChangeDescription_Click(object sender, RoutedEventArgs e)
        {
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
                return;

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            m_Dictionary.ChangeDescriptionOfConception(conception.ConceptionId, txtEditDescription.Text, languageForEdit);
            
            UpdateAllControls();
            listConceptions.Items.Refresh();
        }

        private void btnRemoveDescription_Click(object sender, RoutedEventArgs e)
        {
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
                return;

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            m_Dictionary.RemoveDescriptionFromConception(conception.ConceptionId, languageForEdit);
            
            UpdateAllControls();
            listConceptions.Items.Refresh();
        }

        private void btnAddConception_Click(object sender, RoutedEventArgs e)
        {
            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            m_Dictionary.AddConception(txtEditDescription.Text, languageForEdit);
            
            UpdateAllControls();
            listConceptions.Items.Refresh();
        }

        private void btnRemoveConception_Click(object sender, RoutedEventArgs e)
        {
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
                return;

            m_Dictionary.RemoveConception(conception.ConceptionId);
            listConceptions.SelectedIndex = -1;
            UpdateAllControls();
            listConceptions.Items.Refresh();
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
    }
}
