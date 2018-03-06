"use strict";
var MirrorJS;
(function (MirrorJS) {
    /** Iterates over the union of two sorted string arrays, optionally applying a filter. */
    var MergeIterator = (function () {
        function MergeIterator(sources, filter) {
            this.sources = sources;
            this.filter = filter;
            /** The total number of items in all sources.  Duplicate items all contribute to the total.
                Used by getProgress() to estimate progress. */
            this.totalItems = 0;
            this.positions = [];
            for (var i = 0; i < this.sources.length; i++) {
                var source = sources[i];
                this.positions.push(0);
                this.totalItems += source.length;
            }
        }
        /** Returns the estimated progress in the iteration as a fraction in the range 0.0 to 1.0 */
        MergeIterator.prototype.getProgress = function () {
            // sum all of the current positions
            var progress = this.positions.reduce(function (previousSum, currentPosition, sourceIndex) {
                return previousSum + currentPosition;
            }, 0);
            // and divide by the total number of items
            return progress / this.totalItems;
        };
        MergeIterator.prototype.getNextFrom = function (sourceIndex) {
            var source = this.sources[sourceIndex];
            var position = this.positions[sourceIndex];
            var item = source[position];
            // The inverted logic below passes the assert when 'position' is the last array item
            // (i.e., when source[position + 1] returns undefined.)
            console.assert(!(item >= source[position + 1]), "Items in source must be sorted in ascending order.");
            return item;
        };
        MergeIterator.prototype.chooseNext = function () {
            var currentIndex = 0;
            var current = this.getNextFrom(currentIndex);
            for (var candidateIndex = 1; candidateIndex < this.sources.length; candidateIndex++) {
                var candidate = this.getNextFrom(candidateIndex);
                // Advance the current choice if the candidate is earlier in the iterator, or if
                // the current choice is undefined (i.e., we have not yet encountered a source with
                // any remaining items.)
                if ((current === undefined) || (current > candidate)) {
                    current = candidate;
                    currentIndex = candidateIndex;
                }
            }
            return currentIndex;
        };
        /** Return the next item in the iteration, ignoring the current filter. */
        MergeIterator.prototype.unfilteredNext = function () {
            var _this = this;
            var sourceIndex = this.chooseNext();
            var nextItem = this.getNextFrom(sourceIndex);
            if (nextItem === undefined) {
                this.positions.forEach(function (position, index, array) {
                    console.assert(position === _this.sources[index].length, "Must only encounter 'undefined' when all sources have been exhausted.");
                });
                return undefined;
            }
            var sourcesMask = (1 << sourceIndex);
            this.positions[sourceIndex]++;
            for (var candidateIndex = sourceIndex + 1; candidateIndex < this.sources.length; candidateIndex++) {
                var candidate = this.getNextFrom(candidateIndex);
                if (candidate === nextItem) {
                    sourcesMask |= (1 << candidateIndex);
                    this.positions[candidateIndex]++;
                }
            }
            return { value: nextItem, sourcesMask: sourcesMask };
        };
        MergeIterator.prototype.next = function () {
            var candidate = this.unfilteredNext();
            // If a filter was specified, fast-forward past any items that fail the filter.
            if (this.filter) {
                for (; candidate && !this.filter(candidate.value, candidate.sourcesMask); candidate = this.unfilteredNext()) { }
            }
            return candidate;
        };
        /** True if the iteration contains 0 items. */
        MergeIterator.prototype.isEmpty = function () {
            // Temporarily advance the iterator to the next item.
            var nextItem = this.next();
            // Trivial case: the is no next item.
            if (nextItem === undefined) {
                return true;
            }
            // Rewind the iterator to just before the current item.
            for (var sourceIndex = 0; sourceIndex < this.positions.length; sourceIndex++) {
                if (nextItem.sourcesMask & (1 << sourceIndex)) {
                    this.positions[sourceIndex]--;
                }
            }
            return false;
        };
        /** Invokes the given callback for each item in the iteration.  'sources' is a bitwise mask
            indicating which sources contained the current value. (e.g., sources = 3 means sources 0 and 1
            contained the item, but not 2, 3, etc.) */
        MergeIterator.prototype.forEach = function (callback) {
            for (var next = this.next(); next && callback(next.value, next.sourcesMask); next = this.next()) { }
        };
        MergeIterator.prototype.toArray = function () {
            var array = [];
            this.forEach(function (value, sourcesMask) {
                array.push({ value: value, sourcesMask: sourcesMask });
                return true;
            });
        };
        return MergeIterator;
    }());
    MirrorJS.MergeIterator = MergeIterator;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=MergeIterator.js.map
