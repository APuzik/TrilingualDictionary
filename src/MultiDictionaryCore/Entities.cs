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
    abstract class Entity : IEntity
    {
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        int id;
    }

    /// <summary>
    /// Class which describes semantic of a term
    /// </summary>
    class Semantic : Entity
    {
    }

    /// <summary>
    /// Class which describes knowledge area of a term
    /// </summary>
    class Topic : Entity
    {
    }

    /// <summary>
    /// Class which describes language abstraction
    /// </summary>
    class Language : Entity
    {
    }

    /// <summary>
    /// Class which describes part of speech abstraction of a word
    /// </summary>
    class PartOfSpeech : Entity
    {
    }

    /// <summary>
    /// Class which describes changeable part abstraction of a word
    /// </summary>
    class ChangPart : Entity
    {
        ChangPartType type;
    }

    /// <summary>
    /// Class which describes type of changeable part abstraction. For example "suffix", "ending", "prefix"
    /// </summary>
    class ChangPartType : Entity
    {

    }

    /// <summary>
    /// Class which describes term abstraction
    /// </summary>
    class Term : Entity
    {
        public Topic Topic { get; set; }
        public Semantic Semantic { get; set; }        
        public Term Parent { get; set; }
    }
}
