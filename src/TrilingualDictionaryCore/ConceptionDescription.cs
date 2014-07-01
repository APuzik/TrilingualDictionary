using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public ChangeablelPart Changeable
        {
            get;
            set;
        }

        public string Topic { get; set; }

        public string Semantic { get; set; }

        public string Link { get; set; }

        public ConceptionDescription(string word)
        {
            m_Word = word;
            Changeable = new ChangeablelPart();
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

        internal static ConceptionDescription Create(System.Xml.XmlReader reader)
        {
            string RegistryDescription = reader["RegistryDescription"];
            ConceptionDescription desc = new ConceptionDescription(RegistryDescription);
            
            desc.Topic = reader["Topic"];
            desc.Semantic = reader["Semantic"];
            desc.Changeable.Type = reader["ChangeableType"];
            desc.Changeable.Value = reader["ChangeableValue"];
            desc.LangPart = reader["LangPart"];
            desc.Link = reader["Link"];
            return desc;
        }
        
    }
}
