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
using System.Windows.Shapes;
using TrilingualDictionaryCore;

namespace TrilingualUI
{
    /// <summary>
    /// Interaction logic for AddConception.xaml
    /// </summary>
    public partial class AddConception : Window
    {
        public Conception m_NewConception = null;
        TrilingualDictionaryViewModel.TrilingualDictionaryViewModel m_Dictionary = null;
        public AddConception(TrilingualDictionaryViewModel.TrilingualDictionaryViewModel dictionary)
        {
            InitializeComponent();
            m_Dictionary = dictionary;
            btnAddDescription.IsEnabled = true;
        }

        private void btnAddDescription_Click(object sender, RoutedEventArgs e)
        {
            int id = m_Dictionary.AddConception(txtEditDescription.Text, (LanguageId)cmbChangeforEdit.SelectedIndex);
            m_NewConception = m_Dictionary.GetConception(id);
            m_NewConception.ParentId = 3;
            m_NewConception.ParentConception = m_Dictionary.GetConception(m_NewConception.ParentId);
            btnChangeDescription.IsEnabled = true;
            btnRemoveDescription.IsEnabled = true;
        }

        private void btnChangeDescription_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemoveDescription_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
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

        }
    }
}
