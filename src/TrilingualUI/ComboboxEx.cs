using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace TrilingualUI
{
    public class ComboboxEx : ComboBox
    {
        int m_PreviousSelectedIndex = -1;

        public int PreviousSelectedIndex 
        {
            get{ return m_PreviousSelectedIndex; }
            set{ m_PreviousSelectedIndex = value; }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            PreviousSelectedIndex = SelectedIndex;
        }
    }
}
