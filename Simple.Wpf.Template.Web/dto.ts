export module Dto
{
    export class Resource {
        constructor(url: string, immutable: boolean) {
            this.url = url;
            this.immutable = immutable;
        }

        url: string;

        immutable: boolean;
    }
}