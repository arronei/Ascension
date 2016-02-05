"use strict";
var MirrorJS;
(function (MirrorJS) {
    var WebIDLView = (function () {
        function WebIDLView(output, report) {
            var _this = this;
            this.output = output;
            this.report = report;
            this.typeNames = new MirrorJS.MergeIterator([]);
            this.currentTypePattern = "";
            this.currentTypeFilter = undefined;
            this.currentPropertyPattern = "";
            this.currentPropertyFilter = undefined;
            this.currentSourcesFilter = undefined;
            this.currentSourcesFilterMask = undefined;
            this.showInherited = false;
            window.addEventListener("message", function (ev) {
                var command = ev.data.command;
                switch (command) {
                    case "update": {
                        var start = Date.now();
                        while (true) {
                            // Every 100ms, break the current turn to allow the screen to repaint.
                            if ((Date.now() - start) > 10) {
                                _this.updateAsync();
                                break;
                            }
                            // If there are no more types to add in the queue, hide the progress UI and exit.
                            var nextType = _this.typeNames.next();
                            if (!nextType) {
                                break;
                            }
                            _this.addType(nextType.value, nextType.sourcesMask);
                        }
                        break;
                    }
                }
            });
            this.update();
        }
        /** Schedule our async callback to process the queue of types to add. */
        WebIDLView.prototype.updateAsync = function () {
            window.postMessage({
                command: "update",
            }, "*");
        };
        WebIDLView.prototype.update = function () {
            var oldOutput = this.output;
            var parent = oldOutput.parentNode;
            this.output = document.createElement("pre");
            this.output.id = oldOutput.id;
            this.output.className = oldOutput.className;
            parent.replaceChild(this.output, oldOutput);
            this.typeNames = this.report.merge(function (types) { return Object.keys(types); }, this.currentTypeFilter);
            this.updateAsync();
        };
        WebIDLView.prototype.mapSourcesToStyle = function (sources) {
            var allSources = (1 << this.report.reflections.length) - 1;
            if (sources === allSources) {
                return undefined;
            }
            return "sources" + sources;
        };
        /** Inserts an anchor linking to the given type declaration. */
        WebIDLView.prototype.addTypeLink = function (parent, typeName) {
            var anchor = document.createElement("a");
            anchor.href = "#" + typeName;
            anchor.textContent = typeName;
            parent.appendChild(anchor);
            return anchor;
        };
        WebIDLView.prototype.addIcon = function (parent, imageSrc) {
            var img = document.createElement("img");
            img.src = imageSrc;
            parent.appendChild(img);
        };
        WebIDLView.prototype.addIconAndText = function (parent, uri, text) {
            var div = document.createElement("div");
            this.addIcon(div, uri);
            if (text) {
                MirrorJS.Html.addSpan(div, " ");
                MirrorJS.Html.addSpan(div, text);
            }
            parent.appendChild(div);
            return div;
        };
        WebIDLView.prototype.addWarningIcon = function (parent) {
            var div = this.addIcon(parent, "Warning.png");
        };
        WebIDLView.prototype.addWarning = function (parent, text) {
            var div = this.addIconAndText(parent, "Warning.png", text);
            div.className = "warning";
            return div;
        };
        WebIDLView.prototype.addError = function (parent, text) {
            var div = this.addIconAndText(parent, "Error.png", text);
            div.className = "error";
            return div;
        };
        WebIDLView.prototype.addBaseType = function (parent, typeName) {
            var _this = this;
            var baseTypes = this.report.mergeType(typeName, function (types, type) {
                var baseTypeName = type.baseType;
                return (baseTypeName && baseTypeName !== "Object") ? [baseTypeName] : [];
            });
            if (baseTypes.isEmpty()) {
                return;
            }
            var span = MirrorJS.Html.addSpan(parent, " : ");
            baseTypes.forEach(function (baseTypeName, sources) {
                _this.addTypeLink(span, baseTypeName).className = _this.mapSourcesToStyle(sources);
                return true;
            });
        };
        WebIDLView.prototype.addDerivedTypes = function (parent, typeName) {
            var _this = this;
            var derivedTypes = this.report.mergeType(typeName, function (types, type) {
                return type.derivedTypes;
            });
            if (derivedTypes.isEmpty()) {
                return;
            }
            var derivedTypesSpan = MirrorJS.Html.addSpan(parent, "    extended by ");
            derivedTypesSpan.className = "text";
            var first = true;
            derivedTypes.forEach(function (derivedType, sources) {
                if (first) {
                    first = false;
                }
                else {
                    MirrorJS.Html.addSpan(derivedTypesSpan, ", ");
                }
                _this.addTypeLink(derivedTypesSpan, derivedType).className = _this.mapSourcesToStyle(sources);
                return true;
            });
            MirrorJS.Html.addSpan(parent, "\r\n");
        };
        /** Called by addProperty() to decorate a property with any appropriate warnings. */
        WebIDLView.prototype.addPropertyConfidence = function (parent, typeName, propertyName) {
            var _this = this;
            if (this.showInherited) {
                return;
            }
            var iterator = this.report.mergeProperty(typeName, propertyName, function (property) {
                return [JSON.stringify({
                    confidence: property.confidence
                })];
            });
            iterator.forEach(function (json, sources) {
                var flags = JSON.parse(json);
                switch (flags.confidence) {
                    case 4 /* Prototype */:
                    case 3 /* InstanceWithBase */:
                    case 2 /* InstanceWithSibling */:
                        break;
                    case 1 /* InstanceWithoutBase */:
                        _this.addWarningIcon(parent);
                        parent.className += " fromInstanceWithoutBase";
                        break;
                    default:
                        throw new Error();
                }
                return true;
            });
        };
        WebIDLView.prototype.mergeFragment = function (parent, typeName, propertyName, propertyToFragement, className) {
            var _this = this;
            var iterator = this.report.mergeProperty(typeName, propertyName, propertyToFragement);
            iterator.forEach(function (modifier, sources) {
                var outer = MirrorJS.Html.addSpan(parent, "");
                outer.className = _this.mapSourcesToStyle(sources);
                MirrorJS.Html.addSpan(outer, modifier).className = className;
                MirrorJS.Html.addSpan(parent, " ");
                return true;
            });
        };
        WebIDLView.prototype.addWritableModifier = function (parent, typeName, propertyName) {
            this.mergeFragment(parent, typeName, propertyName, function (property) {
                return property.isWritable ? ["writable"] : [];
            }, "keyword");
        };
        WebIDLView.prototype.addEnumerableModifier = function (parent, typeName, propertyName) {
            this.mergeFragment(parent, typeName, propertyName, function (property) {
                return property.isEnumerable ? ["enumerable"] : [];
            }, "keyword");
        };
        WebIDLView.prototype.addConfigurableModifier = function (parent, typeName, propertyName) {
            this.mergeFragment(parent, typeName, propertyName, function (property) {
                return property.isEnumerable ? ["configurable"] : [];
            }, "keyword");
        };
        WebIDLView.prototype.addOpenBrace = function (parent, typeName, propertyName) {
            this.mergeFragment(parent, typeName, propertyName, function (property) {
                return property.hasGet || property.hasSet ? [" {"] : [];
            }, "");
        };
        WebIDLView.prototype.addGetAccessor = function (parent, typeName, propertyName) {
            this.mergeFragment(parent, typeName, propertyName, function (property) {
                return property.hasGet ? ["get;"] : [];
            }, "");
        };
        WebIDLView.prototype.addSetAccessor = function (parent, typeName, propertyName) {
            this.mergeFragment(parent, typeName, propertyName, function (property) {
                return property.hasSet ? ["set;"] : [];
            }, "");
        };
        WebIDLView.prototype.addCloseBrace = function (parent, typeName, propertyName) {
            this.mergeFragment(parent, typeName, propertyName, function (property) {
                return property.hasGet || property.hasSet ? ["}"] : [];
            }, "");
        };
        WebIDLView.prototype.addPropertyType = function (parent, typeName, propertyName) {
            var _this = this;
            var iterator = this.report.mergeProperty(typeName, propertyName, function (property) {
                return property.type ? [property.type] : [];
            });
            iterator.forEach(function (typeName, sources) {
                var outer = MirrorJS.Html.addSpan(parent, "");
                outer.className = _this.mapSourcesToStyle(sources);
                var style = "";
                switch (typeName) {
                    case "function":
                    case "object":
                    case "number":
                    case "string":
                        style = "keyword";
                        break;
                }
                MirrorJS.Html.addSpan(outer, typeName).className = style;
                MirrorJS.Html.addSpan(parent, " ");
                return true;
            });
        };
        WebIDLView.prototype.addTypeComment = function (parent, typeName) {
            var _this = this;
            var comment = new MirrorJS.Comment(parent, function () {
                var type = _this.report.getTypeMetadata(typeName);
                if (type === undefined) {
                    return undefined;
                }
                return type["comment"];
            });
        };
        WebIDLView.prototype.addPropertyComment = function (parent, typeName, propertyName) {
            var _this = this;
            var comment = new MirrorJS.Comment(parent, function () {
                var property = _this.report.getPropertyMetadata(typeName, propertyName);
                if (property === undefined) {
                    return undefined;
                }
                return property["comment"];
            });
        };
        /** Helper to append content for the given 'propertyName' to the specified 'div'. */
        WebIDLView.prototype.addProperty = function (parent, typeName, propertyName, sources) {
            MirrorJS.Html.addSpan(parent, "    ");
            var span = document.createElement("span");
            span.className = this.mapSourcesToStyle(sources);
            this.addPropertyConfidence(span, typeName, propertyName);
            this.addConfigurableModifier(span, typeName, propertyName);
            this.addWritableModifier(span, typeName, propertyName);
            this.addEnumerableModifier(span, typeName, propertyName);
            this.addPropertyType(span, typeName, propertyName);
            var nameSpan = MirrorJS.Html.addSpan(span, propertyName);
            if (propertyName === "constructor") {
                nameSpan.className = "keyword";
            }
            this.addOpenBrace(span, typeName, propertyName);
            this.addGetAccessor(span, typeName, propertyName);
            this.addSetAccessor(span, typeName, propertyName);
            this.addCloseBrace(span, typeName, propertyName);
            parent.appendChild(span);
            // Add a space and CRLF as a separate span with the non-highligthed style so that annotations
            // typed after this has been copied & pasted into an email appear non-highligted.
            MirrorJS.Html.addSpan(parent, " \r\n");
        };
        WebIDLView.prototype.addTypeConfidence = function (parent, typeName) {
            var _this = this;
            this.report.reflections.forEach(function (reflection, index, array) {
                var type = reflection.types[typeName];
                if (type === undefined) {
                    return;
                }
                if (!_this.showInherited) {
                    if (MirrorJS.TypeModel.mayReportInheritedPropertiesAsOwn(type)) {
                        MirrorJS.Html.addSpan(parent, "    ");
                        var div = _this.addWarning(parent, "Properties from " + reflection.browserVersion + " appearing on this type may actually be inherited from ");
                        _this.addTypeLink(div, type.baseType);
                        MirrorJS.Html.addSpan(div, ".");
                        MirrorJS.Html.addSpan(parent, "\r\n");
                    }
                }
                if (MirrorJS.TypeModel.mayBeMissingProperties(type)) {
                    MirrorJS.Html.addSpan(parent, "    ");
                    _this.addError(parent, "Properties from " + reflection.browserVersion + " may be missing.");
                    MirrorJS.Html.addSpan(parent, "\r\n");
                }
            });
        };
        WebIDLView.prototype.addProperties = function (parent, typeName, properties) {
            var _this = this;
            var table = document.createElement("table");
            parent.appendChild(table);
            properties.forEach(function (propertyName, sources) {
                var row = document.createElement("tr");
                table.appendChild(row);
                var td = document.createElement("td");
                td.className = "codeColumn";
                row.appendChild(td);
                _this.addProperty(td, typeName, propertyName, sources);
                var td = document.createElement("td");
                td.className = "commentColumn";
                row.appendChild(td);
                _this.addPropertyComment(td, typeName, propertyName);
                return true;
            });
        };
        /** Helper to append content for the given 'typeName' to the specified 'div'. */
        WebIDLView.prototype.addType = function (typeName, sources) {
            var _this = this;
            var properties = this.report.mergeType(typeName, function (types, type) {
                var result = MirrorJS.Report.getPropertyNames(types, type, _this.showInherited);
                if (_this.currentPropertyFilter !== undefined) {
                    result = result.filter(_this.currentPropertyFilter);
                }
                return result;
            }, this.currentSourcesFilter);
            if ((this.currentSourcesFilter || this.currentPropertyFilter) && properties.isEmpty()) {
                return;
            }
            // Otherwise, create a div for the next type and populate it.
            var parent = document.createElement("div");
            parent.id = typeName;
            parent.className = this.mapSourcesToStyle(sources);
            var table = document.createElement("table");
            parent.appendChild(table);
            var row = document.createElement("tr");
            table.appendChild(row);
            var td = document.createElement("td");
            td.className = "codeColumn";
            row.appendChild(td);
            var span = MirrorJS.Html.addSpan(td, "interface ").className = "keyword";
            MirrorJS.Html.addSpan(td, typeName).className = "identifier";
            // Display the base type information.  Note that if diffing, the left/right may disagree.
            this.addBaseType(td, typeName);
            var td = document.createElement("td");
            td.className = "commentColumn";
            row.appendChild(td);
            this.addTypeComment(td, typeName);
            this.addDerivedTypes(parent, typeName);
            MirrorJS.Html.addSpan(parent, "{\r\n");
            this.addTypeConfidence(parent, typeName);
            this.addProperties(parent, typeName, properties);
            MirrorJS.Html.addSpan(parent, "}\r\n");
            this.output.appendChild(parent);
            this.output.appendChild(document.createElement("br"));
        };
        /** Change the current regex used to filter the displayed results. */
        WebIDLView.prototype.setTypeSearchPattern = function (searchPattern) {
            // If the searchPattern hasn't changed, early exit.
            if (searchPattern === this.currentTypePattern) {
                return;
            }
            this.currentTypePattern = searchPattern;
            if (!this.currentTypePattern) {
                this.currentTypeFilter = undefined;
            }
            else {
                try {
                    var regex = new RegExp(this.currentTypePattern);
                    this.currentTypeFilter = function (candidate) {
                        return regex.test(candidate);
                    };
                }
                catch (error) {
                    return;
                }
            }
            this.update();
        };
        /** Change the current regex used to filter the displayed results. */
        WebIDLView.prototype.setPropertySearchPattern = function (searchPattern) {
            // If the searchPattern hasn't changed, early exit.
            if (searchPattern === this.currentPropertyPattern) {
                return;
            }
            this.currentPropertyPattern = searchPattern;
            if (!this.currentPropertyPattern) {
                this.currentPropertyFilter = undefined;
            }
            else {
                try {
                    var regex = new RegExp(this.currentPropertyPattern);
                    this.currentPropertyFilter = function (candidate, index, array) {
                        return regex.test(candidate);
                    };
                }
                catch (error) {
                    return;
                }
            }
            this.update();
        };
        /** Change the current regex used to filter the displayed results. */
        WebIDLView.prototype.setApiFilterMask = function (sourcesMask) {
            var _this = this;
            // If the searchPattern hasn't changed, early exit.
            if (sourcesMask === this.currentSourcesFilterMask) {
                return;
            }
            this.currentSourcesFilterMask = sourcesMask;
            this.currentSourcesFilter = this.currentSourcesFilterMask === 0 ? undefined : function (propertyName, sourcesMask) {
                return sourcesMask === _this.currentSourcesFilterMask;
            };
            this.update();
        };
        WebIDLView.prototype.setShowInherited = function (showInherited) {
            if (showInherited === this.showInherited) {
                return;
            }
            this.showInherited = showInherited;
            this.update();
        };
        return WebIDLView;
    })();
    MirrorJS.WebIDLView = WebIDLView;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=WebIDLView.js.map