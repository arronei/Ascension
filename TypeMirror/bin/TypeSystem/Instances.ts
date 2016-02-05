"use strict";

module MirrorJS {
    export class Instances {
        static audioContext: any;
        static canvas: HTMLCanvasElement;

        /** Maps type names to an instance. */
        static typesToFactories = {
            // Web Audio
            "AudioBuffer": () => { return Instances.getAudioContext().createBuffer(1, 512, 44000); },
            "AudioBufferSourceNode": () => { return Instances.getAudioContext().createBufferSource(); },
            "AudioDestinationNode": () => { return Instances.getAudioContext().destination; },
            "AudioListener": () => { return Instances.getAudioContext().listener; },
            "AudioParam": () => { return Instances.getAudioContext().createGain().gain; },
            "MediaStreamAudioDestinationNode": () => { return Instances.getAudioContext().createMediaStreamDestination(); },
            "AudioWorkerNode": () => { return Instances.getAudioContext().createAudioWorker("script.js"); },
            "ScriptProcessorNode": () => { return Instances.getAudioContext().createScriptProcessor(); },
            "AnalyserNode": () => { return Instances.getAudioContext().createAnalyser(); },
            "GainNode": () => { return Instances.getAudioContext().createGain(); },
            "DelayNode": () => { return Instances.getAudioContext().createDelay(); },
            "BiquadFilterNode": () => { return Instances.getAudioContext().createBiquadFilter(); },
            "WaveShaperNode": () => { return Instances.getAudioContext().createWaveShaper(); },
            "PannerNode": () => { return Instances.getAudioContext().createPanner(); },
            "ConvolverNode": () => { return Instances.getAudioContext().createConvolver(); },
            "ChannelSplitterNode": () => { return Instances.getAudioContext().createChannelSplitter(2); },
            "ChannelMergerNode": () => { return Instances.getAudioContext().createChannelMerger(2); },
            "DynamicsCompressorNode": () => { return Instances.getAudioContext().createDynamicsCompressor(); },
            "OscillatorNode": () => { return Instances.getAudioContext().createOscillator(); },
            "PeriodicWave": () => { return Instances.getAudioContext().createPeriodicWave(new Float32Array(1), new Float32Array(1)); },

            // DOM
            "Attr": () => { return document.createAttribute("unknown-attribute"); },
            "CanvasRenderingContext2D": () => { return Instances.getCanvas().getContext("2d"); },
            "CDATASection": () => {
                var dt = document.implementation.createDocumentType("unknown", "unknown", "unknown");
                var doc = document.implementation.createDocument("unknownQualifier", "unknownName", dt);
                return doc.createCDATASection("");
            },
            "ClientRect": () => { return document.body.getClientRects()[0]; },
            "ClientRectList": () => { return document.body.getClientRects(); },
            "Comment": () => { return document.createComment("unknown comment"); },
            "CSSStyleDeclaration": () => { return window.getComputedStyle(document.body); },
            "DocumentType": () => { return document.implementation.createDocumentType("unknown", "unknown", "unknown"); },
            "DOMException": Instances.catch(() => { return document.body.removeChild(document.body); }),
            "DOMImplementation": () => { return document.implementation; },
            "Element": () => { return document.createElementNS("someUnknownNamespace", "someUnknownElement"); },
            "HTMLDocument": () => { return document; },
            "HTMLUnknownElement": () => { return document.createElementNS("http://www.w3.org/1999/xhtml", "someUnknownElement"); },
            "Location": () => { return window.location; },
            "XMLDocument": () => {
                var dt = document.implementation.createDocumentType("unknown", "unknown", "unknown");
                return document.implementation.createDocument("unknownQualifier", "unknownName", dt);
            },
            "Window": () => { return window; },
            "DOMSettableTokenList": () => { return Instances.getInstance("HTMLLinkElement").sizes; },
            "DOMStringList": () => { return Instances.getInstance("Location").ancestorOrigins; },
            "DOMStringMap": () => { return Instances.getInstance("Element").dataset; },
            "DOMTokenList": () => { return Instances.getInstance("Element").classList; },
            "HTMLCollection": () => { return Instances.getInstance("HTMLDocument").children; },
            "HTMLFormControlsCollection": () => { return Instances.getInstance("HTMLFormElement").elements; },
            "HTMLOptionsCollection": () => { return Instances.getInstance("HTMLSelectElement").options; },
            "History": () => { return Instances.getInstance("Window").history; },
            "IDBFactory": () => { return Instances.getInstance("Window").indexedDB; },
            "MessagePort": () => { return Instances.getInstance("MessageChannel").port1; },
            "NamedNodeMap": () => { return Instances.getInstance("Element").attributes; },
            "Navigator": () => { return Instances.getInstance("Window").navigator; },
            "NodeList": () => { return Instances.getInstance("HTMLDocument").childNodes; },
            "Storage": () => { return Instances.getInstance("Window").localStorage; },
            "StyleSheetList": () => { return Instances.getInstance("HTMLDocument").styleSheets; },
            "TextTrack": () => { return Instances.getInstance("HTMLTrackElement").track; },
            "TextTrackList": () => { return Instances.getInstance("HTMLVideoElement").textTracks; },
            "TimeRanges": () => { return Instances.getInstance("HTMLVideoElement").played; },
            "ValidityState": () => { return Instances.getInstance("HTMLInputElement").validity; },
            "XMLHttpRequestUpload": () => { return Instances.getInstance("XMLHttpRequest").upload; },
            "CSSStyleSheet": () => { return Instances.getInstance("StyleSheetList")[0]; },
            "MimeTypeArray": () => { return Instances.getInstance("Navigator").mimeTypes; },
            "PluginArray": () => { return Instances.getInstance("Navigator").plugins; },
            "ServiceWorkerContainer": () => { return Instances.getInstance("Navigator").serviceWorker; },

            // Unsorted:
            "ApplicationCache": () => { return window.applicationCache; },
            "BarProp": () => { return window["locationbar"]; },

            // Performance
            "Performance": () => { return window.performance; },
            "PerformanceMark": () => {
                window.performance.mark("__dumptypes_mark__");
                return window.performance.getEntriesByName("__dumptypes_mark__")[0];
            },
            "PerformanceMeasure": () => {
                window.performance.mark("__dumptypes_start__");
                window.performance.mark("__dumptypes_end__");
                window.performance.measure("__dumptypes_measure__", "__dumptypes_start__", "__dumptypes_end__");
                return window.performance.getEntriesByName("__dumptypes_measure__")[0];
            },
            "PerformanceNavigation": () => {
                return performance.navigation;
            },
            "PerformanceNavigationTiming": () => {
                try {
                    return performance.getEntriesByType("navigation")[0];
                } catch (error) {
                    // FF39 does not yet implement getEntriesByType().
                    return undefined;
                }
            },
            "PerformanceResourceTiming": () => {
                try {
                    return performance.getEntriesByType("resource")[0];
                } catch (error) {
                    // FF39 does not yet implement getEntriesByType().
                    return undefined;
                }
            },
            "PerformanceTiming": () => {
                return performance.timing;
            },

            // SVG
            "SVGElement": () => { return document.createElementNS("http://www.w3.org/2000/svg", "someUnknownSVGElement") },
            "SVGException": Instances.catch(() => {
                var m = (<SVGSVGElement> document.createElementNS("http://www.w3.org/2000/svg", "svg")).createSVGMatrix();
                m.d = 0;
                m.inverse();
            }),
            "SVGMatrix": () => { return (<SVGSVGElement> document.createElementNS("http://www.w3.org/2000/svg", "svg")).createSVGMatrix(); },
            "SVGAnimatedAngle": () => { return Instances.getInstance("SVGMarkerElement").orientAngle; },
            "SVGAnimatedBoolean": () => { return Instances.getInstance("SVGFEConvolveMatrixElement").preserveAlpha; },
            "SVGAnimatedEnumeration": () => { return Instances.getInstance("SVGTextElement").lengthAdjust; },
            "SVGAnimatedInteger": () => { return Instances.getInstance("SVGFilterElement").filterResX; },
            "SVGAnimatedLength": () => { return Instances.getInstance("SVGFEImageElement").x; },
            "SVGAnimatedLengthList": () => { return Instances.getInstance("SVGTextElement").x; },
            "SVGAnimatedNumber": () => { return Instances.getInstance("SVGStopElement").offset; },
            "SVGAnimatedNumberList": () => { return Instances.getInstance("SVGTextElement").rotate; },
            "SVGAnimatedPreserveAspectRatio": () => { return Instances.getInstance("SVGFEImageElement").preserveAspectRatio; },
            "SVGAnimatedRect": () => { return Instances.getInstance("SVGSVGElement").viewBox; },
            "SVGAnimatedString": () => { return Instances.getInstance("SVGFEImageElement").href; },
            "SVGAnimatedTransformList": () => { return Instances.getInstance("SVGImageElement").transform; },
            "SVGPathSegList": () => { return Instances.getInstance("SVGPathElement").pathSegList; },
            "SVGPoint": () => { return Instances.getInstance("SVGSVGElement").currentTranslate; },
            "SVGPointList": () => { return Instances.getInstance("SVGPolygonElement").points; },
            "SVGStringList": () => { return Instances.getInstance("SVGTextElement").systemLanguage; },
            "SVGViewSpec": () => { return Instances.getInstance("SVGSVGElement").currentView; },
            "SVGAngle": () => { return Instances.getInstance("SVGAnimatedAngle").baseVal; },
            "SVGLength": () => { return Instances.getInstance("SVGAnimatedLength").baseVal; },
            "SVGLengthList": () => { return Instances.getInstance("SVGAnimatedLengthList").baseVal; },
            "SVGNumberList": () => { return Instances.getInstance("SVGAnimatedNumberList").baseVal; },
            "SVGPreserveAspectRatio": () => { return Instances.getInstance("SVGAnimatedPreserveAspectRatio").baseVal; },
            "SVGTransformList": () => { return Instances.getInstance("SVGAnimatedTransformList").baseVal; },
        }

