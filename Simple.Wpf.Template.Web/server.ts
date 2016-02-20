/// <reference path="dto.ts" />

import express = require("express");
import http = require("http");
import path = require("path");
import fs = require("fs");

var colors = require("colors");
import dto = require("./dto");

import Request = express.Request;
import Response = express.Response;

var port = process.env.port || 1337;
var app = express();

var rootPath = path.resolve(".").toLowerCase();
var rootUrl = "";

app.all("*", audit);
app.get("/heartbeat", getHeartbeat);
app.get("/resources", getResources);
app.use(failed);

var server = app.listen(port, "localhost", () => {
    var host = server.address().address;
    var port = server.address().port;
    rootUrl = `http://${host}:${port}`;

    console.log(`"Example app listening at ${rootUrl}`);
    console.log(`Root path = ${rootPath}`);
});

function audit(req: Request, res: Response, next: Function) {

    console.log();
    console.log(req.method + " -> " + req.originalUrl.green);

    next();
}

function failed(err: any, req: any, res: any, next: any) {

    console.error(err.stack);
    res.status(500).send("Oops, something went wrong!");

    next();
}

function writeSuccessfulHeader(res: http.ServerResponse): http.ServerResponse {

    res.writeHead(200, { "Content-Type": "application/json" });
    return res;
}

function getHeartbeat(req: http.ServerRequest, res: http.ServerResponse) {

    var displayDate = new Date().toUTCString();
    var json = `{ \"timeStamp\" : \"${displayDate}\"}`;

    writeSuccessfulHeader(res)
        .end(json);
}

function getResources(req: http.ServerRequest, res: http.ServerResponse) {

    var resources: dto.Dto.Resource[] = [];

    resources.push(new dto.Dto.Resource(rootUrl + req.url));
    resources.push(new dto.Dto.Resource(rootUrl + "/heartbeat"));

    writeSuccessfulHeader(res)
        .end(JSON.stringify(resources));
}