"use strict";
var MirrorJS;
(function (MirrorJS) {
    var ReachableTypesReport = (function () {
        function ReachableTypesReport(report, parent) {
            this.report = report;
            this.parent = parent;
        }
        ReachableTypesReport.prototype.findInstance = function (sourceIndex, typeName) {
            var _this = this;
            var typeNames = this.report.merge(function (types) { return Object.keys(types); });
            typeNames.forEach(function (candidateName, typeSources) {
                var type = _this.report.getType(sourceIndex, candidateName);
                var propertyNames = Object.keys(type.properties);
                propertyNames.forEach(function (propertyName) {
                    var property = type.properties[propertyName];
                    if (property.type === typeName) {
                        MirrorJS.Html.addSpan(_this.parent, '"' + typeName + '": () => { return Instances.getInstance("' + candidateName + '").' + propertyName + '; },\n');
                    }
                });
                return true;
            });
        };
        ReachableTypesReport.prototype.execute = function () {
            var _this = this;
            var typeNames = this.report.merge(function (types) { return Object.keys(types); });
            var nonInstantiable = [];
            typeNames.forEach(function (typeName, typeSources) {
                MirrorJS.Utils.getSetBits(typeSources).forEach(function (sourceIndex) {
                    var typeModel = _this.report.getType(sourceIndex, typeName);
                    if (typeModel.confidence == 0 /* None */ || typeModel.confidence == 4 /* Prototype */) {
                        if (!_this.findInstance(sourceIndex, typeName)) {
                        }
                    }
                });
                return true;
            });
            console.log("delete me.");
        };
        return ReachableTypesReport;
    })();
    MirrorJS.ReachableTypesReport = ReachableTypesReport;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=ReachableTypesReport.js.map