        static getAudioContext() {
            if (!Instances.audioContext) {
                Instances.audioContext = new window["AudioContext"]();
            }

            return Instances.audioContext;
        }

        static getCanvas() {
            if (!Instances.canvas) {
                Instances.canvas = document.createElement("canvas");
            }

            return Instances.canvas;
        }

        static catch(throws: () => void): () => Error {
            return () => {
                try {
                    throws();
                } catch (error) {
                    return error;
                }

                console.assert(false, "'throws' function must throw an error.");
            };
        }

        /** Maps type names to information about the HTML or SVG tag they represent. */
        static typesToTags = {
            "HTMLElement": [
                { tagName: "noframes", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "noscript", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "wbr", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "section", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "nav", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "article", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "aside", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "hgroup", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "header", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "footer", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "figure", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "figcaption", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "mark", namespace: "http://www.w3.org/1999/xhtml" },
                // msRelatedElement{name=rp, namespace="http://www.w3.org/1999/xhtml"},
                // msRelatedElement{name=summary, namespace="http://www.w3.org/1999/xhtml"},
            ],
            "HTMLHtmlElement": [
                { tagName: "html", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLHeadElement": [
                { tagName: "head", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLLinkElement": [
                { tagName: "link", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTitleElement": [
                { tagName: "title", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLMetaElement": [
                { tagName: "meta", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLBaseElement": [
                { tagName: "base", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLStyleElement": [
                { tagName: "style", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLBodyElement": [
                { tagName: "body", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLFormElement": [
                { tagName: "form", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLSelectElement": [
                { tagName: "select", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLOptGroupElement": [
                { tagName: "optgroup", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLOptionElement": [
                { tagName: "option", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLInputElement": [
                { tagName: "input", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTextAreaElement": [
                { tagName: "textarea", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLButtonElement": [
                { tagName: "button", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLLabelElement": [
                { tagName: "label", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLFieldSetElement": [
                { tagName: "fieldset", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLLegendElement": [
                { tagName: "legend", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLUListElement": [
                { tagName: "ul", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLOListElement": [
                { tagName: "ol", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLDListElement": [
                { tagName: "dl", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLDirectoryElement": [
                { tagName: "dir", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLMenuElement": [
                { tagName: "menu", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLLIElement": [
                { tagName: "li", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLDivElement": [
                { tagName: "div", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLParagraphElement": [
                { tagName: "p", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLHeadingElement": [
                { tagName: "h1", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "h2", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "h3", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "h4", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "h5", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "h6", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLQuoteElement": [
                { tagName: "q", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLPreElement": [
                { tagName: "pre", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLBRElement": [
                { tagName: "br", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLBaseFontElement": [
                { tagName: "basefont", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLFontElement": [
                { tagName: "font", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLHRElement": [
                { tagName: "hr", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLModElement": [
                { tagName: "ins", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "del", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLAnchorElement": [
                { tagName: "a", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLImageElement": [
                { tagName: "img", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLObjectElement": [
                { tagName: "object", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLParamElement": [
                { tagName: "param", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLAppletElement": [
                { tagName: "applet", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLMapElement": [
                { tagName: "map", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLAreaElement": [
                { tagName: "area", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLSpanElement": [
                { tagName: "span", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLScriptElement": [
                { tagName: "script", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTableElement": [
                { tagName: "table", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTableCaptionElement": [
                { tagName: "caption", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTableColElement": [
                { tagName: "col", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "colgroup", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTableSectionElement": [
                { tagName: "thead", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "tbody", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "tfoot", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTableRowElement": [
                { tagName: "tr", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTableDataCellElement": [
                { tagName: "td", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTableHeaderCellElement": [
                { tagName: "th", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLFrameSetElement": [
                { tagName: "frameset", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLFrameElement": [
                { tagName: "frame", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLIFrameElement": [
                { tagName: "iframe", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLMarqueeElement": [
                { tagName: "marquee", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLEmbedElement": [
                { tagName: "embed", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLIsIndexElement": [
                { tagName: "isindex", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLBlockElement": [
                { tagName: "address", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "blockquote", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "center", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "keygen", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "listing", namespace: "http://www.w3.org/1999/xhtml" },   // obsolete in HTMLmsRelatedElement{name=plaintext, namespace="http://www.w3.org/1999/xhtml" },
                { tagName: "xmp", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLPhraseElement": [
                { tagName: "abbr", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "acronym", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "b", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "bdo", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "big", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "cite", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "code", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "dfn", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "em", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "i", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "kbd", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "nobr", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "rt", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "ruby", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "s", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "samp", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "small", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "strike", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "strong", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "sub", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "sup", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "tt", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "u", namespace: "http://www.w3.org/1999/xhtml" },
                { tagName: "var", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLDDElement": [
                { tagName: "dd", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLDTElement": [
                { tagName: "dt", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLNextIdElement": [
                { tagName: "nextid", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLVideoElement": [
                { tagName: "video", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLAudioElement": [
                { tagName: "audio", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLSourceElement": [
                { tagName: "SOURCE", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLCanvasElement": [
                { tagName: "canvas", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "SVGSVGElement": [
                { tagName: "svg", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGGElement": [
                { tagName: "g", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGDefsElement": [
                { tagName: "defs", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGDescElement": [
                { tagName: "desc", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGTitleElement": [
                { tagName: "title", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGSymbolElement": [
                { tagName: "symbol", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGUseElement": [
                { tagName: "use", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGImageElement": [
                { tagName: "image", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGSwitchElement": [
                { tagName: "switch", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGStyleElement": [
                { tagName: "style", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGPathElement": [
                { tagName: "path", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGRectElement": [
                { tagName: "rect", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGCircleElement": [
                { tagName: "circle", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGEllipseElement": [
                { tagName: "ellipse", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGLineElement": [
                { tagName: "line", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGPolylineElement": [
                { tagName: "polyline", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGPolygonElement": [
                { tagName: "polygon", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGTextElement": [
                { tagName: "text", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGTSpanElement": [
                { tagName: "tspan", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGTextPathElement": [
                { tagName: "textPath", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGMarkerElement": [
                { tagName: "marker", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGLinearGradientElement": [
                { tagName: "linearGradient", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGRadialGradientElement": [
                { tagName: "radialGradient", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGStopElement": [
                { tagName: "stop", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGPatternElement": [
                { tagName: "pattern", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGClipPathElement": [
                { tagName: "clipPath", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGMaskElement": [
                { tagName: "mask", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGAElement": [
                { tagName: "a", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGViewElement": [
                { tagName: "view", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGScriptElement": [
                { tagName: "script", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGMetadataElement": [
                { tagName: "metadata", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGForeignObjectElement": [
                { tagName: "foreignObject", namespace: "http://www.w3.org/2000/svg" },
            ],
            //interface HTCEventElement
            //msRelatedElement{name="PUBLIC:EVENT", namespace="http://www.w3.org/1999/xhtml"},
            //interface HTCPropertyElement
            //msRelatedElement{name="PUBLIC:PROPERTY", namespace="http://www.w3.org/1999/xhtml"},
            //interface HTCComponentElement
            //msRelatedElement{name="PUBLIC:COMPONENT", namespace="http://www.w3.org/1999/xhtml"},
            //interface HTCMethodElement
            //msRelatedElement{name="PUBLIC:METHOD", namespace="http://www.w3.org/1999/xhtml"},
            //interface HTCAttachElement
            //msRelatedElement{name="PUBLIC:ATTACH", namespace="http://www.w3.org/1999/xhtml"},
            //interface HTCElementBehaviorDefaults
            //msRelatedElement{name="PUBLIC:DEFAULTS", namespace="http://www.w3.org/1999/xhtml"}],
            //interface HTAApplicationElement
            //msRelatedElement{name="APPLICATION", namespace="http://www.w3.org/1999/xhtml"}],
            "HTMLProgressElement": [
                { tagName: "progress", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "HTMLTrackElement": [
                { tagName: "track", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            "SVGFilterElement": [
                { tagName: "filter", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEBlendElement": [
                { tagName: "feBlend", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEColorMatrixElement": [
                { tagName: "feColorMatrix", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEComponentTransferElement": [
                { tagName: "feComponentTransfer", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEFuncRElement": [
                { tagName: "feFuncR", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEFuncGElement": [
                { tagName: "feFuncG", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEFuncBElement": [
                { tagName: "feFuncB", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEFuncAElement": [
                { tagName: "feFuncA", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFECompositeElement": [
                { tagName: "feComposite", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEConvolveMatrixElement": [
                { tagName: "feConvolveMatrix", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEDiffuseLightingElement": [
                { tagName: "feDiffuseLighting", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEDistantLightElement": [
                { tagName: "feDistantLight", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEPointLightElement": [
                { tagName: "fePointLight", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFESpotLightElement": [
                { tagName: "feSpotLight", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEDisplacementMapElement": [
                { tagName: "feDisplacementMap", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEFloodElement": [
                { tagName: "feFlood", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEGaussianBlurElement": [
                { tagName: "feGaussianBlur", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEImageElement": [
                { tagName: "feImage", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEMergeElement": [
                { tagName: "feMerge", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEMergeNodeElement": [
                { tagName: "feMergeNode", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEMorphologyElement": [
                { tagName: "feMorphology", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFEOffsetElement": [
                { tagName: "feOffset", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFESpecularLightingElement": [
                { tagName: "feSpecularLighting", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFETileElement": [
                { tagName: "feTile", namespace: "http://www.w3.org/2000/svg" },
            ],
            "SVGFETurbulenceElement": [
                { tagName: "feTurbulence", namespace: "http://www.w3.org/2000/svg" },
            ],
            "FormData": [
                { tagName: "formdata", namespace: "http://www.w3.org/1999/xhtml" },
            ],
            // Chrome-only
            "HTMLContentElement": [
                { tagName: "content", namespace: "http://www.w3.org/1999/xhtml" },
            ],
        };

        public static getInstance(typeName: string) {
            // First, see if it's one of the types we know how to construct.
            var result = this.getInstanceFromFactory(typeName);
            if (result) {
                return Utils.assertType(result, typeName);
            }

            // Next, try document.createEvent.
            try {
                result = document.createEvent(typeName);
                return Utils.assertType(result, typeName);
            }
            catch (ignored) {
            }

            // Finally, try invoking the constructor.
            try {
                result = new window[typeName]();
                return Utils.assertType(result, typeName);
            }
            catch (ignored) {
            }

            return undefined;
        }

        /** Attempts to return an instance of the given 'typeName' via a known factory/helper. */
        private static getInstanceFromFactory(typeName: string) {
            var instance = Instances.getInstanceFromTag(typeName);
            if (instance) {
                return instance;
            }

            var factory = Instances.typesToFactories[typeName];
            if (factory) {
                try {
                    instance = factory();
                } catch (error) {
                    Utils.log("Skipped '" + typeName + "': " + error);
                }
            }

            return instance;
        }

        /** Attempts to return an instance of the given 'typeName' via document.createElementNS. */
        private static getInstanceFromTag(typeName: string) {
            var tags = Instances.typesToTags[typeName];
            if (!tags) {
                return undefined;
            }

            var instance: any;

            for (var i = 0; i < tags.length; i++) {
                var tag = tags[i];
                var candidate = document.createElementNS(tag.namespace, tag.tagName);

                var ctor = candidate.constructor;

                Utils.assertType(candidate, typeName);

                if (Utils.is(candidate, typeName)) {
                    instance = candidate;
                }
            }

            return instance;
        }
    }
}
