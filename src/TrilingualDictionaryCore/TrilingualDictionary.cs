using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace TrilingualDictionaryCore
{
    public class TrilingualDictionary
    {
        private int m_LastId = 0;
        private Dictionary<int, int> m_AvailableIds = new Dictionary<int, int>();
        private Dictionary<int, Conception> m_Dictionary = new Dictionary<int, Conception>();
        
        private static readonly string[] m_TopicMarks = new string[]
        {
            "бион.",//бионика                                             		
            "біон.",//бионика
            "вчт", //вычислительная тех-ника
            "кв. рф", //квантовая радиофизи-ка
            "кв. эл.", //квантовая электрони-ка
            "кв. ел.", //квантовая электрони-ка
            "крист.", //кристаллография 
            "магн.", //магнетизм
            "мат.", //математика
            "микр.", //микроэлектроника
            "мікр.", //микроэлектроника
            "опт.", //оптика
            "пп", //полупроводники
            "рлк", //радиолокация
            "рф", //радиофизика
            "сп", //сверхпроводники, сверхпроводимость
            "техн.", //техника
            "тлв", //телевидение
            "тлг", //телеграфия
            "тлф", //телефония
            "тн", //теория надёжности
            "физ.", //физика
            "фіз.", //физика
            "фтт" //физика твёрдого тела
        };

        private static readonly string[] m_ChangeableMarks = new string[]
        {
            "род.", //родительный
        };

        private static readonly string[] m_LangMarks = new string[]
        {
            "прил.", //прилагательное
            "прич.", //причастие
            "сущ.", //существительное
        };

        private static readonly string[] m_LinkMarks = new string[]
        {
            "см.", //смотри
//            "см. еще", //смотри ещё
//            "см. ещё", //смотри ещё
        };

        private static readonly string[] m_OtherMarks = new string[]
        {
            "напр.", //например
            "собир.", //собирательное
        };

        public static string[] TopicMarks
        {
            get { return m_TopicMarks; }
        }

        public static string[] ChangeableMarks
        {
            get { return m_ChangeableMarks; }
        }

        public static string[] LangMarks
        {
            get { return m_LangMarks; }
        }

        public static string[] LinkMarks
        {
            get { return m_LinkMarks; }
        }

        public static string[] OtherMarks
        {
            get { return m_OtherMarks; }
        }

        public int AddConception(string word, Conception.LanguageId languageId)
        {            
            lock (this)
            {
                int newConceptionId = m_LastId + 1;
                if (m_AvailableIds.Count > 0)
                {
                    newConceptionId = m_AvailableIds.First().Key;
                    m_AvailableIds.Remove(newConceptionId);
                }

                Conception newConception = new Conception(newConceptionId, word, languageId);
                m_Dictionary.Add(newConception.ConceptionId, newConception);
                m_LastId = newConceptionId;
                return newConceptionId;
            }            
        }

        public void AddDescriptionToConception(int conceptionId, string word, Conception.LanguageId languageId)
        {
            GetConception(conceptionId).AddDescription(word, languageId);
        }

        public void ChangeDescriptionOfConception(int conceptionId, string word, Conception.LanguageId languageId)
        {            
         //   GetConception(conceptionId).ChangeDescription(word, languageId, pos);
        }

        public void RemoveDescriptionFromConception(int conceptionId, Conception.LanguageId languageId)
        {
            Conception handledConception = GetConception(conceptionId);
            handledConception.RemoveAllDescriptions(languageId);

            if (handledConception.DescriptionsCount == 0)
                RemoveConception(conceptionId);
        }

        public void RemoveConception(int conceptionId)
        {
            lock (this)
            {
                if (m_Dictionary.Keys.Contains(conceptionId))
                {
                    m_AvailableIds.Add(conceptionId, 0);
                    m_Dictionary.Remove(conceptionId);
                }
                else
                    throw new Exception(string.Format("Unable to remove conception. Conception with id ({0}) is absent.", conceptionId));
            }
        }

        public int ConceptionsCount
        {
            get { return m_Dictionary.Count; }
        }

        public Conception GetConception(int conceptionId)
        {
            return m_Dictionary[conceptionId];
        }

        public IEnumerable<Conception> GetConceptions()
        {
            return m_Dictionary.Values;
        }

        public Conception.LanguageId MainLanguage
        {
            get { return Conception.MainLanguage; }
            set { Conception.MainLanguage = value; }
        }

        public void Load(string dictionaryDataFolder)
        {
            PlaintTextDataLoader loader = new PlaintTextDataLoader(this);
            loader.Load(dictionaryDataFolder);
        }
    }

    class PlaintTextDataLoader
    {
        TrilingualDictionary m_Dictionary = null;

        public PlaintTextDataLoader(TrilingualDictionary dictionary)
        {
            m_Dictionary = dictionary;
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
                int baseId = m_Dictionary.ConceptionsCount;
                string mainwordRus = "";
                string mainwordUkr = "";
                int parentId = 0;
                for (int i = 0; i < lines.Length; i += 2)
                {
                    int startPos = 2;
                    string textRus = lines[i].Substring(startPos);
                    string textUkr = lines[i + 1].Substring(startPos);

                    List<string> wordsRus = SplitText(textRus, ";");
                    List<string> wordsUkr = SplitText(textUkr, ";");

                    if (wordsRus.Count > 1)
                    {
                        int k = 1;
                        //something wrong
                    }

                    string topicPartOld = "";
                    string topicPartExOld = "";
                    Conception oldConception = null;
                    bool isFirst = true;
                    foreach (string partUkr in wordsUkr)
                    {
                        string clearUkr = partUkr;

                        string topicPart = ExtractTopics(clearUkr);
                        if (!string.IsNullOrWhiteSpace(topicPart))
                            clearUkr = partUkr.Replace(topicPart, "").Trim();

                        string changeAble = ExtractChangableUkr(clearUkr);
                        if (!string.IsNullOrWhiteSpace(changeAble))
                            clearUkr = clearUkr.Replace(changeAble, "").Trim();

                        string langPart = ExtractLangPart(clearUkr);
                        if (!string.IsNullOrWhiteSpace(langPart))
                            clearUkr = clearUkr.Replace(langPart, "").Trim();

                        string linkPart = ExtractLinkPart(clearUkr);
                        if (!string.IsNullOrWhiteSpace(linkPart))
                            clearUkr = clearUkr.Replace(linkPart, "").Trim();

                         string topicPartEx = ExtractParentheses(textRus, clearUkr);
                        if (!string.IsNullOrWhiteSpace(topicPartEx))
                            clearUkr = clearUkr.Replace(topicPartEx, "").Trim();

                        if (!string.IsNullOrWhiteSpace(topicPartEx))
                        {
                            topicPartExOld = topicPartEx;
                        }
                        //else
                        //{
                        //    topicPartEx = topicPartExOld;
                        //}

                        if (!string.IsNullOrWhiteSpace(topicPart))
                        {
                            topicPartOld = topicPart;
                        }
                        //else
                        //{
                        //    topicPart = topicPartOld;
                        //}
                        if (!textRus.Contains('~'))
                        {
                            parentId = 0;
                        }

                        if (!string.IsNullOrWhiteSpace(topicPart) ||
                            !string.IsNullOrWhiteSpace(topicPartEx) ||
                            isFirst)
                        {
                            int n = m_Dictionary.AddConception(textRus, Conception.LanguageId.Russian);
                            m_Dictionary.AddDescriptionToConception(n, clearUkr, Conception.LanguageId.Ukrainian);

                            Conception conception = m_Dictionary.GetConception(n);
                            conception.Topic = topicPart;
                            conception.TopicEx = topicPartEx;
                            conception.Link = linkPart;

                            if (!string.IsNullOrWhiteSpace(changeAble))
                            {
                                conception.GetConceptionDescription(Conception.LanguageId.Ukrainian, 0).Changeable.Type = "род.";
                                conception.GetConceptionDescription(Conception.LanguageId.Ukrainian, 0).Changeable.Value = changeAble;
                            }

                            if (!string.IsNullOrWhiteSpace(langPart))
                            {
                                conception.GetConceptionDescription(Conception.LanguageId.Ukrainian, 0).LangPart = langPart;
                            }

                            if (parentId == 0)
                                parentId = n;
                            else
                                conception.ParentId = parentId;

                            oldConception = conception;
                            isFirst = false;
                        }
                        else
                        {
                            oldConception.AddDescription(clearUkr, Conception.LanguageId.Ukrainian);
                            if (!string.IsNullOrWhiteSpace(changeAble))
                            {
                                oldConception.GetConceptionDescription(Conception.LanguageId.Ukrainian,
                                    oldConception.GetLanguageDescriptionsCount(Conception.LanguageId.Ukrainian) - 1).Changeable.Type = "род.";
                                oldConception.GetConceptionDescription(Conception.LanguageId.Ukrainian,
                                    oldConception.GetLanguageDescriptionsCount(Conception.LanguageId.Ukrainian) - 1).Changeable.Value = changeAble;
                            }

                            if (!string.IsNullOrWhiteSpace(langPart))
                            {
                                oldConception.GetConceptionDescription(Conception.LanguageId.Ukrainian, 
                                     oldConception.GetLanguageDescriptionsCount(Conception.LanguageId.Ukrainian) - 1).LangPart = langPart;
                            }
                            oldConception.Link = linkPart;
                        }
                    }


                    //if (textUkr[0] == '(')
                    //{
                    //    int k = 1;
                    //    List<string> wordsUkr2 = SplitText(textUkr, ", ");
                    //}

                    //if (!textRus.Contains('~'))
                    //{
                    //    mainwordRus = GetMainWord(wordsRus);
                    //    mainwordUkr = GetMainWord(wordsUkr);
                    //    parentId = 0;
                    //    if (mainwordRus == "")
                    //    {
                    //        int a = 2;
                    //        a++;
                    //    }
                    //}
                    //else
                    //{
                    //    textUkr = textUkr.Replace(mainwordUkr, "~");
                    //}

                    //int n = m_Dictionary.AddConception(textRus, Conception.LanguageId.Russian);
                    //m_Dictionary.AddDescriptionToConception(n, textUkr, Conception.LanguageId.Ukrainian);
                    //if( parentId == 0 )
                    //    parentId = n;
                    //else
                    //    m_Dictionary.GetConception(n).ParentId = parentId;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        internal string ExtractParentheses(string textRus, string textUkr)
        {
            string output = "";
            //string pattern = @"(\s|^)(\(.+?\))+(\s|$)";
            string pattern = @"(?<=(^|\s))(\([^#ёЁ]+?\))+(?=(;|\s|$))";
            MatchCollection matchRus = Regex.Matches(textRus, pattern);
            MatchCollection matchUkr = Regex.Matches(textUkr, pattern);
            if (matchRus.Count == matchUkr.Count)
            {
                return "";
            }

            //only 1 topicEx should be in description
            if (matchUkr.Count - matchRus.Count > 0)
            {                
                output += matchUkr[0].Groups[0];
            }

            if (matchUkr.Count - matchRus.Count != 1 )
            {
                File.AppendAllText(@"D:\AS\_Aspirantura\Projects\TrilingualDictionary\Data\parenths.txt",
                    string.Format("1.{0}\r\n2.{1}\r\n", textRus, textUkr), Encoding.Unicode);
            }
            return output.Trim();
        }

        internal string ExtractLinkPart(string text)
        {
            string fmtPattenTemplate = @"(\s|^)\(?({0})\)?.+$";
            //string fmtPattenTemplate = @"(\s|^)\(?{0}\)?.+$";
            return ExtractByPattern2(text, TrilingualDictionary.LinkMarks, fmtPattenTemplate);
        }

        internal string ExtractLangPart(string text)
        {
            string fmtPattenTemplate = @"(\s|^)({0})(\s|,)";
            //string fmtPattenTemplate = @"(\s|^){0}(\s|,)";
            return ExtractByPattern2(text, TrilingualDictionary.LangMarks, fmtPattenTemplate);
        }

        private static string ExtractByPattern(string text, string[] searchers, string fmtPattenTemplate)
        {
            string output = "";
            foreach (string mark in searchers)
            {
                string pattern = string.Format(fmtPattenTemplate, mark.Replace(".", @"\."));
                Match match = Regex.Match(text, pattern);
                if (match.Length > 0)
                {
                    output += match.Groups[0];
                }
            }

            return output.Trim();
        }

        private static string ExtractByPattern2(string text, string[] searchers, string fmtPattenTemplate)
        {
            string output = "";
            string list = "";
            foreach (string mark in searchers)
            {
                list += mark + "|";                
            }
            list = list.Remove(list.Length - 1);
            string pattern = string.Format(fmtPattenTemplate, list.Replace(".", @"\."));
            MatchCollection match = Regex.Matches(text, pattern);
            if (match.Count > 0)
            {
                output += match[0].Groups[0];
            }
            if (match.Count > 1)
            {
                int k = 1;
            }
            return output.Trim();
        }

        internal string ExtractChangableUkr(string text)
        {
            string fmtPattenTemplate = @"(,?\s({0})\s|,?\s-)[\w#]+(\s|$)";
            //string fmtPattenTemplate = @"(,?\s{0}\.\s|,?\s-)[\w#]+(\s|$)";
            return ExtractByPattern2(text, TrilingualDictionary.ChangeableMarks, fmtPattenTemplate);            
        }

        internal string ExtractTopics(string textUkr)
        {
            string fmtPattenTemplate = @"((\s|^)({0})(,|\s))+";
            //string fmtPattenTemplate = @"(\s|^){0}(\s|,)";
            return ExtractByPattern2(textUkr, TrilingualDictionary.TopicMarks, fmtPattenTemplate);

            //string output = "";
            //foreach( string mark in TrilingualDictionary.TopicMarks)
            //{
            //    string pattern = string.Format(@"(\s|^){0}(\s|,)", mark.Replace(".", @"\."));
            //    Match match = Regex.Match(partUkr, pattern);
            //    if (match.Length > 0)
            //        output += match.Groups[0];
            //}

            //if(!string.IsNullOrWhiteSpace(output))
            //    File.AppendAllText(@"D:\AS\_Aspirantura\Projects\TrilingualDictionary\Data\TopicMarks.txt",
            //            string.Format("{0}\r\n", partUkr), Encoding.Unicode);

            //return output.Trim();
        }

        private string GetMainWord(List<string> words)
        {
            if (words.Count == 1 &&
                (words[0].IndexOfAny(new char[] { '[', ']', '(', ')' }) == -1 || !words[0].Contains(' ')))
            {
                return words[0];
            }

            int mainCount = 0;
            int indexMain = 0;
            for (int i = 0; i < words.Count; i++)
            {
                string word = words[i];
                int index = word.IndexOfAny(new char[] { '[', ']', '(', ')' });
                if (index == -1)
                {
                    mainCount++;
                    indexMain = i;
                    if (mainCount > 1)
                        return "";
                }
            }

            return words[indexMain];// if (mainCount == 1) ;
        }

        public static List<string> SplitText(string text, string separators)
        {
            try
            {
                Stack<char> brackets = new Stack<char>();
                Tuple<char, char> br1 = new Tuple<char, char>('[', ']');
                Tuple<char, char> br2 = new Tuple<char, char>('(', ')');
                StringBuilder sb = new StringBuilder();
                List<string> words = new List<string>();
                bool bPrevComma = false;
                foreach (char c in text)
                {
                    if (IsSeparator(separators, c) && brackets.Count == 0 && !bPrevComma)
                    {
                        string word = sb.ToString();
                        if (!string.IsNullOrWhiteSpace(word))
                            words.Add(word.Trim());
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
                    bPrevComma = (c == ',');
                }
                words.Add(sb.ToString().Trim());
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

        private static bool IsSeparator(string separators, char c)
        {
            return separators.Contains(c);// c == ';';//Char.IsWhiteSpace(c) || 
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
    }
}
