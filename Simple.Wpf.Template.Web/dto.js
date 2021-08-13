"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Dto = void 0;
var Dto;
(function (Dto) {
    class Metadata {
        constructor(url, immutable) {
            this.url = url;
            this.immutable = immutable;
        }
    }
    Dto.Metadata = Metadata;
    class Resource {
        constructor(json) {
            this.json = json;
        }
    }
    Dto.Resource = Resource;
    class File {
        constructor(fullPath, relativePath, relativePathithoutFilename) {
            this.fullPath = fullPath;
            this.relativePath = relativePath;
            this.relativePathithoutFilename = relativePathithoutFilename;
        }
    }
    Dto.File = File;
})(Dto = exports.Dto || (exports.Dto = {}));
//# sourceMappingURL=dto.js.map