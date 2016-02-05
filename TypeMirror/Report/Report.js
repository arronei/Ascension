"use strict";
var MirrorJS;
(function (MirrorJS) {
    var Report = (function () {
        function Report(reflections, config, metadata) {
            this.reflections = reflections;
            this.config = config;
            this.metadata = metadata;
        }
        Report.load = function (dataPath, done) {
            var _this = this;
            var configPath = MirrorJS.Utils.getParameterByName("config");
            if (configPath) {
                MirrorJS.Utils.Xhr(dataPath + configPath, function (json) {
                    config = JSON.parse(json);
                    config.dataPath = dataPath;
                    _this.loadInternal(config, done);
                });
            }
            else {
                var config = {
                    dataPath: dataPath,
                    sources: MirrorJS.Utils.getParameterByName("sources").split(","),
                    metadata: MirrorJS.Utils.getParameterByName("metadata"),
                };
                this.loadInternal(config, done);
            }
        };
        Report.loadInternal = function (config, done) {
            var reflections = [];
            var metadata = { types: {} };
            var remaining = 0;
            var liveIndices = [];
            var checkDone = function () {
                if (remaining === 0) {
                    var report = new Report(reflections, config, metadata);
                    done(report);
                }
            };
            config.sources.forEach(function (sourcePath, index) {
                remaining++;
                // If we encounter the "live" source, remember its index.
                if (sourcePath === ".") {
                    liveIndices.push(index);
                    return;
                }
                (function (copyOfIndex) {
                    MirrorJS.Utils.Xhr(config.dataPath + sourcePath, function (json) {
                        reflections[copyOfIndex] = JSON.parse(json);
                        remaining--;
                        checkDone();
                    });
                })(index);
            });
            var metadataPath = MirrorJS.Utils.getParameterByName("metadata");
            if (metadataPath) {
                remaining++;
                MirrorJS.Utils.Xhr(config.dataPath + metadataPath, function (json) {
                    metadata = JSON.parse(json);
                    remaining--;
                    checkDone();
                });
            }
            liveIndices.forEach(function (liveIndex, index, array) {
                reflections[liveIndex] = new MirrorJS.TypeContext(window).exportReflection();
                remaining--;
                checkDone();
            });
        };
        Report.prototype.getMaxSourcesMask = function () {
            return (1 << this.reflections.length) - 1;
        };
        Report.prototype.getSourcesName = function (sources) {
            var maxMask = this.getMaxSourcesMask();
            console.assert(1 <= sources && sources <= maxMask);
            var browsers = this.getBrowserVersions();
            var name = "";
            for (var i = 0; i <= browsers.length; i++) {
                var currentMask = (1 << i);
                if ((sources & currentMask) !== 0) {
                    if (name.length > 0) {
                        name += " and ";
                    }
                    name += browsers[i];
                }
            }
            console.assert(name.length > 0);
            return name;
        };
        Report.prototype.merge = function (selector, filter) {
            var sources = this.reflections.reduce(function (previousSources, currentReflection, currentIndex, array) {
                var types = currentReflection.types;
                var source = selector(types).sort();
                previousSources.push(source);
                return previousSources;
            }, []);
            console.assert(sources.length === this.reflections.length, "The number of sources must be the same as the number of inputs.");
            return new MirrorJS.MergeIterator(sources, filter);
        };
        Report.prototype.mergeType = function (typeName, selector, filter) {
            return this.merge(function (types) {
                return Object.hasOwnProperty.call(types, typeName) ? selector(types, types[typeName]) : [];
            }, filter);
        };
        Report.prototype.mergeProperty = function (typeName, propertyName, selector) {
            return this.mergeType(typeName, function (types, type) {
                return Object.hasOwnProperty.call(type.properties, propertyName) ? selector(type.properties[propertyName]) : [];
            });
        };
        Report.getPropertyNamesUnsorted = function (types, type, includeInherited) {
            var properties = Object.keys(type.properties);
            var baseName = type.baseType;
            if (includeInherited && (baseName !== undefined)) {
                var baseType = this.getTypeData(types, baseName);
                if (baseType !== undefined) {
                    var allBaseProperties = this.getPropertyNamesUnsorted(types, baseType, includeInherited);
                    properties = properties.concat(allBaseProperties);
                }
            }
            return properties;
        };
        Report.getPropertyNames = function (types, type, includeInherited) {
            return Report.getPropertyNamesUnsorted(types, type, includeInherited).sort();
        };
        Report.getTypeData = function (types, typeName) {
            return Object.hasOwnProperty.call(types, typeName) ? types[typeName] : undefined;
        };
        Report.prototype.getTypeMetadata = function (typeName) {
            var types = this.metadata["types"];
            return Object.hasOwnProperty.call(types, typeName) ? types[typeName] : undefined;
        };
        Report.prototype.getPropertyMetadata = function (typeName, propertyName) {
            var type = this.getTypeMetadata(typeName);
            if (type === undefined) {
                return;
            }
            var properties = type["properties"];
            return Object.hasOwnProperty.call(properties, propertyName) ? properties[propertyName] : undefined;
        };
        Report.prototype.getType = function (reflectionIndex, typeName) {
            var reflection = this.reflections[reflectionIndex];
            var types = reflection.types;
            if (!Object.hasOwnProperty.call(types, typeName)) {
                return undefined;
            }
            return reflection.types[typeName];
        };
        Report.prototype.getBrowserVersions = function () {
            return this.reflections.map(function (reflection, index, array) {
                return reflection.browserVersion;
            });
        };
        Report.prototype.forEachProperty = function (callback) {
            var _this = this;
            var typeNames = this.merge(function (types) { return Object.keys(types); });
            typeNames.forEach(function (typeName, typeSources) {
                var propertyNames = _this.mergeType(typeName, function (types, type) {
                    return Report.getPropertyNames(types, type, false);
                });
                propertyNames.forEach(function (propertyName, propertySources) {
                    callback(typeName, propertyName, propertySources);
                    return true;
                });
                return true;
            });
        };
        return Report;
    })();
    MirrorJS.Report = Report;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=Report.js.map