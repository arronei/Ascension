using System.Collections.Generic;

namespace MS.Internal.TypeSystem.Core
{
    public class TypeSystemObject
    {
        //public IDictionary<string, ICallbackObject> Callbacks { get; set; }
        //public IDictionary<string, IEnumerationObject> Enumerations { get; set; }
        //public IDictionary<string, ITypeDefObject> TypeDefs { get; set; }
        //public IDictionary<string, IDictionaryObject> Dictionaries { get; set; }
        public IReadOnlyDictionary<string, InterfaceObject> Interfaces { get; }
        //public IDictionary<string, IImplementObject> Implements { get; set; }

        public TypeSystemObject()
        {
            Interfaces = new Dictionary<string, InterfaceObject>();
        }
    }
}