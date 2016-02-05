"use strict";
var MirrorJS;
(function (MirrorJS) {
    var BucketizedTypeData = (function () {
        function BucketizedTypeData(report) {
            var _this = this;
            this.report = report;
            this.sourcesToTypes = [];
            this.sourcesToProperties = [];
            var chromeIndices = [];
            for (var i = 0; i < this.report.reflections.length; i++) {
                if (MirrorJS.Utils.isFromChrome(this.report.reflections[i])) {
                    chromeIndices.push(i);
                }
            }
            var typeNames = this.report.merge(function (types) { return Object.keys(types); });
            typeNames.forEach(function (typeName, typeSources) {
                var skipType = chromeIndices.reduce(function (previous, current) {
                    if (previous) {
                        return true;
                    }
                    var chromeType = _this.report.getType(current, typeName);
                    if (chromeType !== undefined) {
                        if (MirrorJS.TypeModel.mayBeMissingProperties(chromeType)) {
                            return true;
                        }
                    }
                }, false);
                if (skipType) {
                    _this.typesSkipped++;
                    return true;
                }
                var types = _this.getTypesForSources(typeSources);
                types[typeName] = typeName;
                var propertyNames = _this.report.mergeType(typeName, function (types, type) {
                    return MirrorJS.Report.getPropertyNames(types, type, false);
                });
                propertyNames.forEach(function (propertyName, propertySources) {
                    var properties = _this.getPropertiesForSources(propertySources);
                    properties[typeName + "." + propertyName] = typeName + "." + propertyName;
                    return true;
                });
                return true;
            });
        }
        BucketizedTypeData.prototype.getTypesForSources = function (sources) {
            var types = this.sourcesToTypes[sources];
            if (types === undefined) {
                types = [];
                this.sourcesToTypes[sources] = types;
            }
            return types;
        };
        BucketizedTypeData.prototype.getPropertiesForSources = function (sources) {
            var properties = this.sourcesToProperties[sources];
            if (properties === undefined) {
                properties = [];
                this.sourcesToProperties[sources] = properties;
            }
            return properties;
        };
        return BucketizedTypeData;
    })();
    MirrorJS.BucketizedTypeData = BucketizedTypeData;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=BucketizedTypeData.js.map