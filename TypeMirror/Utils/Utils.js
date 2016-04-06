"use strict";
var MirrorJS;
(function (MirrorJS) {
    var Utils = (function () {
        function Utils() {
        }
        Utils.getLog = function (create) {
            var log = document.getElementById("log");
            if (!log && create) {
                log = document.createElement("div");
                document.body.appendChild(log);
            }
            return log;
        };
        Utils.log = function (text) {
            //Disable logging here
            if (typeof WorkerGlobalScope !== 'undefined' && self instanceof WorkerGlobalScope) {
                if (typeof ServiceWorkerGlobalScope !== 'undefined' && self instanceof ServiceWorkerGlobalScope) {
                    self.port.postMessage({ type: "log", text: text });
                } else {
                    self.postMessage({ type: "log", text: text });
                }
            }
            else {
                var log = Utils.getLog();
                if (log) {
                    log.innerHTML += text;
                    MirrorJS.Html.addBr(log);
                }
                console.log(text);
            }
        };
        /** Helper that adds HTML displaying "{errorType}: {text}" and scrolls the window to ensure it is visible.
            If errorType is ommitted, it defaults to 'Script Error' */
        Utils.logError = function (text, errorType) {
            if (typeof WorkerGlobalScope !== 'undefined' && self instanceof WorkerGlobalScope) {
                if (typeof ServiceWorkerGlobalScope !== 'undefined' && self instanceof ServiceWorkerGlobalScope) {
                    self.port.postMessage({ type: "log", text: text });
                } else {
                    self.postMessage({ type: "error", text: text, "errorType": errorType });
                }
            }
            else {
                errorType = errorType || "Script Error";
                var log = Utils.getLog(true);
                var div = MirrorJS.Html.addDiv(log);
                var labelSpan = MirrorJS.Html.addSpan(div, errorType + ": ");
                labelSpan.style.color = "red";
                MirrorJS.Html.addSpan(div, text);
                window.scrollTo(labelSpan.clientLeft, labelSpan.clientTop);
            }
        };
        Utils.setupErrorHandler = function () {
            window.onerror = function (event, source, fileno, columnNumber) {
                Utils.logError(source + "(" + fileno + "," + columnNumber + "): " + event.toString());
            };
        };
        Utils.Xhr = function (url, done, data) {
            var method = data === undefined ? "GET" : "POST";
            var cacheWorkaround = data === undefined ? "?date=" + Date.now() : "";
            var errorHandler = function (statusText, responseText, done) {
                Utils.logError(statusText, "Server Error");
                if (responseText) {
                    Utils.logError(responseText, "Server Error");
                }
                done(undefined);
            };

            if (typeof ServiceWorkerGlobalScope !== 'undefined' && self instanceof ServiceWorkerGlobalScope) {
                fetch(url + cacheWorkaround, {'method': method, 'body': (data || null)}).then(function(response) {
                    if (response.status === 200) {
                        response.text().then(done);
                    }
                    else {
                        response.text().then(function(text) {
                            errorHandler(response.statusText, text, done);
                        });
                    }}).catch(function(response) {
                        response.text().then(function(text) {
                            errorHandler(response.statusText, text, done);
                        });
                    });
            } else {
                var xhr = new XMLHttpRequest();
                xhr.open(method, url + cacheWorkaround, true);
                xhr.onload = function (e) {
                    if (xhr.readyState === 4) {
                        if (xhr.status === 200) {
                            done(xhr.responseText);
                        }
                        else {
                            errorHandler(xhr.statusText, xhr.responseText, done);
                        }
                    }
                };
                xhr.onerror = function (e) {
                    errorHandler(xhr.statusText, xhr.responseText, done);
                };
                xhr.send(data || null);
            }
        };
        /** Returns true if running on Chrome.  This is largely used in work-arounds for Chromium #43394. */
        Utils.isChrome = function () {
            return Utils.isChromeInternal(Utils.detectBrowser());
        };
        Utils.workaroundChromium43394 = function () {
            if (!Utils.isChrome()) {
                return false;
            }
            var ver = Utils.getChromeVersion();
            // Chromium #43394 was fixed in version 43.
            return ver < 43;
        };
        Utils.isFromChrome = function (reflection) {
            return Utils.isChromeInternal(reflection.browserVersion);
        };
        Utils.isChromeInternal = function (browserVersion) {
            return browserVersion.substring(0, 6).toLowerCase() === "chrome";
        };
        Utils.getChromeVersion = function () {
            return parseInt(self.navigator.appVersion.match(/Chrome\/(\d+)\./)[1], 10);
        };
        Utils.is = function (instance, typeName) {
            var typeCtor = self[typeName];
            if (!typeCtor) {
                console.assert(Utils.isChrome(), "Global types must be reachable via 'window' (except on Chrome).");
                return undefined;
            }
            var actualCtor = instance.constructor;
            console.assert(actualCtor, "'instance' must have a defined constructor.");
            // Method 1: Both types have the same constructor.
            var result1 = actualCtor === typeCtor;
            // Method 2: Object stringifier contains the type name.
            var result2 = Utils.getTypeNameFromInstance(instance) === typeName;
            if (result1 !== result2) {
                if (Utils.isChrome()) {
                    return result2;
                }
                console.assert(false, "Conflicting type results.  '" + Utils.getTypeNameFromInstance(instance) + "' vs. '" + typeName + "'.");
            }
            return result1;
        };
        /** For a given 'instance', asserts that it has the same constructor as the given 'typeName'. */
        Utils.assertType = function (instance, typeName) {
            var is = Utils.is(instance, typeName);
            if (is === undefined && Utils.isChrome()) {
                is = true;
            }
            console.assert(is, "Constructed type '" + Utils.getTypeNameFromCtor(instance.constructor) + "' does not match expected type '" + typeName + "'.");
            return instance;
        };
        Utils.formatAsPercentage = function (value, denominator) {
            if (denominator !== undefined) {
                value /= denominator;
            }
            value = Math.round(value * 1000) / 10;
            return value.toString() + "%";
        };
        Utils.getTypeNameFromCtor = function (ctor) {
            if (ctor instanceof Function) {
                return this.getPropertyNameFromFunction(ctor);
            }
            else {
                console.assert(ctor instanceof Object, "All script types must inherit from Object.");
                return this.getTypeNameFromInstance(ctor);
            }
        };
        /** Returns a Function instance, returns the function's name. */
        Utils.getPropertyNameFromFunction = function (fn) {
            // Use the ES6 'Function.name' property if supported.
            var name = fn["name"];
            if (name) {
                return name;
            }
            var stringifiedFn;
            try {
                stringifiedFn = Function.prototype.toString.apply(fn);
            }
            catch (error) {
                console.assert(false, "Could not invoke Function's toString() on type '" + (typeof fn) + "'.");
                stringifiedFn = fn.toString();
            }
            return Utils.getFirstCapture(Utils.functionNameRegex, stringifiedFn);
        };
        Utils.getTypeNameFromInstance = function (instance) {
            var type = typeof instance;
            // If the type is an object, extract the type name from Object's stringifier.
            if (instance !== null && type === "object") {
                var stringifiedObj = Object.prototype.toString.apply(instance);
                type = Utils.getFirstCapture(Utils.objectNameRegex, stringifiedObj);
            }
            return type;
        };
        /** Returns a function that memoizes the result of the given no-argument function 'fn' */
        Utils.memoize0 = function (map, fn) {
            return function () {
                return Utils.memoizeBody(map, Utils.noArgSentinel, function (unused) {
                    return fn();
                });
            };
        };
        /** Returns a function that memoizes the result of the given single argument function 'fn' */
        Utils.memoize1 = function (map, fn) {
            return function (arg) {
                return Utils.memoizeBody(map, arg, fn);
            };
        };
        /** Common lambda implementation for memoize0 and memoize1. */
        Utils.memoizeBody = function (map, arg, fn) {
            var cachedValue = map[arg];
            if (cachedValue === undefined) {
                cachedValue = fn(arg);
                map[arg] = cachedValue === undefined ? Utils.undefinedSentinel : cachedValue;
                return cachedValue;
            }
            return cachedValue === Utils.undefinedSentinel ? undefined : cachedValue;
        };
        /** Executes the given RegExp on the provided string and returns the first captured value. */
        Utils.getFirstCapture = function (regex, str) {
            var results = regex.exec(str);
            return (results && results.length > 1) ? results[1] : undefined;
        };
        /** Prompt the user to save the given 'text' file with the default 'filename'.  Uses
            FileSaver.js under the covers to work around quirks in IE. */
        Utils.download = function (filename, text) {
            var blob = new Blob([text], {
                type: "text/plain;charset=utf-8;",
            });
            window["saveAs"](blob, filename);
        };
        Utils.detectBrowser = function () {
            var ua = navigator.userAgent;
            var matches = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
            if (/trident/i.test(matches[1])) {
                var ieVersion = /\brv[ :]+(\d+)/g.exec(ua) || [];
                return 'IE ' + (ieVersion[1] || '');
            }
            if (matches[1] === 'Chrome') {
                var isOpera = ua.match(/\bOPR\/(\d+)/);
                if (isOpera) {
                    return 'Opera ' + isOpera[1];
                }
                var isEdge = ua.match(/\bEdge\/(\d+)/);
                if (isEdge) {
                    return 'IE' + isEdge[1] + "-Edge";
                }
            }
            matches = matches[2] ? [matches[1], matches[2]] : [navigator.appName, navigator.appVersion, '-?'];
            var version = ua.match(/version\/(\d+)/i);
            if (version != null) {
                matches.splice(1, 1, version[1]);
            }
            return matches.join(' ');
        };
        Utils.getExportFilename = function (context) {
            if (!context) { context = ""; }
            return Utils.detectBrowser().split(" ").join("").concat(context + ".js");
        };
        Utils.getParameterByName = function (name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
            var results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        };
        Utils.getSetBits = function (mask) {
            console.assert((mask | 0) == mask, "'mask' must be a 32b unsigned integer.");
            var positions = [];
            for (var i = 0; i < 32; i++) {
                var bit = (1 << i);
                if ((mask & bit) != 0) {
                    positions.push(i);
                }
            }
            return positions;
        };
        Utils.stringToIntList = function (str) {
            return str.split(",").map(function (value, index, array) {
                return (new Number(value)) | 0;
            });
        };
        /** On IE, used by getNameFromCtor() to scrape the type name from a ctor instance. */
        Utils.functionNameRegex = /function\s([^ (]{1,})\s*\(/;
        /** On IE, used by getNameFromCtor() to scrape the type name from a ctor instance. */
        Utils.objectNameRegex = /\[object ([^ \]]{1,})\s*/;
        /** A sentinel key used by memoize0 when interning functions with no arguments. */
        Utils.noArgSentinel = "memoize0_key";
        /** A sentinel value used to cache the value "undefined" when interning. */
        Utils.undefinedSentinel = {};
        return Utils;
    })();
    MirrorJS.Utils = Utils;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=Utils.js.map