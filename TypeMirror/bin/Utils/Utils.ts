"use strict";

module MirrorJS {
    export class Utils {
        /** On IE, used by getNameFromCtor() to scrape the type name from a ctor instance. */
        private static functionNameRegex = /function\s([^ (]{1,})\s*\(/;

        /** On IE, used by getNameFromCtor() to scrape the type name from a ctor instance. */
        private static objectNameRegex = /\[object ([^ \]]{1,})\s*/;

        /** A sentinel key used by memoize0 when interning functions with no arguments. */
        private static noArgSentinel = "memoize0_key";

        /** A sentinel value used to cache the value "undefined" when interning. */
        private static undefinedSentinel = {};

        private static getLog(create?: boolean) {
            var log = document.getElementById("log");
            if (!log && create) {
                log = document.createElement("div");
                document.body.appendChild(log);
            }
            return log;
        }

        public static log(text: string) {
            var log = Utils.getLog();
            if (log) {
                log.innerHTML += text;
                Html.addBr(log);
            }
            console.log(text);
        }

        /** Helper that adds HTML displaying "{errorType}: {text}" and scrolls the window to ensure it is visible.
            If errorType is ommitted, it defaults to 'Script Error' */
        static logError(text: string, errorType?: string) {
            errorType = errorType || "Script Error";

            var log = Utils.getLog(true);
            var div = Html.addDiv(log);

            var labelSpan = Html.addSpan(div, errorType + ": ");
            labelSpan.style.color = "red";

            Html.addSpan(div, text);

            window.scrollTo(labelSpan.clientLeft, labelSpan.clientTop);
        }

        public static setupErrorHandler() {
            window.onerror =
                (event: Event, source: string, fileno: number, columnNumber: number) => {
                    Utils.logError(source + "(" + fileno + "," + columnNumber + "): " + event.toString());
                };
        }

        public static Xhr(url: string, done: (text: string) => void, data?: any) {
            var errorHandler = (xhr: XMLHttpRequest) => {
                Utils.logError(xhr.statusText, "Server Error");
                if (xhr.responseText) {
                    Utils.logError(xhr.responseText, "Server Error");
                }
            };

            var xhr = new XMLHttpRequest();

            var method =
                data === undefined
                    ? "GET"
                    : "POST";

            var cacheWorkaround =
                data === undefined
                    ? "?date=" + Date.now()
                    : "";

            xhr.open(method, url + cacheWorkaround, true);

            xhr.onload = (e) => {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        done(xhr.responseText);
                    } else {
                        errorHandler(xhr);
                        done(undefined);
                    }
                }
            };

            xhr.onerror = (e) => {
                errorHandler(xhr);
                done(undefined);
            };

            xhr.send(data || null);
        }

        /** Returns true if running on Chrome.  This is largely used in work-arounds for Chromium #43394. */
        private static isChrome() {
            return Utils.isChromeInternal(Utils.detectBrowser());
        }

        public static workaroundChromium43394() {
            if (!Utils.isChrome()) {
                return false;
            }

            var ver = Utils.getChromeVersion();

            // Chromium #43394 was fixed in version 43.
            return ver < 43;
        }

        public static isFromChrome(reflection: IReflection) {
            return Utils.isChromeInternal(reflection.browserVersion);
        }

        private static isChromeInternal(browserVersion: string) {
            return browserVersion.substring(0, 6).toLowerCase() === "chrome";
        }

        private static getChromeVersion() {
            return parseInt(window.navigator.appVersion.match(/Chrome\/(\d+)\./)[1], 10);
        }

        public static is(instance: any, typeName: string) {
            var typeCtor = window[typeName];

            if (!typeCtor) {
                console.assert(Utils.isChrome(),
                    "Global types must be reachable via 'window' (except on Chrome).");

                return undefined;
            }

            var actualCtor = instance.constructor;

            console.assert(actualCtor,
                "'instance' must have a defined constructor.");

            // Method 1: Both types have the same constructor.
            var result1 = actualCtor === typeCtor;

            // Method 2: Object stringifier contains the type name.
            var result2 = Utils.getTypeNameFromInstance(instance) === typeName;

            if (result1 !== result2) {
                if (Utils.isChrome()) {
                    return result2;
                }

                console.assert(false,
                    "Conflicting type results.  '" + Utils.getTypeNameFromInstance(instance) + "' vs. '" + typeName + "'.");
            }

            return result1;
        }

