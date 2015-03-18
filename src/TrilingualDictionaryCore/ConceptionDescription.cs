using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace TrilingualDictionaryCore
{
    public class ConceptionDescription
    {
        public class ChangeablelPart
        {
            public string Type;
            public string Value;
        }

        private string m_Word;
        private string m_Explanation = "";
        private Conception m_Conception = null;
        private int m_DescriptionId;

        public ChangeablelPart Changeable
        {
            get;
            set;
        }

        public string Topic { get; set; }

        public string Semantic { get; set; }

        public string Link { get; set; }

        public ConceptionDescription(Conception conception, string word)
        {
            m_Conception = conception;
            m_Word = word;
            Changeable = new ChangeablelPart();
        }

        public Conception OwnConception
        {
            get { return m_Conception; }
        }

        internal void ChangeDescription(string word)
        {
            m_Word = word;
        }

        public string ConceptionRegistryDescription
        {
            get { return m_Word; }
        }

        public string ConceptionRegistryDescriptionWoAccents
        {
            get { return m_Word.Replace("#", ""); }
        }

        public string ConceptionExplanation
        {
            get { return m_Explanation; }
        }

        public string LangPart { get; set; }

        public string ConceptionSortDescription 
        {
            get
            {
                //string forSorting = Regex.Replace(m_Word, @"(\(|\)|\[|\]|#)", "");
                string forSorting = Regex.Replace(m_Word, @"([^\w-\.~ ])", "");
                return forSorting;
            }
        }

        internal void SaveDescription(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("RegistryDescription", ConceptionRegistryDescription);
            if (!string.IsNullOrWhiteSpace(Topic))
                writer.WriteAttributeString("Topic", Topic);
            if (!string.IsNullOrWhiteSpace(Semantic))
                writer.WriteAttributeString("Semantic", Semantic);
            if (!string.IsNullOrWhiteSpace(Changeable.Type))
                writer.WriteAttributeString("ChangeableType", Changeable.Type);
            if (!string.IsNullOrWhiteSpace(Changeable.Value))
                writer.WriteAttributeString("ChangeableValue", Changeable.Value);
            if (!string.IsNullOrWhiteSpace(LangPart))
                writer.WriteAttributeString("LangPart", LangPart);
            if (!string.IsNullOrWhiteSpace(Link))
                writer.WriteAttributeString("Link", Link);
        }

        internal static ConceptionDescription Create(XmlReader reader, Conception conception)
        {
            string RegistryDescription = GetXmlAttribute(reader, "RegistryDescription");
            ConceptionDescription desc = new ConceptionDescription(conception, RegistryDescription);

            desc.Topic = GetXmlAttribute(reader, "Topic");
            desc.Semantic = GetXmlAttribute(reader, "Semantic");
            desc.Changeable.Type = GetXmlAttribute(reader, "ChangeableType");
            desc.Changeable.Value = GetXmlAttribute(reader, "ChangeableValue");
            desc.LangPart = GetXmlAttribute(reader, "LangPart");
            desc.Link = GetXmlAttribute(reader, "Link");
            return desc;
        }

        /// <summary>
        /// Converts english scanned equivalent to Cyrillic
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetXmlAttribute(XmlReader reader, string name)
        {
            string result = reader[name] ?? "";
            string pattern = "(a|A|B|e|E|c|C|T|y|i|I|o|O|p|P|H|k|K|x|X|m|M)";
            string result1 = result;
            result = Regex.Replace(result, pattern, (match) =>
            {
                string s = result1;
                switch (match.Value)
                {
                    case "a":
                        return "а";
                    case "A":
                        return "А";
                    case "B":
                        return "В";
                    case "e":
                        return "е";
                    case "E":
                        return "Е";
                    case "c":
                        return "с";
                    case "C":
                        return "С";
                    case "T":
                        return "Т";
                    case "y":
                        return "у";
                    case "i":
                        return "і";
                    case "I":
                        return "І";
                    case "o":
                        return "о";
                    case "O":
                        return "О";
                    case "p":
                        return "р";
                    case "P":
                        return "Р";
                    case "H":
                        return "Н";
                    case "k":
                        return "к";
                    case "K":
                        return "К";
                    case "x":
                        return "х";
                    case "X":
                        return "Х";
                    //case "m":
                    //    return "м";
                    case "M":
                        return "М";
                }
                return "";
            });
            return result;
        }

        public int DescriptionId
        {
            get { return m_DescriptionId; }
            set { m_DescriptionId = value; }
        }
    }
}
