"use strict";
var MirrorJS;
(function (MirrorJS) {
    var Html = (function () {
        function Html() {
        }
        Html.addDiv = function (parent) {
            var div = document.createElement("div");
            parent.appendChild(div);
            return div;
        };
        Html.addBr = function (parent) {
            return parent.appendChild(document.createElement("br"));
        };
        /** Helper to append a span with the given 'textContent' to the specified 'element'. */
        Html.addSpan = function (parent, textContent) {
            var span = document.createElement("span");
            span.textContent = textContent;
            parent.appendChild(span);
            return span;
        };
        /** Helper to append an H3 with the given 'textContent' to the specified 'element'. */
        Html.addH1 = function (parent, textContent) {
            var element = document.createElement("h1");
            element.textContent = textContent;
            parent.appendChild(element);
            return element;
        };
        /** Helper to append an H3 with the given 'textContent' to the specified 'element'. */
        Html.addH3 = function (parent, textContent) {
            var element = document.createElement("h3");
            element.textContent = textContent;
            parent.appendChild(element);
            return element;
        };
        Html.linkify = function (inputText) {
            return inputText.replace(Html.linkifyRegex, '<a href="$1">$1</a>');
        };
        Html.addTable = function (parent) {
            var table = document.createElement("table");
            parent.appendChild(table);
            return table;
        };
        Html.addTableRow = function (parent) {
            var row = document.createElement("tr");
            parent.appendChild(row);
            return row;
        };
        Html.addTableCell = function (parent, textContent) {
            var row = document.createElement("td");
            row.textContent = textContent;
            parent.appendChild(row);
            return row;
        };
        Html.makeTable = function (cellText) {
            var table = document.createElement("table");
            var index = -1;
            while (true) {
                var cells = cellText(++index);
                if (cells === undefined) {
                    break;
                }
                var row = Html.addTableRow(table);
                cells.forEach(function (text, index, array) {
                    Html.addTableCell(row, text);
                });
            }
            return table;
        };
        Html.tabilify = function (data) {
            var keys = Object.keys(data);
            return Html.makeTable(function (index) {
                if (index >= keys.length) {
                    return undefined;
                }
                var key = keys[index];
                var value = data[key];
                return [key, value];
            });
        };
        Html.linkifyRegex = /(\bhttps?:\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;
        return Html;
    }());
    MirrorJS.Html = Html;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=Html.js.map
