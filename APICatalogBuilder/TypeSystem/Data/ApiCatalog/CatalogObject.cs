using System.Collections.Generic;
using System.Linq;

namespace TypeSystem.Data.ApiCatalog
{
    public class CatalogObject
    {
        private SortedDictionary<string, string> _specifications;

        private readonly SortedDictionary<string, string> _specRefFullList;

        public SortedDictionary<string, InterfaceObject> Interfaces { get; }
        //public IDictionary<string, IImplementObject> Implements { get; set; }

        public IList<string> Browsers { get; }

        public SortedDictionary<string, string> Specifications
        {
            get
            {
                if (_specifications.Any())
                {
                    return _specifications;
                }
                foreach (var specShortName in Interfaces.Values.SelectMany(a => a.SpecificationNames).Distinct().OrderBy(a => a))
                {
                    _specifications.Add(specShortName, _specRefFullList[specShortName]);
                }

                var thisListOfSpecs = Interfaces.Values.SelectMany(a => a.SpecificationNames).Distinct().OrderBy(a => a).ToDictionary(k=>k);
                var intersectedList = _specRefFullList.Intersect(thisListOfSpecs, new SortedDictionaryKeyComparer()).ToDictionary(k=>k.Key, v=>v.Value);
                _specifications = new SortedDictionary<string, string>(intersectedList);

                return _specifications;
            }
            set => _specifications = value;
        }

        public CatalogObject(SortedDictionary<string, string> specRefFullList)
        {
            Interfaces = new SortedDictionary<string, InterfaceObject>();
            Browsers = new List<string>();
            Specifications = new SortedDictionary<string, string>();
            _specRefFullList = specRefFullList;
        }
    }

    public class SortedDictionaryKeyComparer : IEqualityComparer<KeyValuePair<string, string>>
    {
        public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
        {
            return x.Key == y.Key;
        }

        public int GetHashCode(KeyValuePair<string, string> x)
        {
            return x.Key.GetHashCode() + x.Key.GetHashCode();
        }
    }
}