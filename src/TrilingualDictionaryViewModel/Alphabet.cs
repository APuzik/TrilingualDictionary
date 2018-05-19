using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryViewModel
{
    public class Alphabet
    {
        ObservableCollection<Chapter> m_Letters = new ObservableCollection<Chapter>();
        Dictionary<char, Chapter> m_dicLetters = new Dictionary<char, Chapter>();
        Chapter m_Absent = new Chapter("Отсутствует"); 
        LanguageId m_Language;

        public Alphabet(LanguageId lang)
        {
            m_Language = lang;
            m_Letters.Add(m_Absent);
        }

        public bool Contains(char c)
        {
            Chapter aChapter = new Chapter(c);
            return Contains(aChapter);
        }

        public bool Contains(Chapter aChapter)
        {
            return m_Letters.Contains(aChapter);
        }

        public Chapter Find(char c)
        {
            c = Char.ToUpper(c);
            if (m_dicLetters.ContainsKey(c))
                return m_dicLetters[c];

            return null;
        }

        public Chapter Find(string chapterName)
        {
            Chapter aChapter = m_Letters.FirstOrDefault(x =>
                x.Letter == chapterName);
            return aChapter;
        }

        public Chapter Add(char c)
        {
            c = Char.ToUpper(c);
            Chapter aChapter = new Chapter(c);
            m_Letters.Add(aChapter);
            if (!m_dicLetters.ContainsKey(c))
                m_dicLetters[c] = aChapter;

            return aChapter;
        }

        //public void Add(Chapter aChapter)
        //{
        //    m_Letters.Add(aChapter);
        //}

        public LanguageId Language
        {
            get { return m_Language; }
        }

        public ObservableCollection<Chapter> Letters
        {
            get { return m_Letters; }
            set { m_Letters = value; }
        }

        public Chapter Absent
        {
            get { return m_Absent; }
            set { m_Absent = value; }
        }
    }
}