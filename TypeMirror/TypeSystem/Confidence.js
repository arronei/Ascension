"use strict";
var MirrorJS;
(function (MirrorJS) {
    /** Records how getOwnProperties() discovered a property.  This tells us how "confident" we are
       that the property is owned by the type being enumerated. */
    var Confidence;
    (function (Confidence) {
        /** We have little confidence in the information about the type or property. */
        Confidence[Confidence["None"] = 0] = "None";
        /** The property was present on an instance, but we were unable to verify that the property is not
            inheritted from the base type (we don't know how to get an instance of the base). */
        Confidence[Confidence["InstanceWithoutBase"] = 1] = "InstanceWithoutBase";
        /** The property was present on an instance and not on an instance of a peer type that derives
            from the same parent. */
        Confidence[Confidence["InstanceWithSibling"] = 2] = "InstanceWithSibling";
        /** The property was present on an instance and not on an instance of the base type, or the type
            has no base type (fairly confident.) */
        Confidence[Confidence["InstanceWithBase"] = 3] = "InstanceWithBase";
        /** The property was discovered via the prototype (highly confident). */
        Confidence[Confidence["Prototype"] = 4] = "Prototype";
    })(Confidence = MirrorJS.Confidence || (MirrorJS.Confidence = {}));
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=Confidence.js.map
