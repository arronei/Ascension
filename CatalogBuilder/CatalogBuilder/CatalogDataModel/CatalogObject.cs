using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogBuilder.CatalogDataModel
{
    public class CatalogObject
    {
        public Dictionary<string, CallbackObject> Callbacks { get; set; }
        public Dictionary<string, EnumerationObject> Enumerations { get; set; }
        public Dictionary<string, TypeDefObject> TypeDefs { get; set; }
        public Dictionary<string, DictionaryObject> Dictionaries { get; set; }
        public Dictionary<string, InterfaceObject> Interfaces { get; set; }
        public Dictionary<string, ImplementObject> Implements { get; set; }
    }
}