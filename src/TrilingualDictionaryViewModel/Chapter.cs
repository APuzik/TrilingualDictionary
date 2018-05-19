using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryViewModel
{
    public class Chapter : IComparable<Chapter>, INotifyPropertyChanged//, ITreeNode
    {
        string m_Letter;
        ObservableCollection<ConceptionDescriptionViewModel> m_Descriptions = new ObservableCollection<ConceptionDescriptionViewModel>();//ObservableCollection<ConceptionDescriptionViewModel>();
        SortedDictionary<int, List<ConceptionDescriptionViewModel>> m_Descs = new SortedDictionary<int, List<ConceptionDescriptionViewModel>>();
        bool m_isExpanded = false;
        bool m_isSelected = false;

        public ObservableCollection<ConceptionDescriptionViewModel> Descriptions
        {
            get { return m_Descriptions; }
        }

        public string Letter
        {
            get { return m_Letter; }
        }

        public string Value
        {
            get { return Letter; }
            set { m_Letter = value; }
        }

        public ConceptionDescriptionViewModel ConceptionDescriptionViewModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Chapter(char c)
        {
            m_Letter = c.ToString().ToUpper();
        }

        public Chapter(string name)
        {
            m_Letter = name;
        }

        public int CompareTo(Chapter other)
        {
            return string.Compare(this.Letter, other.Letter, true);
        }

        ////public void SortChildren()
        ////{
        ////    m_Descriptions.Sort();
        ////}

        public void AddChild(ConceptionDescriptionViewModel newChild)
        {
            int pos = GetInsertPos(newChild.RegistryDescriptionWoAccent, m_Descriptions, 0, m_Descriptions.Count);
            m_Descriptions.Insert(pos, newChild);
            
            int id = newChild.Conception.ConceptionId;
            if (!m_Descs.ContainsKey(id))
                m_Descs.Add(id, new List<ConceptionDescriptionViewModel>());

            m_Descs[id].Add(newChild);
            //for (int i = 0; i < m_Descriptions.Count; i++)
            //{
            //    if (string.Compare(newChild.RegistryDescriptionWoAccent, m_Descriptions[i].RegistryDescriptionWoAccent, true) < 0)
            //    {
            //        m_Descriptions.Insert(i, newChild);
            //        return;
            //    }
            //}

            //m_Descriptions.Add(newChild);
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

            int pos = (end - start)/2 + start;
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
            int id = aChild.Conception.ConceptionId;
            //for (int i = 0; i < m_Descriptions.Count; i++)
            //{
            //    if (id == m_Descriptions[i].Conception.ConceptionId &&
            //        string.Compare(aChild.Value, m_Descriptions[i].Value, true) == 0)
            //    {
            //        m_Descriptions.RemoveAt(i);
            //    }
            //}

            
            if (!m_Descs.ContainsKey(id))
                throw new Exception("id not found in chapter");

            ConceptionDescriptionViewModel found = null;
            for (int j = 0; j < m_Descs[id].Count; j++)
            {
                if (string.Compare(aChild.Value, m_Descs[id][j].Value, true) == 0)
                {
                    found = m_Descs[id][j];
                    m_Descs[id].RemoveAt(j);
                    break;
                }
            }
            if(found != null)
                m_Descriptions.Remove(found);
        }

        public void ChangeChildValue(ITreeNode node, string newValue)
        {
            for (int i = 0; i < m_Descriptions.Count; i++)
            {
                //if (string.Compare(node.Value, m_Descriptions[i].RegistryDescriptionWoAccent, true) == 0)
                //    m_Descriptions[i].
            }
        }


        //private int Insert(ITreeNode node, int index)
        //{
        //    if( m_Descriptions[index]
        //}

        internal List<ConceptionDescriptionViewModel> GetDescVM(int conceptionId)
        {
            return m_Descs[conceptionId];
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
    }
}
