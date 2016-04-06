"use strict";
var MirrorJS;
(function (MirrorJS) {
    var Exporter = (function () {
        function Exporter() {
        }
        Exporter.modelToJSON = function () {
            var types = new MirrorJS.TypeContext(self);
            var model = types.exportReflection();
            return JSON.stringify(model, null, 2);
        };
        Exporter.uploadModel = function (filename) {
            var json = Exporter.modelToJSON();
            MirrorJS.Utils.Xhr("Export.aspx?filename=" + filename, function (response) {
                MirrorJS.Utils.log(response);
            }, json);
        };
        Exporter.downloadModel = function (filename) {
            var json = Exporter.modelToJSON();
            MirrorJS.Utils.download(filename, json);
        };
        return Exporter;
    })();
    MirrorJS.Exporter = Exporter;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=Exporter.js.map