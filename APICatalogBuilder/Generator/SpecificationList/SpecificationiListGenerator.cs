using System.Linq;
using Generator.Core;
using TypeSystem.Data.ApiCatalog;
using TypeSystem.Data.SpecRef;

namespace Generator.SpecificationList
{
    public class SpecificationiListGenerator : BaseGenerator<string, CatalogObject>
    {
        public override string GenerateSpecificDataObject(CatalogObject catalogObject)
        {
            if (catalogObject == null)
            {
                return null;
            }
            var finalSpecNameList = catalogObject.Specifications.Select(catalogObjectSpecification => new SpecList
            {
                ShortName = catalogObjectSpecification.Key,
                Title = catalogObjectSpecification.Value.Trim('"')
            }).OrderBy(t => t.Title).ToList();

            return SerializeObject(finalSpecNameList);
        }
    }
}