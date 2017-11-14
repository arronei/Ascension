"use strict";
var MirrorJS;
(function (MirrorJS) {
    /** Describes a property, returned by TypeModel. */
    var PropertyModel = (function () {
        function PropertyModel(confidence, descriptor) {
            this.confidence = confidence;
            this.type = undefined;
            this.isPlausiblyInherited = false;
            this.isPlausiblyDefined = false;
            console.assert(confidence !== 0 /* None */);
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
                case 0 /* None */:
                    console.assert(model.isPlausiblyDefined, "A property with confidence 'None' must have its 'isPlausiblyDefined.");
                    break;
                case 1 /* InstanceWithoutBase */:
                case 2 /* InstanceWithSibling */:
                case 4 /* Prototype */:
                case 3 /* InstanceWithBase */:
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