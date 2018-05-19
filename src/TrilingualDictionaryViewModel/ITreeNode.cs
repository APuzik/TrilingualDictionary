using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryViewModel
{
    public interface ITreeNode
    {
        string Value { get; }
        void SortChildren();
        void AddChild(ITreeNode node);
        void RemoveChild(ITreeNode node);
        void ChangeChildValue(ITreeNode node, string newValue);
        //void AddSibling(ITreeNode node);
        //ITreeNode FindValue();
    }
}
