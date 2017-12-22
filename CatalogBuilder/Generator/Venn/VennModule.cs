using Autofac;
using MS.Internal.Generator.Core;

namespace MS.Internal.Generator.Venn
{
    public class VennModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new Generator()).As<IGenerator>();
        }
    }
}