using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryViewModel
{
    //public class ConceptionDescriptionViewModelEqualityComparer : IEqualityComparer<ConceptionDescriptionViewModel>
    //{
    //    public bool Equals(ConceptionDescriptionViewModel x, ConceptionDescriptionViewModel y)
    //    {
    //        return x.Conception.ConceptionId == y.Conception.ConceptionId;
    //    }

    //    public int GetHashCode(ConceptionDescriptionViewModel x)
    //    {
    //        return x.Conception.ConceptionId;
    //    }
    //}

    public class ConceptionDescriptionViewModel : INotifyPropertyChanged
    {
        ObservableCollection<ConceptionDescriptionViewModel> m_Children = null;
        ConceptionDescription m_Description = null;
        ConceptionDescriptionViewModel m_Parent = null;
        bool m_isExpanded = false;
        bool m_isSelected = false;

        public ConceptionDescriptionViewModel(ConceptionDescription desc, ConceptionDescriptionViewModel parent)
        {
            m_Description = desc;
            m_Children = new ObservableCollection<ConceptionDescriptionViewModel>();
            m_Parent = parent;
        }

        public string ConceptionRegistryDescription
        {
            get { return m_Description.ConceptionRegistryDescription; }
        }

        public ObservableCollection<ConceptionDescriptionViewModel> Children
        {
            get { return m_Children; }
        }

        public bool IsAbsent
        {
            get;
            set;
        }

        public Conception Conception 
        {
            get { return m_Description.OwnConception; }
        }

        public string RegistryDescriptionWoAccent
        {
            get { return m_Description.ConceptionRegistryDescriptionWoAccents; }
        }

        public void AddChild(ConceptionDescriptionViewModel aChild)
        {
            int pos = GetInsertPos(aChild.RegistryDescriptionWoAccent, m_Children, 0, m_Children.Count);
            m_Children.Insert(pos, aChild);
            aChild.Parent = this;
            //for (int i = 0; i < m_Children.Count; i++)
            //{
            //    if (string.Compare(aChild.Value, m_Children[i].Value, true) < 0)
            //    {
            //        m_Children.Insert(i, aChild);
            //        return;
            //    }
            //}

            //m_Children.Add(aChild);
        }

        private int GetInsertPos(string value, Collection<ConceptionDescriptionViewModel> list, int start, int end)
        {
            if (list.Count == 0)
                return 0;

            if (list.Count == start)
                return start;

            if (start >= end)
            {
                int res1 = string.Compare(value, list[start].RegistryDescriptionWoAccent, true);
                if (res1 < 0)
                {
                    return start;
                }
                else
                {
                    return start + 1;
                }
            }

            int pos = (end - start) / 2 + start;
            int res = string.Compare(value, list[pos].RegistryDescriptionWoAccent, true);
            if (res < 0)
            {
                return GetInsertPos(value, list, start, pos - 1);
            }
            else if (res > 0)
            {
                return GetInsertPos(value, list, pos + 1, end);
            }
            else
                return pos;
        }

        public void RemoveChild(ConceptionDescriptionViewModel aChild)
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                if (aChild.Conception.ConceptionId == m_Children[i].Conception.ConceptionId &&
                    string.Compare(aChild.Value, m_Children[i].Value, true) == 0)
                {
                    m_Children.RemoveAt(i);
                    break;
                }
            }
        }
        //public void ChangeChildValue(ITreeNode node, string newValue)
        //{
        //    for (int i = 0; i < m_Children.Count; i++)
        //    {
        //        //if (string.Compare(node.Value, m_Descriptions[i].RegistryDescriptionWoAccent, true) == 0)
        //        //    m_Descriptions[i].
        //    }
        //}
        public void SortChildren()
        { }

        public string Value
        {
            get { return m_Description.ConceptionRegistryDescription; }
            //set { m_Description.ConceptionRegistryDescription = value; }
        }

        public ConceptionDescriptionViewModel Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public ConceptionDescription ConceptionDescription
        {
            get { return m_Description; }
        }

        public bool IsExpanded
        {
            get { return m_isExpanded; }
            set
            {
                if (value != m_isExpanded)
                {
                    m_isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (m_isExpanded && m_Parent != null)
                    m_Parent.IsExpanded = true;
            }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (value != m_isSelected)
                {
                    m_isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        internal bool ContainsText(string textToSearch)
        {
            if (m_Description.DescriptionId == 22131 || m_Description.OwnConception.ConceptionId == 11065)
            {
                int j = 1;
            }

            if (textToSearch.Contains('#'))
            {
                
                return m_Description.ConceptionRegistryDescription.Contains(textToSearch);
            }
            else
                return m_Description.ConceptionRegistryDescriptionWoAccents.Contains(textToSearch);
        }

        //public static bool GetBringIntoViewWhenSelected(TreeViewItem treeViewItem)
        //{
        //    return (bool)treeViewItem.GetValue(BringIntoViewWhenSelectedProperty);
        //}

        //public static void SetBringIntoViewWhenSelected(TreeViewItem treeViewItem, bool value)
        //{
        //    treeViewItem.SetValue(BringIntoViewWhenSelectedProperty, value);
        //}

        //public static readonly DependencyProperty BringIntoViewWhenSelectedProperty =
        //    DependencyProperty.RegisterAttached("BringIntoViewWhenSelected", typeof(bool),
        //    typeof(TreeViewItemBehavior), new UIPropertyMetadata(false, OnBringIntoViewWhenSelectedChanged));

        //static void OnBringIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        //{
        //    TreeViewItem item = depObj as TreeViewItem;
        //    if (item == null)
        //        return;

        //    if (e.NewValue is bool == false)
        //        return;

        //    if ((bool)e.NewValue)
        //        item.BringIntoView();
        //}
    }

    //public class ConceptionDescriptionViewModel
    //{
    //    //public class ConceptionDescriptionComparer : IComparer
    //    //{
    //    //    private LanguageId m_LanguageId;

    //    //    public ConceptionDescriptionComparer(LanguageId languageId)
    //    //    {
    //    //        m_LanguageId = languageId;
    //    //    }

    //    //    public int Compare(ConceptionDescription desc1, ConceptionDescription desc2)
    //    //    {
    //    //        try
    //    //        {
    //    //            Conception x = desc1.OwnConception;
    //    //            Conception y = desc2.OwnConception;
    //    //            if (x.ParentConception != null && y.ParentConception != null && x.ParentConception != y.ParentConception)
    //    //            {
    //    //                return String.Compare(x.GetParentNameForSorting(m_LanguageId), y.GetParentNameForSorting(m_LanguageId), StringComparison.InvariantCultureIgnoreCase);
    //    //            }
    //    //            else
    //    //            {
    //    //                return String.Compare(desc1.ConceptionSortDescription, desc2.ConceptionSortDescription, StringComparison.InvariantCultureIgnoreCase);
    //    //            }

    //    //        }
    //    //        catch (Exception ex)
    //    //        {

    //    //            throw;
    //    //        }
    //    //    }

    //    //    public int Compare(object x, object y)
    //    //    {
    //    //        ConceptionDescriptionViewModel x1 = x as ConceptionDescriptionViewModel;
    //    //        ConceptionDescriptionViewModel y1 = y as ConceptionDescriptionViewModel;
    //    //        return Compare(x1.ObjectDescription, y1.ObjectDescription);
    //    //    }
    //    //};

    //    private ConceptionDescription m_Description;
    //    private bool m_isDecriptionMatchesLanguage;

    //    public ConceptionDescriptionViewModel(ConceptionDescription desc, bool isDecriptionMatchesLanguage)
    //    {            
    //        m_Description = desc;
    //        m_isDecriptionMatchesLanguage = isDecriptionMatchesLanguage;
    //    }
    //    public ConceptionDescription ObjectDescription
    //    {
    //        get { return m_Description; }
    //    }

    //    public string RegistryDescription
    //    {
    //        get { return m_Description.ConceptionRegistryDescription; }
    //    }

    //    public bool IsDescriptionMatchesLanguage
    //    {
    //        get { return m_isDecriptionMatchesLanguage; }
    //    }

    //    public string RegistryDescriptionWoAccent
    //    {
    //        get { return m_Description.ConceptionRegistryDescriptionWoAccents; }
    //    }

    //    //public static bool BracketsrFilter(object item)
    //    //{
    //    //    ConceptionDescriptionViewModel conceptionDescription = item as ConceptionDescriptionViewModel;

    //    //    return conceptionDescription.ObjectDescription.OwnConception.IsPotentialUndefinded();
    //    //}

    //    //public static bool SameDescriptionsFilter(object item)
    //    //{
    //    //    ConceptionDescriptionViewModel conceptionDescription = item as ConceptionDescriptionViewModel;

    //    //    return conceptionDescription.ObjectDescription.OwnConception.IsPotentialUndefinded();
    //    //}
    //}
}
