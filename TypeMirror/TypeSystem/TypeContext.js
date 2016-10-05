"use strict";
var MirrorJS;
(function (MirrorJS) {
    var TypeContext = (function () {
        function TypeContext(root) {
            var _this = this;
            this.root = root;
            // The following maps are used by the Utils.memoize0/1() helper to cache the computations
            // of various functions.  (Search for thier use, there is only one.)
            this.internedRootTypes = {};
            this.internedNames = {};
            this.internedProperties = {};
            this.internedInstances = {};
            this.internedBaseTypes = {};
            this.internedDerivedTypes = {};
            /** Return info for properties owned by the the given type name.  The result is a map of property
                names to a 'Confidence' enum that indicates how confident we are that the property is in fact
                owned by the specified type. */
            this.getOwnProperties = MirrorJS.Utils.memoize1(this.internedProperties, function (typeName) {
                var ctor = _this.getCtor(typeName);
                var nameToModel = {};
                // First use Object.getOwnPropertyNames() to discover properties from the prototype.
                //
                // Note that on Chrome, a couple of types like 'XMLHttpRequestEventTarget' are reachable via the prototype
                // chain but not reachable via window['XMLHttpRequestEventTarget'].  Hence 'type' might be undefined.
                if (ctor !== undefined) {
                    var prototype = ctor.prototype;
                    if (prototype) {
                        Object.getOwnPropertyNames(prototype).forEach(function (propertyName, index, array) {
                            if (propertyName === "MirrorJS") {
                                return;
                            }
                            try {
                                var descriptor = Object.getOwnPropertyDescriptor(prototype, propertyName);
                            } catch (ignored) {}
                            nameToModel[propertyName] = new MirrorJS.PropertyModel(4 /* Prototype */, undefined, descriptor);
                        });
                    }
                    // Process static members and instance members on static types
                    Object.getOwnPropertyNames(ctor).forEach(function (propertyName, index, array) {
                            if (propertyName === "MirrorJS") {
                                return;
                            }
                            var model = nameToModel[propertyName];
                            if (model === undefined) {
                                //var result = ctor.hasOwnProperty(propertyName);
                                var result = _this.isPropertyInherited(typeName, propertyName);
                                if (!ctor.hasOwnProperty(propertyName)) { //(result.isInherited) {
                                    return;
                                }
                                try {
                                    var descriptor = Object.getOwnPropertyDescriptor(ctor, propertyName);
                                } catch (ignored) {}
                                model = new MirrorJS.PropertyModel(result.confidence, undefined, descriptor);
                                nameToModel[propertyName] = model;
                            }
                            try {
                                if (!_this.preventAV(typeName, propertyName)) {
                                    nameToModel[propertyName].type = MirrorJS.Utils.getTypeNameFromInstance(ctor[propertyName]);
                                }
                            }
                            catch (ignored) {
                                // REVIEW: On Firefox, this throws a security exception.
                                console.assert(typeName === "XSLTProcessor" && propertyName === "flags");
                            }
                        });
                }
                // Next, we attempt to enumerate the properties of an instance of an object to discover any properties
                // that didn't appear on the prototype.  This is a workaround for Chromium #43394, but it doesn't hurt
                // to vet other browsers when we can.
                var instance = _this.getInstance(typeName);
                if (instance) {
                    for (var propertyName in instance) {
                        if (propertyName === "MirrorJS") {
                            continue;
                        }
                        var model = nameToModel[propertyName];
                        if (model === undefined) {
                            var result = _this.isPropertyInherited(typeName, propertyName);
                            if (result.isInherited) {
                                continue;
                            }
                            if (instance.__proto__) {
                                try {
                                    var descriptor = Object.getOwnPropertyDescriptor(instance.__proto__, propertyName);
                                }
                                catch (ignored) {}
                                if (!descriptor) {
                                    try{
                                        descriptor = Object.getOwnPropertyDescriptor(instance, propertyName);
                                    }
                                    catch(e){
                                        //console.warn("" + propertyname);
                                    }
                                }
                                model = new MirrorJS.PropertyModel(result.confidence, undefined, descriptor);
                                nameToModel[propertyName] = model;
                            }
                        }
                        try {
                            if (!_this.preventAV(typeName, propertyName)) {
                                nameToModel[propertyName].type = MirrorJS.Utils.getTypeNameFromInstance(instance[propertyName]);
                            }
                        }
                        catch (ignored) {
                            // REVIEW: On Firefox, this throws a security exception.
                            console.assert(typeName === "XSLTProcessor" && propertyName === "flags");
                        }
                    }
                }
                else {
                    console.warn("Could not create instance of type '" + typeName + "'.");
                }
                return nameToModel;
            });
            /** Given a reference to a constructor, returns the name of the type it constructs. */
            this.getNameFromCtor = MirrorJS.Utils.memoize1(this.internedNames, function (ctor) {
                return MirrorJS.Utils.getTypeNameFromCtor(ctor);
            });
            /** Returns an array of type names reachable from the global window object. */
            this.getRootTypes = MirrorJS.Utils.memoize0(this.internedRootTypes, function () {
                // Iterate through each property of the root object.
                var propertyNames = Object.getOwnPropertyNames(_this.root);
                // Exclude those that our heuristics tell us do not appear to be ctors.
                var ctors = propertyNames.filter(function (propertyName, index, array) {
                    return _this.getIsConstructor(propertyName);
                });
                // Sort and return the remaining names.
                return ctors.sort();
            });
            /** For a given type name, returns the name of its base type. */
            this.getBaseType = MirrorJS.Utils.memoize1(this.internedBaseTypes, function (typeName) {
                var ctor = _this.getCtor(typeName);
                if (!ctor) {
                    console.assert(MirrorJS.Utils.workaroundChromium43394(), "Only Chrome has cases where we are unable to find a ctor for a type.");
                    return;
                }
                var proto = ctor.prototype;
                if(!proto){
                    return;
                }
                var base = Object.getPrototypeOf(proto);
                if (!base) {
                    console.assert(typeName === "Object", "All types must inherit from Object (except Object itself).");
                    return undefined;
                }
                var baseCtor = base.constructor;
                return _this.getNameFromCtor(baseCtor);
            });
            /** For a given type name, returns the names of any derived types. */
            this.getDerivedTypes = MirrorJS.Utils.memoize1(this.internedDerivedTypes, function (typeName) {
                return _this.getRootTypes().filter(function (candidate, index, array) {
                    return _this.getBaseType(candidate) === typeName;
                }).sort();
            });
            /** For a given 'typeName', returns a (not necessarily unique) instance of the type. */
            this.getInstance = MirrorJS.Utils.memoize1(this.internedInstances, function (typeName) {
                var instance = MirrorJS.Instances.getInstance(typeName);
                if (instance !== undefined) {
                    var instanceTypeName = MirrorJS.Utils.getTypeNameFromInstance(instance);
                }
                return instance;
            });
        }
        /** Wrapped call to 'console.info' so it can be conveniently squelched if diagnostic info gets out of hand.
            For now, it's always logged. */
        TypeContext.prototype.trace = function (message) {
            console.info(message);
        };
        TypeContext.prototype.instanceHasProperty = function (typeName, propertyName) {
            var instance = this.getInstance(typeName);
            if (!instance) {
                return undefined;
            }
            return instance[propertyName] !== undefined;
        };
        TypeContext.prototype.allDerivedInstancesHaveProperty = function (typeName, propertyName) {
            // If we can create an instance of this type, and...
            // ...it definitively has the property, all subtypes inherit the proprety, or...
            // ...it definitively is missing the property, then all derived types do not have the property.
            var _this = this;
            var instanceHasProperty = this.instanceHasProperty(typeName, propertyName);
            if (instanceHasProperty !== undefined) {
                return instanceHasProperty;
            }
            // Otherwise, we were unable to create an instance of this type.  Recurse to this types derived
            // types for further evidence.
            return this.getDerivedTypes(typeName).reduce(function (previousValue, derivedName, index, array) {
                var derivedHaveProperty = _this.allDerivedInstancesHaveProperty(derivedName, propertyName);
                console.assert(derivedHaveProperty === true || derivedHaveProperty === false);
                return previousValue && derivedHaveProperty;
            }, true);
        };
        /** True if the given 'propertyName' exists on any parent in the prototype chain of 'typeName'. */
        TypeContext.prototype.isPropertyInherited = function (typeName, propertyName) {
            var _this = this;
            var baseType = this.getBaseType(typeName);
            // If there is no base type, there is 0% probability that the property was inherited.
            if (!baseType) {
                console.assert(typeName === "Object", "All types must inherit from Object (exempting Object).");
                return { isInherited: false, confidence: 3 /* InstanceWithBase */ };
            }
            for (var ancestor = baseType; ancestor; ancestor = this.getBaseType(ancestor)) {
                // We use our 'getOwnProperties' helper which produces a merged list of properties discovered
                // via prototypes and by enumerating instances.
                var baseProperties = this.getOwnProperties(ancestor);
                var propertyModel = baseProperties[propertyName];
                if (propertyModel !== undefined) {
                    switch (propertyModel.confidence) {
                        case 4 /* Prototype */:
                            return { isInherited: true, confidence: 4 /* Prototype */ };
                        case 2 /* InstanceWithSibling */:
                        case 3 /* InstanceWithBase */:
                        case 1 /* InstanceWithoutBase */:
                            return { isInherited: true, confidence: 3 /* InstanceWithBase */ };
                        default:
                            console.assert(false);
                            return undefined;
                    }
                }
            }
            if (this.instanceHasProperty(baseType, propertyName)) {
                console.assert(MirrorJS.Utils.workaroundChromium43394() && baseType === "Error" && propertyName === "stack", "Properties on the base instance must be discovered by walking the memoized ancestor property models.");
                return { isInherited: true, confidence: 3 /* InstanceWithBase */ };
            }
            // If we are able to create an instance of the base type, and we known that we didn't find the
            // property on the base type, then the property can not be inherited.
            if (this.getInstance(baseType) !== undefined) {
                return { isInherited: false, confidence: 3 /* InstanceWithBase */ };
            }
            // Finally, recursively search sibblings that inherit from the same base type.  If we can
            // determine that any of these are missing the property, it must not be inherited.
            var appearsOnAllPeerTypes = this.getDerivedTypes(baseType).reduce(function (previousValue, derivedName, currentIndex, array) {
                if (derivedName === typeName) {
                    return previousValue;
                }
                return _this.allDerivedInstancesHaveProperty(derivedName, propertyName);
            }, true);
            if (!appearsOnAllPeerTypes) {
                return { isInherited: false, confidence: 2 /* InstanceWithSibling */ };
            }
            return { isInherited: false, confidence: 1 /* InstanceWithoutBase */ };
        };
        TypeContext.prototype.getCtor = function (typeName) {
            var ctor = this.root[typeName];
            if (ctor) {
                return ctor;
            }
            console.assert(MirrorJS.Utils.workaroundChromium43394(), "All types must be accessible from the 'window' (except on Chrome.)");
            var instance = this.getInstance(typeName);
            if (instance && instance.constructor) {
                return instance.constructor;
            }
            console.error("Could not find ctor for '" + typeName + "'.");
            return undefined;
        };
        TypeContext.isReadonly = function (descriptor) {
            if (descriptor.writable) {
                return false;
            }
            if (descriptor.set) {
                return false;
            }
            return true;
        };
        TypeContext.prototype.getAllProperties = function (typeName) {
            var properties = this.getOwnProperties(typeName);
            var baseType = this.getBaseType(typeName);
            if (baseType !== undefined) {
                var allBaseProperties = this.getAllProperties(baseType);
                Object.keys(allBaseProperties).forEach(function (basePropertyName, index, array) {
                    properties[basePropertyName] = allBaseProperties[basePropertyName];
                });
            }
            return properties;
        };
        TypeContext.prototype.preventAV = function (typeName, propertyName) {
            switch (typeName) {
                case "MSGestureEvent":
                    switch (propertyName) {
                        case "gestureObject":
                            return true;
                    }
            }
            return false;
        };
        /** Returns true if the window[propertyName] resembles a constructor.  This method is used
            by getConstructors to filter non-constructible objects like 'Math', etc.

            The hueristics seem to work pretty well on Chrome, IE, and FireFox. */
        TypeContext.prototype.getIsConstructor = function (propertyName) {
            // By convention, the names of constructors begin with an upper-case letter.
            var firstChar = propertyName[0];
            if ((!propertyName.startsWith('webkit') && (firstChar.toLowerCase() === firstChar))) {
                // Begins with a lower-case character.  Exclude it.
                return false;
            }
            var propertyValue = this.root[propertyName];
            // On Chrome, constructors are of type 'function'.
            var isFunction = (typeof propertyValue) === "function";
            // On IE11, they are of type 'object'.
            var isObject = (typeof propertyValue) === "object";
            if (isFunction || isObject) {
                // Skip "static" types 'CSS', 'Debug', 'Intl', 'JSON', 'Math'
                try{
                    var prototype = propertyValue.prototype;
                }
                catch(e){
                    return false;
                }
                if (!prototype && propertyName === "MirrorJS") {
                    this.trace("Skipping '" + propertyName + "': no prototype.");
                    return false;
                }
                return true;
            }
            // Skip "NaN", "Infinity", "ActiveXObject"
            this.trace("Skipping '" + propertyName + "': type '" + (typeof propertyValue) + "' is not function or object.");
            return false;
        };
        /** Returns a snapshot of the type information reachable from the given typeContext. */
        TypeContext.prototype.exportReflection = function () {
            var _this = this;
            var exported = new MirrorJS.ExportedReflection();
            exported.browserVersion = MirrorJS.Utils.detectBrowser();
            exported.timestamp = new Date();
            exported.types = {};
            this.getRootTypes().forEach(function (typeName, index, array) {
                MirrorJS.Utils.log("Exporting '" + typeName + "'");
                exported.types[typeName] = new MirrorJS.TypeModel(_this, typeName);
            });
            exported.fixupProperties();
            return exported;
        };
        /** Calculates our confidence in the accuracy of the information for the given 'typeName'. */
        TypeContext.prototype.getConfidence = function (typeName) {
            // Chromium #43394: DOM attributes are missing from object prototypes.
            if (!MirrorJS.Utils.workaroundChromium43394()) {
                return 4 /* Prototype */;
            }
            // True if we can construct an instance of the requested type.
            var canConstruct = this.getInstance(typeName) !== undefined;
            // True if we can construct the base type of the requested type.
            var baseType = this.getBaseType(typeName);
            var canConstructBase = this.getInstance(baseType) !== undefined;
            if (canConstruct) {
                if (canConstructBase) {
                    return 3 /* InstanceWithBase */;
                }
                return 1 /* InstanceWithoutBase */;
            }
            return 0 /* None */;
        };
        return TypeContext;
    })();
    MirrorJS.TypeContext = TypeContext;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=TypeContext.js.map