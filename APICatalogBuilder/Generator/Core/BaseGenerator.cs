using System;
using System.IO;
using TypeSystem.Data.Core;

namespace Generator.Core
{
    public class BaseGenerator<T, TU> : BaseSerializarionJson<TU>, IGenerator<T, TU>
    {
        public string OutputPath { get; set; }

        public virtual T GenerateSpecificDataObject(TU dataObject)
        {
            throw new NotImplementedException();
        }

        public virtual T ProcessJsonObject<TV>(TV jsonObject)
        {
            throw new NotImplementedException();
        }

        public void Write(string jsonString)
        {
            Write(OutputPath, jsonString);
        }

        public void Write(string pathAndFileName, string jsonString)
        {
            if (pathAndFileName.LastIndexOf(@"\", StringComparison.Ordinal) >= 0)
            {
                var path = pathAndFileName.Substring(0, pathAndFileName.LastIndexOf(@"\", StringComparison.Ordinal));
                Directory.CreateDirectory(path);
            }
            if(File.Exists(pathAndFileName))
            {
                File.Delete(pathAndFileName);
            }
            File.WriteAllText(pathAndFileName, jsonString);
        }
    }
}