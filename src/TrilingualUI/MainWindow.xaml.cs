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
            string dictionaryDataFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFolder);
            m_Dictionary.Load(dictionaryDataFolder);
            for (int i = 0; i < m_Dictionary.ConceptionsCount; i++)
            {
                Conception copy = m_Dictionary.GetConceptionCopy(i + 1);
                listConceptions.Items.Add(copy.GetConceptionDescription(Conception.LanguageId.Russian).ConceptionRegistryDescription)
                ;//(copy);
                //listConceptions.ItemStringFormat =
                //    copy.GetConceptionDescription(Conception.LanguageId.Russian).ConceptionRegistryDescription;
            }
        }

        private void chkAllLanguages_Checked(object sender, RoutedEventArgs e)
        {
            cmbLanguageToSelect.IsEnabled = chkAllLanguages.IsChecked != true;
        }
    }
}
