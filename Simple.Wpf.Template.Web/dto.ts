export module Dto {
    export class Metadata {
        constructor(url: string, immutable: boolean) {
            this.url = url;
            this.immutable = immutable;
        }

        url: string;

        immutable: boolean;
    }

    export class Resource {
        constructor(json: string) {
            this.json = json;
        }

        json: string;
    }

    export class File {
        constructor(fullPath: string, relativePath: string, relativePathithoutFilename: string) {
            this.fullPath = fullPath;
            this.relativePath = relativePath;
            this.relativePathithoutFilename = relativePathithoutFilename;
        }

        fullPath: string;

        relativePath: string;

        relativePathithoutFilename: string;
    }
}