using System.Collections.Generic;

namespace WebIDLCollector.IDLTypes
{
    public class Definitions
    {
        public Definitions()
        {
            Implements = new List<ImplementsType>();
            Interfaces = new List<InterfaceType>();
            Dictionaries = new List<DictionaryType>();
            Enumerations = new List<EnumType>();
            TypeDefs = new List<TypeDefType>();
            Callbacks = new List<CallbackType>();
        }

        public List<ImplementsType> Implements { get; set; }

        public List<InterfaceType> Interfaces { get; set; }

        public List<DictionaryType> Dictionaries { get; set; }

        public List<EnumType> Enumerations { get; set; }

        public List<TypeDefType> TypeDefs { get; set; }

        public List<CallbackType> Callbacks { get; set; }
    }
}