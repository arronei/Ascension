"use strict";
var MirrorJS;
(function (MirrorJS) {
    var PropertyConfigurationReport = (function () {
        function PropertyConfigurationReport(report) {
            this.report = report;
        }
        PropertyConfigurationReport.prototype.logKeys = function (parent, header, list) {
            MirrorJS.Html.addH3(parent, header);
            Object.keys(list).sort().forEach(function (value) {
                MirrorJS.Html.addSpan(parent, value + '\n');
            });
        };
        PropertyConfigurationReport.prototype.addSection = function (parent, query, sectionType) {
            var _this = this;
            var ranks = this.report.config["ranks"];
            var labels = this.report.config["labels"];
            ranks.forEach(function (rank) {
                var values = query(rank);
                if (values.length === 0) {
                    return;
                }
                MirrorJS.Html.addH3(parent, labels[rank] + " (" + _this.report.getSourcesName(rank) + "): " + values.length + " issues\r\n");
                var totalSpan = document.getElementById(sectionType + "Sources" + rank);
                if (totalSpan) {
                    totalSpan.textContent = values.length.toString();
                }
                values.sort().forEach(function (value) {
                    MirrorJS.Html.addSpan(parent, value + "\r\n");
                });
            });
        };
        PropertyConfigurationReport.prototype.mergeFragment = function (parent, typeName, propertyName, propertySources, predicate) {
            var _this = this;
            var exists = this.report.getSourcesName(propertySources);
            var iterator = this.report.mergeProperty(typeName, propertyName, predicate);
            iterator.forEach(function (modifier, sources) {
                var modified = _this.report.getSourcesName(sources);
                if (sources !== propertySources) {
                    MirrorJS.Html.addSpan(parent, "" + typeName + "." + propertyName + ":\n");
                    MirrorJS.Html.addSpan(parent, "    " + modifier + " on " + modified + ".\n");
                    return true;
                }
            });
        };
        PropertyConfigurationReport.prototype.generate = function (parent) {
            var _this = this;
            this.report.forEachProperty(function (typeName, propertyName, propertySources) {
                switch (propertySources) {
                    case 3:
                    case 5:
                    case 7:
                        break;
                    default:
                        // otherwise we're not interested.
                        return;
                }
                _this.mergeFragment(parent, typeName, propertyName, propertySources, function (propertyModel) {
                    return propertyModel.isWritable ? ["writable"] : [];
                });
            });
        };
        return PropertyConfigurationReport;
    })();
    MirrorJS.PropertyConfigurationReport = PropertyConfigurationReport;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=PropertyConfigurationReport.js.map