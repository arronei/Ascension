using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using CatalogBuilder;
using MS.Internal.Generator.Core;

namespace MS.Internal
{
    public static class CatalogBuilder
    {
        private static IList<string> JsonFileNames { get; } = new List<string>();

        private static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            PromptUser();

            //var config = new ConfigurationBuilder();

            var builder = new ContainerBuilder();

            builder.RegisterType<Generator.Venn.Generator>().As<BaseGenerator>();


            Container = builder.Build();

            //var fileName = "";

            using (var scope = Container.BeginLifetimeScope())
            {
                var de = scope.Resolve<BaseGenerator>();

            //    var s = de.DeserializeJsonDataFile<RootObject>(fileName);
            }
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var assemblies = Directory.GetFiles(path, "Generator*.dll", SearchOption.TopDirectoryOnly).Select(Assembly.LoadFrom);
            foreach (var assembly in assemblies)
            {
                var modules = assembly.GetTypes().Where(p => typeof(IModule).IsAssignableFrom(p) && !p.IsAbstract).Select(p => (IModule)Activator.CreateInstance(p));
                foreach (var module in modules)
                {
                    builder.RegisterModule(module);
                }
            }
        }

        private static void PromptUser()
        {
            try
            {
                Console.WriteLine("Use defaults? (Y/N) Waiting 3 seconds...");
                var entry = Reader.ReadLine(3000);
                if (entry.Equals("N", StringComparison.InvariantCultureIgnoreCase))
                {
                    BuildJsonFilePathList();
                    Console.WriteLine();
                    return;
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Timeout expired. Using defaults...");
            }

            BuildJsonFilePathList(true);
            Console.WriteLine();
        }

        private static void BuildJsonFilePathList(bool useDefaults = false)
        {
            var browserSupportListFile = ConfigurationManager.AppSettings["browserSupportListFile"] ?? @".\DataFiles\browsers.json";
            var browserSupportList = new BaseSerializarionJson().DeserializeJsonDataFile<IList<Browser>>(browserSupportListFile);

            var dataFilePath = ConfigurationManager.AppSettings["browserDataFilePath"] ?? @"D:\GitHub\Ascension\TypeMirror\Data\";

            var specDataFilePath = ConfigurationManager.AppSettings["specDataFile"] ?? @".\TypeMirrorDataFiles\SpecMirror.js";
            JsonFileNames.Add(specDataFilePath);

            foreach (var item in browserSupportList)
            {
                var browserName = item.Name;
                var minVersion = item.MinimumSupportedVersion;
                var maxVersion = item.MaximumSupportedVersion;
                var finalVersion = maxVersion;

                if (useDefaults)
                {
                    JsonFileNames.Add(string.Concat(dataFilePath, $@"{browserName}\{browserName}{finalVersion}.js"));
                    continue;
                }
                while (true)
                {
                    Console.WriteLine($"Enter an {browserName} version between ({minVersion} and {maxVersion}):");
                    var browserVersion = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(browserVersion))
                    {
                        Console.WriteLine($"Using {browserName} default ({maxVersion}).");
                        break;
                    }
                    if ((byte.TryParse(browserVersion, out finalVersion)) && (finalVersion >= minVersion && finalVersion <= maxVersion))
                    {
                        break;
                    }
                    Console.WriteLine($"Enter a valid version number between {minVersion} and {maxVersion}.");
                }

                JsonFileNames.Add(string.Concat(dataFilePath, $@"{browserName}\{browserName}{finalVersion}.js"));
            }
        }
    }
}