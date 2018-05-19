using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for ChangeTranslatableWnd.xaml
    /// </summary>
    public partial class ChangeTranslatableWnd : Window
    {
        ObservableCollection<TranslationVM> m_translatables = null;
        ITranslationFactory m_factory = null;
        TrilingualDictionaryViewModel.TrilingualDictionaryViewModel m_VM = null;

        public ChangeTranslatableWnd(ObservableCollection<TranslationVM> observableCollection, ITranslationFactory factory, 
            TrilingualDictionaryViewModel.TrilingualDictionaryViewModel vm)
        {
            m_translatables = observableCollection;
            m_factory = factory;
            m_VM = vm;

            InitializeComponent();
            SwitchDataContext();

            ICollectionView view = CollectionViewSource.GetDefaultView(m_translatables);//lstItems.ItemsSource);
            view.Filter = FilterFirstLast;
        }

        private void SwitchDataContext()
        {
            DataContext = new
            {
                ActiveTranslatables = this.ActiveTranslatables
            };
        }

        public ObservableCollection<TranslationVM> ActiveTranslatables                                                   
        {
            get { return m_translatables; }
        }

        private bool FilterFirstLast(object item)
        {
            TranslationVM transl = item as TranslationVM;
            return !transl.IsAddItem && !string.IsNullOrEmpty(transl.Translation);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtSelectedItem.Text))
            {
                MessageBox.Show("Введите название");
                txtSelectedItem.Focus();
            }

            TranslationVM newItem = new TranslationVM(m_factory.CreateObject(txtSelectedItem.Text), true);
            newItem.Item.LangForId = m_VM.MainLanguage;
            newItem.Item.Id = newItem.Item.SaveToDB(m_VM.MainLanguage, false);

            m_translatables.Insert(m_translatables.Count - 1, newItem);
            lstItems.SelectedItem = newItem;
            txtSelectedItem.Text = newItem.Translation;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSelectedItem.Text))
            {
                MessageBox.Show("Введите название");
                txtSelectedItem.Focus();
            }

            TranslationVM tr = lstItems.SelectedItem as TranslationVM;
            //PartOfSpeechTranslation ptr = tr.Item as PartOfSpeechTranslation;
            if (tr != null)
            {
                tr.Translation = txtSelectedItem.Text;
                tr.Item.SaveToDB(m_VM.MainLanguage, tr.IsAbsent);
                tr.IsAbsent = false;
                txtSelectedItem.Text = tr.Translation;
                CollectionViewSource.GetDefaultView(m_translatables).Refresh();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if(lstItems.SelectedIndex < 0)
                return;
            MessageBoxResult res = MessageBox.Show(string.Format("Вы уверены, что хотите удалить служебный термин '{0}'?", txtSelectedItem.Text), "Подтвердите удаление", 
                MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                TranslationVM tr = lstItems.SelectedItem as TranslationVM;
                bool isItemDeleted = tr.Item.DeleteFromDB();
                if (isItemDeleted)
                    m_translatables.RemoveAt(lstItems.SelectedIndex + 1);
                else 
                {
                    tr.Item.GetAnyTranslation();
                    tr.IsAbsent = true;
                    txtSelectedItem.Text = tr.Translation;
                }
                CollectionViewSource.GetDefaultView(m_translatables).Refresh();
            }            
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TranslationVM trans = lstItems.SelectedItem as TranslationVM;
            if (trans != null)
                txtSelectedItem.Text = trans.Translation;
            else
                txtSelectedItem.Text = "";
        }
    }
}
