"use strict";
var MirrorJS;
(function (MirrorJS) {
    var Comment = (function () {
        function Comment(output, getComment) {
            this.output = output;
            this.getComment = getComment;
            this.update();
        }
        Comment.prototype.update = function () {
            var comment = this.getComment();
            if (comment === undefined) {
                return;
            }
            comment = MirrorJS.Html.linkify(comment);
            MirrorJS.Html.addSpan(this.output, "").innerHTML = comment;
        };
        return Comment;
    })();
    MirrorJS.Comment = Comment;
})(MirrorJS || (MirrorJS = {}));
//# sourceMappingURL=Comment.js.map