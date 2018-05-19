using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiDictionaryCore
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    /// <summary>
    /// Abstart class for every entity
    /// </summary>
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Class which describes semantic of a term
    /// </summary>
    public class SemanticDescription : Entity
    {
    }

    /// <summary>
    /// Class which describes knowledge area of a term
    /// </summary>
    public class TopicDescription : Entity
    {
    }

    /// <summary>
    /// Class which describes language abstraction
    /// </summary>
    public class Language : Entity
    {
    }

    /// <summary>
    /// Class which describes part of speech abstraction of a word
    /// </summary>
    public class PartOfSpeech : Entity
    {
    }

    /// <summary>
    /// Class which describes changeable part abstraction of a word
    /// </summary>
    public class ChangPart : Entity
    {
        public ChangPartType Type { get; set; }
        
        /// <summary>
        /// Value of morfema
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Language where morfema is used
        /// </summary>
        public Language Language { get; set; }
    }

    /// <summary>
    /// Class which describes type of changeable part abstraction. For example "suffix", "ending", "prefix"
    /// </summary>
    public class ChangPartType : Entity
    {

    }

    /// <summary>
    /// Class which describes term abstraction
    /// </summary>
    public class Term1 : Entity
    {
        public TopicDescription Topic { get; set; }
        public SemanticDescription Semantic { get; set; }        
        public Term1 Parent { get; set; }

        //internal void AddTranslation(TermTranslation item)
        //{
        //    List<TermTranslation> trans;
        //    if (!translations.TryGetValue(item.LanguageId, out trans))
        //    {
        //        trans = new List<TermTranslation>();
        //        translations.Add(item.Translation.Language.Id, trans);
        //    }

        //    trans.Add(item);
        //    trans.Sort((word1, word2) => word1.Translation.Value.CompareTo(word2.Translation.Value));            
        //}

        //internal List<TermTranslation> GetTranslation(int languageId)
        //{
        //    List<TermTranslation> trans = null;
        //    if (!translations.TryGetValue(languageId, out trans))
        //    {
        //        return null;
        //    }
        //    return trans;
        //}

        //private Dictionary<int, List<TermTranslation>> translations = new Dictionary<int, List<TermTranslation>>();
    }
}
