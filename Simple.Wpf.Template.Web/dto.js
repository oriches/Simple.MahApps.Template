"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Dto;
(function (Dto) {
    var Metadata = /** @class */ (function () {
        function Metadata(url, immutable) {
            this.url = url;
            this.immutable = immutable;
        }
        return Metadata;
    }());
    Dto.Metadata = Metadata;
    var Resource = /** @class */ (function () {
        function Resource(json) {
            this.json = json;
        }
        return Resource;
    }());
    Dto.Resource = Resource;
    var File = /** @class */ (function () {
        function File(fullPath, relativePath, relativePathithoutFilename) {
            this.fullPath = fullPath;
            this.relativePath = relativePath;
            this.relativePathithoutFilename = relativePathithoutFilename;
        }
        return File;
    }());
    Dto.File = File;
})(Dto = exports.Dto || (exports.Dto = {}));
//# sourceMappingURL=dto.js.map