"use strict"
self.importScripts(
    'Utils/Utils.js',
    'Utils/MergeIterator.js',
    'Utils/Html.js',
    'TypeSystem/Confidence.js',
    'TypeSystem/PropertyModel.js',
    'TypeSystem/TypeModel.js',
    'TypeSystem/ExportedTypes.js',
    'TypeSystem/Instances.js',
    'TypeSystem/TypeContext.js',
    'Exporter/Exporter.js'
    );

self.port = 0;

self.addEventListener('message', function(event) {
    self.port = event.ports[0];
    if (event.data === 'start') {
        MirrorJS.Exporter.uploadModel(MirrorJS.Utils.getExportFilename("ServiceWorker"));
    }
});
