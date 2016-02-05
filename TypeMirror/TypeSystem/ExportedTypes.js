"use strict";
var MirrorJS;
(function (MirrorJS) {
    /** A collection of exported types. */
    var ExportedReflection = (function () {
        function ExportedReflection() {
        }
        ExportedReflection.prototype.fixupProperties = function () {
            console.assert(this.types["Object"], "'Object' must be in the set of exported types.");
            this.fixupPropertiesRecursive("Object");
        };
        ExportedReflection.prototype.fixupPropertiesRecursive = function (baseName) {
            var _this = this;
            var baseModel = this.types[baseName];
            baseModel.derivedTypes.forEach(function (derivedName, index, array) {
                _this.fixupPropertiesRecursive(derivedName);
            });
            var plausiblyInherited = this.getPlausiblyInherited(baseModel);
            baseModel.derivedTypes.forEach(function (derivedName, typeIndex, array) {
                var derivedModel = _this.types[derivedName];
                Object.keys(plausiblyInherited).forEach(function (propertyName, propertyIndex, array) {
                    var derivedProperty = derivedModel.properties[propertyName];
                    if (derivedProperty) {
                        derivedProperty.isPlausiblyDefined = true;
                        derivedProperty.isPlausiblyInherited = true;
                    }
                    // Clone the properties we find on the first derived type up to the base.  We only
                    // need to do this once.
                    if (!baseModel.properties[propertyName]) {
                        baseModel.properties[propertyName] = (plausiblyInherited[propertyName].clone && plausiblyInherited[propertyName].clone()) || {};
                        baseModel.properties[propertyName].isPlausiblyDefined = true;
                    }
                });
            });
        };
        ExportedReflection.prototype.getPlausiblyInherited = function (baseModel) {
            var _this = this;
            var common = undefined;
            baseModel.derivedTypes.forEach(function (derivedName, index, array) {
                var derivedModel = _this.types[derivedName];
                if (MirrorJS.TypeModel.mayBeMissingProperties(derivedModel)) {
                    return;
                }
                var candidates = Object.keys(derivedModel.properties).sort();
                // Skip any properties for which we already definitively know owner, either because they
                // were discovered via the prototype, or because we were able to compare
                candidates = candidates.filter(function (propertyName, index, array) {
                    var propertyModel = derivedModel.properties[propertyName];
                    console.assert(!propertyModel.isPlausiblyInherited);
                    switch (propertyModel.confidence) {
                        case 1 /* InstanceWithoutBase */:
                            return true;
                        default:
                            console.assert(propertyModel.confidence === 4 /* Prototype */ || propertyModel.confidence === 3 /* InstanceWithBase */ || propertyModel.confidence === 2 /* InstanceWithSibling */);
                            return false;
                    }
                });
                // For the first derived type we examine, all candidates are common to the derived types examined
                // so far.
                if (common === undefined) {
                    common = {};
                    candidates.forEach(function (propertyName, index, array) {
                        common[propertyName] = derivedModel.properties[propertyName];
                    });
                    return;
                }
                // For each subsequent derived types, we filter our initial guess.
                var newCommon = {};
                var bothSources = (1 << 1) && (1 << 2);
                new MirrorJS.MergeIterator([Object.keys(common).sort(), candidates]).forEach(function (candidateName, sources) {
                    if (sources === bothSources) {
                        newCommon[candidateName] = common[candidateName];
                    }
                    return true;
                });
                common = newCommon;
            });
            return common || {};
        };
        return ExportedReflection;
    })();
    MirrorJS.ExportedReflection = ExportedReflection;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=ExportedTypes.js.map