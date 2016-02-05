"use strict";
var MirrorJS;
(function (MirrorJS) {
    var ProgressReport = (function () {
        function ProgressReport(report) {
            this.report = report;
        }
        ProgressReport.prototype.logKeys = function (parent, header, list) {
            MirrorJS.Html.addH3(parent, header);
            Object.keys(list).sort().forEach(function (value) {
                MirrorJS.Html.addSpan(parent, value + '\n');
            });
        };
        ProgressReport.prototype.addSection = function (parent, query, sectionType) {
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
        ProgressReport.prototype.generate = function (parent) {
            var bucketized = new MirrorJS.BucketizedTypeData(this.report);
            this.addSection(parent, function (sources) {
                return Object.keys(bucketized.getTypesForSources(sources));
            }, "Type");
            this.addSection(parent, function (sources) {
                return Object.keys(bucketized.getPropertiesForSources(sources));
            }, "Property");
        };
        return ProgressReport;
    })();
    MirrorJS.ProgressReport = ProgressReport;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=ProgressReport.js.map