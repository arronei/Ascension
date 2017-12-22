using System;
using System.Collections.Generic;
using System.Linq;
using MS.Internal.Utility;

namespace TypeSystem.Data.Browser
{
    public class BrowsersCollection : List<BrowserInfo>
    {
        public IEnumerable<string> BrowserOrder { private get; set; } = "Edge,Chrome,Firefox,Safari".Split(',');

        public (byte Max, byte Min) MinMaxByBrowser(string shortName)
        {
            var allVersions = GetAllByShortName(shortName).ToList();
            return (Max: allVersions.Max(a => a.Version), Min: allVersions.Min(a => a.Version));
        }

        public BrowserInfo GetLatestItem(string shortName)
        {
            return GetAllByShortName(shortName).Last();
        }

        private IEnumerable<BrowserInfo> GetAllByShortName(string shortName)
        {
            return this.Where(a => a.ShortName == shortName).OrderBy(a => a.Version);
        }

        public string GetLatestFullName(string shortName)
        {
            var latestBrowser = GetLatestItem(shortName);
            return $"{latestBrowser.Name} {latestBrowser.Version}" + (string.IsNullOrWhiteSpace(latestBrowser.Release) ? string.Empty : $" ({latestBrowser.Release})");
        }

        public string GetLatestFilePath(string shortName)
        {
            var latestBrowser = GetLatestItem(shortName);
            return latestBrowser.GetFilePath();
        }

        public BrowserInfo GetItem(string shortName, byte? version)
        {
            return this.Any(a=>a.ShortName == shortName && a.Version == version) ? this.Where(a => a.ShortName == shortName).First(a => a.Version == version) : null;
        }

        private IEnumerable<BrowserInfo> GetAllBrowserData(string[] releases = null)
        {
            var returnValue = new List<BrowserInfo>();
            foreach (var browserName in BrowserOrder)
            {
                BrowserInfo latestVersion = null;
                BrowserInfo latestReleaseDate = null;
                var browserReleases = new List<BrowserInfo>();
                foreach (var item in FindAll(a => a.ShortName == browserName))
                {
                    latestVersion = latestVersion == null || latestVersion.Version < item.Version ? item : latestVersion;

                    latestReleaseDate = latestReleaseDate == null ? item : (latestReleaseDate.ReleaseDate < item.ReleaseDate) ? item : latestReleaseDate;

                    if (releases != null && item.Release.Split(',').Intersect(releases, StringComparer.InvariantCultureIgnoreCase).Any())
                    {
                        browserReleases.Add(item);
                    }
                }
                if (releases != null)
                {
                    if (!returnValue.Contains(latestVersion))
                    {
                        returnValue.Add(latestVersion);
                    }
                    if (releases.Contains("Release", StringComparer.InvariantCultureIgnoreCase) && !returnValue.Contains(latestReleaseDate))
                    {
                        returnValue.Add(latestReleaseDate);
                    }
                    returnValue.AddRange(browserReleases.Where(a => !returnValue.Contains(a)));
                }
                else
                {
                    returnValue.Add(latestVersion);
                }
            }

            return returnValue.OrderBy(a => a.Version).OrderBySequence(BrowserOrder, a => a.ShortName);
        }

        public IEnumerable<string> GetAllBrowserFilePaths(string[] releases = null)
        {
            return GetAllBrowserData(releases).Select(a => a.GetFilePath());
        }

        public IEnumerable<string> GetAllBrowserFullNames(string[] releases = null)
        {
            return GetAllBrowserData(releases).Select(a => a.GetFullName());
        }

        public IEnumerable<string> GetAllBrowserShortNames(string[] releases = null)
        {
            return GetAllBrowserData(releases).Select(a => a.ShortName).Distinct();
        }
    }
}
