export module Dto
{
    export class Resource {
        constructor(url: string) {
            this.url = url;
        }

        url: string;
    }
}