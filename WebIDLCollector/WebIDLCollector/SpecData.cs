using System.Collections.Generic;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector
{
    public class SpecData
    {
        public SpecData()
        {
            Identification = new List<SpecIdentification>();
            Implements = new List<ImplementsType>();
            Interfaces = new List<InterfaceType>();
            Dictionaries = new List<DictionaryType>();
            Enumerations = new List<EnumType>();
            TypeDefs = new List<TypeDefType>();
            Callbacks = new List<CallbackType>();
            ShortNamesTitles = new SortedDictionary<string, string>();
        }

        public string Name { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string File { get; set; }

        public List<SpecIdentification> Identification { get; set; }

        public List<ImplementsType> Implements { get; set; }

        public List<InterfaceType> Interfaces { get; set; }

        public List<DictionaryType> Dictionaries { get; set; }

        public List<EnumType> Enumerations { get; set; }

        public List<TypeDefType> TypeDefs { get; set; }

        public List<CallbackType> Callbacks { get; set; }

        public SortedDictionary<string, string> ShortNamesTitles { get; set; }
    }

    public class SpecIdentification
    {
        public string Selector { get; set; }
        public string Type { get; set; }//Change this to an enum
    }
}