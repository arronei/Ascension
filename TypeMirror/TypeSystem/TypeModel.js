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
                case 0 /* None */:
                    return true;
                case 4 /* Prototype */:
                case 3 /* InstanceWithBase */:
                case 1 /* InstanceWithoutBase */:
                    return false;
                default:
                    console.assert(false);
            }
        };
        /** True if the type may report properties as its own that it actually inherited. */
        TypeModel.mayReportInheritedPropertiesAsOwn = function (self) {
            switch (self.confidence) {
                case 0 /* None */:
                case 1 /* InstanceWithoutBase */:
                    return Object.keys(self.properties).reduce(function (previous, propertyName, index, array) {
                        var propertyModel = self.properties[propertyName];
                        switch (propertyModel.confidence) {
                            case 3 /* InstanceWithBase */:
                            case 2 /* InstanceWithSibling */:
                            case 4 /* Prototype */:
                                return previous;
                            case 1 /* InstanceWithoutBase */:
                                return true;
                        }
                    }, false);
                case 4 /* Prototype */:
                case 3 /* InstanceWithBase */:
                    return false;
                default:
                    console.assert(false);
            }
        };
        return TypeModel;
    })();
    MirrorJS.TypeModel = TypeModel;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=TypeModel.js.map