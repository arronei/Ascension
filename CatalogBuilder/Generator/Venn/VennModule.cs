using Autofac;
using MS.Internal.Generator.Core;
using MS.Internal.Generator.Venn;

namespace Venn
{
    public class VennModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new Generator()).As<IGenerator>();
        }
    }
}