export declare module Dto {
    class Metadata {
        constructor(url: string, immutable: boolean);
        url: string;
        immutable: boolean;
    }
    class Resource {
        constructor(json: string);
        json: string;
    }
    class File {
        constructor(fullPath: string, relativePath: string, relativePathithoutFilename: string);
        fullPath: string;
        relativePath: string;
        relativePathithoutFilename: string;
    }
}
