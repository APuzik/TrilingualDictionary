using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultilingualDictionary
{
    /// <summary>
    /// Interaction logic for EditTerm.xaml
    /// </summary>
    public partial class EditTerm : Window
    {
        public EditTerm()
        {
            InitializeComponent();
        }

        private void parentTerm_DropDownOpened(object sender, EventArgs e)
        {
            var textbox = (TextBox)parentTerm.Template.FindName("PART_EditableTextBox", parentTerm);
            if (textbox != null && textbox.SelectionLength > 0)
            {
                textbox.Select(textbox.SelectionLength, 0);
            }
        }

        private void parentTerm_LostFocus(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)parentTerm.Template.FindName("PART_EditableTextBox", parentTerm);
            if (textbox != null)
            {
                foreach (var tt in parentTerm.Items)
                {
                    if (string.Equals(tt.ToString(), textbox.Text))
                    {
                        parentTerm.SelectedItem = tt;
                        return;
                    }
                }
            }
        }

        private void parentTerm_KeyUp(object sender, KeyEventArgs e)
        {
            ((MultiDictionaryViewModel.TermVM)parentTerm.DataContext).ResetParentTranslation();
        }
    }
}
