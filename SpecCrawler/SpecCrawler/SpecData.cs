using System;

namespace MS.Internal
{
    public class SpecData
    {
        public string ShortName { get; set; }
        public Date PublishDate { get; set; }
        public string Url { get; set; }
        public SpecTypes Type { get; set; }
        public string Selector { get; set; }

    }

    public enum SpecTypes
    {
        Css,
        SvgCss,
        Bikeshed,
        Respec,
        Idl
    }
}