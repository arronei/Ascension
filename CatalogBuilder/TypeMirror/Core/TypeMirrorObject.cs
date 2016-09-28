using System.Collections.Generic;

namespace MS.Internal.TypeMirror.Core
{
    public class TypeMirrorObject
    {
        public string BrowserVersion { get; set; }
        public string Timestamp { get; set; }
        public IReadOnlyDictionary<string, TypeObject> Types { get; }

        public TypeMirrorObject()
        {
            Types = new SortedDictionary<string, TypeObject>();
        }
    }
}