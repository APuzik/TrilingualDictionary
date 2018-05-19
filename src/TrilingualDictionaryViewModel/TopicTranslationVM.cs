using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryViewModel
{
    public class TranslationVM
    {
        ITranslatable m_Item;
        bool m_AddItem;
        bool m_RealLanguage;
        LanguageId m_MainLanguage = LanguageId.Undefined;

        public TranslationVM(ITranslatable item, bool realLanguage)
        {
            m_Item = item;
            m_AddItem = false;
            m_RealLanguage = realLanguage;
        }

        private TranslationVM()
        {
            m_Item = default(ITranslatable);
            m_AddItem = false;
            m_RealLanguage = false;
        }

        public ITranslatable Item { get { return m_Item; } }
        static internal TranslationVM CreateEmptyTopic()
        {
            return new TranslationVM
            {
                m_AddItem = false,
                m_Item = default(ITranslatable),
                m_RealLanguage = false
            };
        }

        static internal TranslationVM CreateAddItemTopic(LanguageId langId)
        {
            return new TranslationVM
            {
                m_AddItem = true,
                m_Item = default(ITranslatable),
                m_RealLanguage = false,
                m_MainLanguage = langId
            };
        }

        public string Translation
        {
            get
            {
                if (m_AddItem)
                {
                    return "Изменить...";
                    //switch (m_MainLanguage)
                    //{                      
                        //case LanguageId.Russian:
                        //    return "Добавить...";
                        //case LanguageId.English:
                        //    return "Add...";
                        //case LanguageId.Ukrainian:
                        //    return "Додати...";
                    //}
                }
                else if (m_Item == null)
                    return "";
                else if (m_MainLanguage == LanguageId.Undefined)
                {
                    return m_Item.Translation;
                }

                return "Undefined";
            }

            set { m_Item.Translation = value; }
        }

        public bool IsAbsent
        {
            get { return !m_RealLanguage; }
            set { m_RealLanguage = !value; }
        }

        public bool IsAddItem
        {
            get { return m_AddItem; }
        }

    }

    //public class TranslationVM<T>
    //{
    //    T m_Item;
    //    bool m_AddItem;
    //    bool m_RealLanguage;
    //    LanguageId m_MainLanguage = LanguageId.Undefined;

    //    public TranslationVM(T item, bool realLanguage)
    //    {
    //        m_Item = item;
    //        m_AddItem = false;
    //        m_RealLanguage = realLanguage;
    //    }

    //    private TranslationVM()
    //    {
    //        m_Item = default(T);
    //        m_AddItem = false;
    //        m_RealLanguage = false;
    //    }

    //    static internal TranslationVM<T> CreateEmptyTopic()
    //    {
    //        return new TranslationVM<T>
    //        {
    //            m_AddItem = false,
    //            m_Item = default(T),
    //            m_RealLanguage = false
    //        };
    //    }

    //    static internal TranslationVM<T> CreateAddItemTopic(LanguageId langId)
    //    {
    //        return new TranslationVM<T>
    //        {
    //            m_AddItem = true,
    //            m_Item = default(T),
    //            m_RealLanguage = false,
    //            m_MainLanguage = langId
    //        };
    //    }

    //    public string Translation
    //    {
    //        get
    //        {
    //            if (m_AddItem)
    //            {
    //                switch (m_MainLanguage)
    //                {
    //                    case LanguageId.Russian:
    //                        return "Добавить...";
    //                    case LanguageId.English:
    //                        return "Add...";
    //                    case LanguageId.Ukrainian:
    //                        return "Додати...";
    //                }
    //            }
    //            else if (m_Item == null)
    //                return "";
    //            else if (m_MainLanguage == LanguageId.Undefined)
    //            {
    //                var dataProperty = m_Item.GetType().GetProperty("Translation");
    //                object data = dataProperty.GetValue(m_Item, new object[] { });
    //                return (string)data;
    //            }

    //            return "Undefined";
    //        }
    //    }

    //    public bool IsAbsent
    //    {
    //        get { return !m_RealLanguage; }
    //    }

    //    public bool IsAddItem
    //    {
    //        get { return m_AddItem; }
    //    }

    //}

    //public class TopicTranslationVM
    //{
    //    TopicTranslation m_Topic;
    //    bool m_AddItem;
    //    bool m_RealLanguage;
    //    LanguageId m_MainLanguage = LanguageId.Undefined;

    //    public TopicTranslationVM(TopicTranslation topic, bool realLanguage)
    //    {
    //        m_Topic = topic;
    //        m_AddItem = false;
    //        m_RealLanguage = realLanguage;
    //    }

    //    private TopicTranslationVM()
    //    {
    //        m_Topic = null;
    //        m_AddItem = false;
    //        m_RealLanguage = false;
    //    }
        
    //    static internal TopicTranslationVM CreateEmptyTopic()
    //    {
    //        return new TopicTranslationVM
    //        {
    //            m_AddItem = false,
    //            m_Topic = null,
    //            m_RealLanguage = false
    //        };
    //    }

    //    static internal TopicTranslationVM CreateAddItemTopic(LanguageId langId)
    //    {
    //        return new TopicTranslationVM
    //        {
    //            m_AddItem = true,
    //            m_Topic = null,
    //            m_RealLanguage = false,
    //            m_MainLanguage = langId
    //        };
    //    }

    //    public string Translation
    //    {
    //        get 
    //        {
    //            if (m_AddItem)
    //            {
    //                switch (m_MainLanguage)
    //                {
    //                    case LanguageId.Russian:
    //                        return "Добавить...";
    //                    case LanguageId.English:
    //                        return "Add...";
    //                    case LanguageId.Ukrainian:
    //                        return "Додати...";
    //                }
    //            }
    //            else if(m_Topic == null)
    //                return "";
    //            else if (m_MainLanguage == LanguageId.Undefined)
    //                return m_Topic.Translation;

    //            return "Undefined";
    //        }
    //    }

    //    public bool IsAbsent
    //    {
    //        get { return !m_RealLanguage; }
    //    }

    //    public bool IsAddItem
    //    {
    //        get { return m_AddItem; }
    //    }

    //}
}