        /** For a given 'instance', asserts that it has the same constructor as the given 'typeName'. */
        public static assertType(instance: any, typeName: string) {
            var is = Utils.is(instance, typeName);

            if (is === undefined && Utils.isChrome()) {
                is = true;
            }

            console.assert(is,
                "Constructed type '" + Utils.getTypeNameFromCtor(instance.constructor) + "' does not match expected type '" + typeName + "'.")

            return instance;
        }

        public static formatAsPercentage(value: number, denominator?: number) {
            if (denominator !== undefined) {
                value /= denominator;
            }

            value = Math.round(value * 1000) / 10;

            return value.toString() + "%";
        }

        public static getTypeNameFromCtor(ctor: any): string {
            if (ctor instanceof Function) {
                return this.getPropertyNameFromFunction(ctor);
            }
            else {
                console.assert(ctor instanceof Object,
                    "All script types must inherit from Object.");

                return this.getTypeNameFromInstance(ctor);
            }
        }

        /** Returns a Function instance, returns the function's name. */
        public static getPropertyNameFromFunction(fn: Function) {
            // Use the ES6 'Function.name' property if supported.
            var name = fn["name"];
            if (name) {
                return name;
            }

            var stringifiedFn;

            // Otherwise, stringify the function and scrape its name using a regex.
            try {
                stringifiedFn = Function.prototype.toString.apply(fn);
            } catch (error) {
                console.assert(false, "Could not invoke Function's toString() on type '" + (typeof fn) + "'.");
                stringifiedFn = fn.toString();
            }

            return Utils.getFirstCapture(Utils.functionNameRegex, stringifiedFn);
        }

        public static getTypeNameFromInstance(instance: Object) {
            var type = typeof instance;

            // If the type is an object, extract the type name from Object's stringifier.
            if (instance !== null && type === "object") {
                var stringifiedObj = Object.prototype.toString.apply(instance);
                type = Utils.getFirstCapture(Utils.objectNameRegex, stringifiedObj);
            }

            return type;
        }

        /** Returns a function that memoizes the result of the given no-argument function 'fn' */
        public static memoize0<TOut>(map: any, fn: () => TOut) {
            return () => {
                return Utils.memoizeBody(map, Utils.noArgSentinel, (unused: string) => { return fn() });
            };
        }

        /** Returns a function that memoizes the result of the given single argument function 'fn' */
        public static memoize1<TIn, TOut>(map: any, fn: (arg: TIn) => TOut) {
            return (arg: TIn) => { return Utils.memoizeBody(map, arg, fn) };
        }

        /** Common lambda implementation for memoize0 and memoize1. */
        private static memoizeBody<TIn, TOut>(map: any, arg: any, fn: (arg: TIn) => TOut): TOut {
            var cachedValue = map[arg];

            if (cachedValue === undefined) {
                cachedValue = fn(arg);
                map[arg] = cachedValue === undefined ? Utils.undefinedSentinel : cachedValue;
                return cachedValue;
            }

            return cachedValue === Utils.undefinedSentinel ? undefined : cachedValue;
        }

        /** Executes the given RegExp on the provided string and returns the first captured value. */
        public static getFirstCapture(regex: RegExp, str: string) {
            var results = regex.exec(str);
            return (results && results.length > 1) ? results[1] : undefined;
        }

        /** Prompt the user to save the given 'text' file with the default 'filename'.  Uses
            FileSaver.js under the covers to work around quirks in IE. */
        public static download(filename, text) {
            var blob = new Blob([text], {
                type: "text/plain;charset=utf-8;",
            });

            window["saveAs"](blob, filename);
        }

        public static detectBrowser() {
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
        }

        static getExportFilename() {
            return Utils.detectBrowser()
                .split(" ")
                .join("")
                .concat(".js");
        }

        public static getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
            var results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        public static getSetBits(mask: number): number[]{
            console.assert((mask | 0) == mask,
                "'mask' must be a 32b unsigned integer.");

            var positions = [];

            for (var i = 0; i < 32; i++) {
                var bit = (1 << i);
                if ((mask & bit) != 0) {
                    positions.push(i);
                }
            }

            return positions;
        }

        public static stringToIntList(str: string) {
            return str
                .split(",")
                .map(
                    (value, index, array) => {
                        return <any>(new Number(value)) | 0;
                    });
        }
    }
}
