using System;

namespace MS.Internal.TypeSystem.Core
{
    [Flags]
    public enum Browsers
    {
        None = 0,
        Specification = 1,
        Edge = 2,
        Chrome = 4,
        Firefox = 8,
        Safari = 16,
        Ie = 32,
        Opera = 64
    }
}