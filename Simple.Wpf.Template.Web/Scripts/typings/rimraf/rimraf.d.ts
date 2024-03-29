// Type definitions for rimraf
// Project: https://github.com/isaacs/rimraf
// Definitions by: Carlos Ballesteros Velasco <https://github.com/soywiz>
// Definitions: https://github.com/borisyankov/DefinitelyTyped

// Imported from: https://github.com/soywiz/typescript-node-definitions/rimraf.d.ts

declare module "rimraf" {
    function rimraf(path: string, callback: (error: Error) => void): void;

    module rimraf {
        export function sync(path: string): void;

        export var EMFILE_MAX: number;
        export var BUSYTRIES_MAX: number;
    }

    export = rimraf;
}