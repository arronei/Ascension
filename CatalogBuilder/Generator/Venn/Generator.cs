using System;
using MS.Internal.Generator.Core;

namespace MS.Internal.Generator.Venn
{
    public class Generator : BaseGenerator
    {
        public override T GenerateSpecificDataObject<T, TU>(TU dataObject)
        {
            throw new NotImplementedException();
        }

        public override T ProcessJsonObject<T, TU>(TU jsonObject)
        {
            throw new NotImplementedException();
        }
    }
}