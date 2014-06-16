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
            listConceptions.Items.Refresh();
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
            btnChangeDescription.IsEnabled = isDescriptionExists;
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
            return conception.GetConceptionDescription(languageForDescription).ConceptionRegistryDescription;
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
        }

        private void btnChangeDescription_Click(object sender, RoutedEventArgs e)
        {
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
                return;

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            m_Dictionary.ChangeDescriptionOfConception(conception.ConceptionId, txtEditDescription.Text, languageForEdit);
            
            UpdateAllControls();
        }

        private void btnRemoveDescription_Click(object sender, RoutedEventArgs e)
        {
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
                return;

            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            m_Dictionary.RemoveDescriptionFromConception(conception.ConceptionId, languageForEdit);
            
            UpdateAllControls();
        }

        private void btnAddConception_Click(object sender, RoutedEventArgs e)
        {
            Conception.LanguageId languageForEdit = (Conception.LanguageId)cmbChangeforEdit.SelectedIndex;
            m_Dictionary.AddConception(txtEditDescription.Text, languageForEdit);
            
            UpdateAllControls();
        }

        private void btnRemoveConception_Click(object sender, RoutedEventArgs e)
        {
            Conception conception = (Conception)listConceptions.SelectedItem;
            if (conception == null)
                return;

            m_Dictionary.RemoveConception(conception.ConceptionId);
            listConceptions.SelectedIndex = -1;
            UpdateAllControls();
        }
    }
}
