"use strict";
var MirrorJS;
(function (MirrorJS) {
    /** Describes a type. */
    var TypeModel = (function () {
        function TypeModel(typeContext, typeName) {
            this.typeName = typeName;
            /** The names of discovered types whose prototypes inherit from this type. */
            this.derivedTypes = [];
            this.baseType = typeContext.getBaseType(typeName);
            this.confidence = typeContext.getConfidence(typeName);
            this.properties = typeContext.getOwnProperties(typeName);
            this.derivedTypes = typeContext.getDerivedTypes(typeName);
        }
        TypeModel.getAllProperties = function (exported, typeName) {
            var type = exported[typeName];
            var properties = type.properties;
            var baseType = type.baseType;
            if (baseType !== undefined) {
                var allBaseProperties = TypeModel.getAllProperties(exported, baseType);
                for (var basePropertyName in allBaseProperties) {
                    properties[basePropertyName] = allBaseProperties[basePropertyName];
                }
            }
            return properties;
        };
        /** True if the type may be missing properties. */
        TypeModel.mayBeMissingProperties = function (self) {
            switch (self.confidence) {
                case MirrorJS.Confidence.None:
                    return true;
                case MirrorJS.Confidence.Prototype:
                case MirrorJS.Confidence.InstanceWithBase:
                case MirrorJS.Confidence.InstanceWithoutBase:
                    return false;
                default:
                    console.assert(false);
            }
        };
        /** True if the type may report properties as its own that it actually inherited. */
        TypeModel.mayReportInheritedPropertiesAsOwn = function (self) {
            switch (self.confidence) {
                case MirrorJS.Confidence.None:
                case MirrorJS.Confidence.InstanceWithoutBase:
                    return Object.keys(self.properties).reduce(function (previous, propertyName, index, array) {
                        var propertyModel = self.properties[propertyName];
                        switch (propertyModel.confidence) {
                            case MirrorJS.Confidence.InstanceWithBase:
                            case MirrorJS.Confidence.InstanceWithSibling:
                            case MirrorJS.Confidence.Prototype:
                                return previous;
                            case MirrorJS.Confidence.InstanceWithoutBase:
                                return true;
                        }
                    }, false);
                case MirrorJS.Confidence.Prototype:
                case MirrorJS.Confidence.InstanceWithBase:
                    return false;
                default:
                    console.assert(false);
            }
        };
        return TypeModel;
    }());
    MirrorJS.TypeModel = TypeModel;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=TypeModel.js.map
