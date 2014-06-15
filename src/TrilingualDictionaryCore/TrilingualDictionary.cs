using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public class TrilingualDictionary
    {
        private Dictionary<int, Conception> m_Dictionary = new Dictionary<int, Conception>();

        public int AddConception(string word, Conception.LanguageId languageId)
        {
            int newConceptionId = m_Dictionary.Count > 0 ? m_Dictionary.Last().Key + 1 : 1;
            Conception newConception = new Conception(newConceptionId, word, languageId);
            m_Dictionary.Add(newConception.ConceptionId, newConception);
            return newConception.ConceptionId;
        }

        public void AddDescriptionToConception(int conceptionId, string word, Conception.LanguageId languageId)
        {
            GetConception(conceptionId).AddDescription(word, languageId);
        }

        public void ChangeDescriptionOfConception(int conceptionId, string word, Conception.LanguageId languageId)
        {
            GetConception(conceptionId).ChangeDescription(word, languageId);
        }

        public void RemoveDescriptionFromConception(int conceptionId, Conception.LanguageId languageId)
        {
            Conception handledConception = GetConception(conceptionId);
            handledConception.RemoveDescription(languageId);

            if (handledConception.DescriptionsCount == 0)
                RemoveConception(conceptionId);
        }

        public void RemoveConception(int conceptionId)
        {
            m_Dictionary.Remove(conceptionId);
        }

        public int ConceptionsCount
        {
            get { return m_Dictionary.Count; }
        }

        private Conception GetConception(int conceptionId)
        {
            return m_Dictionary[conceptionId];
        }

        public Conception GetConceptionCopy(int conceptionId)
        {
            return m_Dictionary[conceptionId];
        }

        public IEnumerable<Conception> GetConceptions()
        {
            return m_Dictionary.Values;
        }

        public void Load(string dictionaryDataFolder)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dictionaryDataFolder);

            if (dirInfo.Exists)
            {
                IEnumerable<FileInfo> files = dirInfo.EnumerateFiles();
                foreach (FileInfo file in files)
                {
                    LoadFromFile(file.FullName);
                }
            }
        }

        private void LoadFromFile(string pathToFile)
        {
            try
            {
                string[] lines = File.ReadAllLines(pathToFile);
                int baseId = m_Dictionary.Count;
                string mainword = "";
                for (int i = 0; i < lines.Length; i += 2)
                {
                    int curId = baseId + i / 2 + 1;
                    int startPos = 2;
                    string text = lines[i].Substring(startPos);
                    List<string> words = SplitText(text);
                    if (!text.Contains('~') && !IsMainWord(words))
                    {
                        int a = 2;
                        a++;
                    }
                    Conception conception = new Conception(curId, text, Conception.LanguageId.Russian);

                    text = lines[i + 1].Substring(startPos);
                    words = SplitText(text);
                    conception.AddDescription(text, Conception.LanguageId.Ukrainian);
                    m_Dictionary.Add(conception.ConceptionId, conception);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private bool IsMainWord(List<string> words)
        {
            if(words.Count == 1 &&
                (words[0].IndexOfAny(new char[] { '[', ']', '(', ')' }) == -1 || !words[0].Contains(' ')))
            {
                return true;
            }

            int mainCount = 0;
            foreach (string word in words)
            {
                
                int index = word.IndexOfAny(new char[] { '[', ']', '(', ')' });
                if ( index == -1)
                {
                    mainCount++;
                    if (mainCount > 1)
                        return false;
                }
            }

            return mainCount == 1;
        }

        public static List<string> SplitText(string text)
        {
            try
            {
                Stack<char> brackets = new Stack<char>();
                Tuple<char, char> br1 = new Tuple<char, char>('[', ']');
                Tuple<char, char> br2 = new Tuple<char, char>('(', ')');
                StringBuilder sb = new StringBuilder();
                List<string> words = new List<string>();
                foreach (char c in text)
                {
                    if (Char.IsWhiteSpace(c) && brackets.Count == 0)
                    {
                        words.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        if (c == '[' || c == '(')
                            brackets.Push(c);

                        if (c == ']' || c == ')')
                        {
                            checkBrackets(brackets, c, br1);
                            checkBrackets(brackets, c, br2);
                        }

                        sb.Append(c);
                    }
                }
                words.Add(sb.ToString());
                if (brackets.Count > 0)
                {
                    int k = 1;
                    File.AppendAllText(@"D:\AS\_Aspirantura\Projects\TrilingualDictionary\Data\ex.txt",
                        string.Format("{0}\r\n", text), Encoding.Unicode);
                }
                return words;
            }
            catch (Exception ex)
            {
                int j = 1;
                File.AppendAllText(@"D:\AS\_Aspirantura\Projects\TrilingualDictionary\Data\ex.txt",
                        string.Format("{0}\r\n", text), Encoding.Unicode);

                return null;
            }
        }

        private static void checkBrackets(Stack<char> brackets, char c, Tuple<char, char> bracketsStartEnd)
        {
            if (c == bracketsStartEnd.Item2)
            {
                if (brackets.Peek() == bracketsStartEnd.Item1)
                    brackets.Pop();
                else
                    throw new Exception("Incorrect brackets");
            }
        }

        public Conception.LanguageId MainLanguage 
        {
            get { return Conception.MainLanguage; }
            set { Conception.MainLanguage = value; }
        }
    }
}
