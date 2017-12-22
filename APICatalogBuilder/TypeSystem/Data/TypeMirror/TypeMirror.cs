using System;
using TypeSystem.Data.ApiCatalog;
using TypeSystem.Data.Browser;
using TypeSystem.Data.Core;

namespace TypeSystem.Data.TypeMirror
{
    public class TypeMirror : BaseSerializarionJson<BrowserObject>
    {
        public void ProcessData(CatalogObject catalogDataObject, BrowserObject browserTypeMirrorData, BrowsersCollection fullBrowserReleaseList)
        {
            Console.WriteLine($"Process data for {browserTypeMirrorData.Name} ...");

            var sn = browserTypeMirrorData.ShortName;
            var v = browserTypeMirrorData.Version;

            var supportedBrowser = fullBrowserReleaseList.GetItem(sn, v)?.GetFullName() ?? sn;

            catalogDataObject.Browsers.Add(supportedBrowser);

            foreach (var interfaceObject in browserTypeMirrorData.Interfaces)
            {
                var interfaceName = interfaceObject.Key;
                var members = interfaceObject.Value.Members;

                if (catalogDataObject.Interfaces.ContainsKey(interfaceName))
                {
                    foreach (var member in members)
                    {
                        var memberName = member.Key;
                        var existingInterface = catalogDataObject.Interfaces[interfaceName];
                        existingInterface.SupportedBrowsers.Add(supportedBrowser);
                        if (existingInterface.Members.ContainsKey(memberName))
                        {
                            var existingMember = existingInterface.Members[memberName];
                            existingMember.SupportedBrowsers.Add(supportedBrowser);
                        }
                        else  // interface does not contain member
                        {
                            var newMember = new ApiCatalog.MemberObject(memberName)
                            {
                                SpecificationNames = member.Value.SpecificationNames
                            };
                            newMember.SupportedBrowsers.Add(supportedBrowser);
                            existingInterface.Members.Add(memberName, newMember);
                        }
                    }
                }
                else  // Catalog does not contain interface
                {
                    var newInterface = new ApiCatalog.InterfaceObject(interfaceName)
                    {
                        SpecificationNames = interfaceObject.Value.SpecificationNames
                    };
                    newInterface.SupportedBrowsers.Add(supportedBrowser);

                    foreach (var member in interfaceObject.Value.Members)
                    {
                        var memberName = member.Key;
                        var newMember = new ApiCatalog.MemberObject(memberName)
                        {
                            SpecificationNames = member.Value.SpecificationNames
                        };
                        newMember.SupportedBrowsers.Add(supportedBrowser);
                        newInterface.Members.Add(memberName, newMember);
                    }
                    catalogDataObject.Interfaces.Add(interfaceName, newInterface);
                }
            }

            Console.WriteLine($"Processing for {browserTypeMirrorData.Name} Complete.");
            Console.WriteLine();
        }

        public BrowserObject DeserializeJsonDataFile(string browserReleaseFilePath)
        {
            Console.WriteLine($"Retrieve TypeMirror data ({browserReleaseFilePath})...");

            return DeserializeJsonData(browserReleaseFilePath);
        }
    }
}