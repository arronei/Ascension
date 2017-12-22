"use strict";
var MirrorJS;
(function (MirrorJS) {
    /** Describes a property, returned by TypeModel. */
    var PropertyModel = (function () {
        function PropertyModel(confidence, descriptor) {
            this.confidence = confidence;
            console.assert(confidence !== MirrorJS.Confidence.None);
            this.type = undefined;
            this.isPlausiblyInherited = false;
            this.isPlausiblyDefined = false;
            if (descriptor !== undefined) {
                if (descriptor.value) {
                    this.type = MirrorJS.Utils.getTypeNameFromInstance(descriptor.value);
                }
                this.hasGet = descriptor.get !== undefined;
                this.hasSet = descriptor.set !== undefined;
                this.isConfigurable = descriptor.configurable;
                this.isEnumerable = descriptor.enumerable;
                this.isWritable = descriptor.writable;
            }
        }
        PropertyModel.assertInvariants = function (model) {
            switch (model.confidence) {
                case MirrorJS.Confidence.None:
                    console.assert(model.isPlausiblyDefined, "A property with confidence 'None' must have its 'isPlausiblyDefined.");
                    break;
                case MirrorJS.Confidence.InstanceWithoutBase:
                case MirrorJS.Confidence.InstanceWithSibling:
                case MirrorJS.Confidence.Prototype:
                case MirrorJS.Confidence.InstanceWithBase:
                    break;
                default:
                    console.assert(false, "Unknown Confidence value '" + model.confidence + "' for property on type '" + model.type + "'.");
                    break;
            }
        };
        return PropertyModel;
    })();
    MirrorJS.PropertyModel = PropertyModel;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=PropertyModel.js.map