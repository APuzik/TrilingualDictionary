using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryViewModel
{
    public class SortedObservableCollection<T> : ObservableCollection<ITreeNode>
    {
        public void Sort()
        {
            QuickSort(0, Items.Count-1);

        }

        void QuickSort(int l, int r)
        {
            ITreeNode temp;
            ITreeNode x = Items[l + (r - l) / 2];
            //запись эквивалентна (l+r)/2,
            //но не вызввает переполнения на больших данных
            int i = l;
            int j = r;
            //код в while обычно выносят в процедуру particle
            while (i <= j)
            {
                while (string.Compare(Items[i].Value, x.Value, true) < 0)
                    i++;
                while (string.Compare(Items[j].Value, x.Value, true) > 0)
                    j--;

                if (i <= j)
                {
                    temp = Items[i];
                    Items[i] = Items[j];
                    Items[j] = temp;
                    i++;
                    j--;
                }
            }
            if (i < r)
                QuickSort(i, r);

            if (l < j)
                QuickSort(l, j);
        }
    }
}
