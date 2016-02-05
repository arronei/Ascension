"use strict";
var MirrorJS;
(function (MirrorJS) {
    var Viewer = (function () {
        function Viewer() {
        }
        Viewer.labelColorKeys = function (report) {
            var browsers = report.getBrowserVersions();
            var views = MirrorJS.Utils.getParameterByName("views");
            var select = document.getElementById("sourceFilterSelect");
            var maxMask = report.getMaxSourcesMask();
            for (var comboMask = 1; comboMask <= maxMask; comboMask++) {
                var elementId = "sources" + comboMask;
                var name = report.getSourcesName(comboMask);
                var label = document.getElementById(elementId);
                if (label) {
                    label.textContent = name;
                }
                if (views.indexOf(comboMask.toString()) >= 0) {
                    var option = document.createElement("option");
                    option.textContent = name + " Only";
                    option.value = comboMask.toString();
                    select.appendChild(option);
                }
            }
        };
        return Viewer;
    })();
    MirrorJS.Viewer = Viewer;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=Viewer.js.map