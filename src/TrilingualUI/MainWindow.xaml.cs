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
            InitializeComponent();
            string dataFolder = "Data";
            //string dictionaryDataFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFolder);
            string dictionaryDataFolder = Properties.Settings.Default.DictionaryData;
            m_Dictionary.Load(dictionaryDataFolder);
            listConceptions.ItemsSource = m_Dictionary.GetConceptions();
            //for (int i = 0; i < m_Dictionary.ConceptionsCount; i++)
            //{
            //    Conception copy = m_Dictionary.GetConceptionCopy(i + 1);
            //    listConceptions.Items.Add(copy.GetConceptionDescription(Conception.LanguageId.Russian).ConceptionRegistryDescription)
            //    ;//(copy);
            //    //listConceptions.ItemStringFormat =
            //    //    copy.GetConceptionDescription(Conception.LanguageId.Russian).ConceptionRegistryDescription;
            //}
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
            UpdateDescription();
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
    }
}
