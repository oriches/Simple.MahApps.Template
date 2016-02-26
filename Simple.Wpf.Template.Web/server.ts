/// <reference path="dto.ts" />

import express = require("express");
import http = require("http");
import path = require("path");
import fs = require("fs");
import mkdirp = require("mkdirp");

var colors = require("colors");
import dto = require("./dto");

var port = process.env.port || 1337;
var app = express();

var rootPath = path.resolve(".").toLowerCase() + "\\" + "resources";
var rootUrl = "";
var filename = "data.json";

app.all("*", audit);
app.get("/heartbeat", getHeartbeat);
app.get("/metadata", getMetadata);
app.post("*", postResource);
app.put("*", putResource);
app.use(failed);

var server = app.listen(port, "localhost", () => {

    var host = server.address().address;
    var port = server.address().port;
    rootUrl = `http://${host}:${port}`;

    console.log(`"Example app listening at ${rootUrl}`);
    console.log(`Root path = ${rootPath}`);
});

function audit(req: http.ServerRequest, res: http.ServerResponse, next: Function) {

    var date = new Date();
    var dateTimeString = date.toDateString() + " " + date.toTimeString();

    console.log();

    console.log(dateTimeString + " - " + req.method + " -> " + req.url.green);

    next();
}

function failed(err: any, req: any, res: any, next: Function) {

    console.log(err.stack);
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

function getMetadata(req: http.ServerRequest, res: http.ServerResponse) {

    var metadata: dto.Dto.Metadata[] = [];

    metadata.push(new dto.Dto.Metadata(rootUrl + req.url, true));
    metadata.push(new dto.Dto.Metadata(rootUrl + "/heartbeat", true));

    var files: dto.Dto.File[] = [];
    getFiles(rootPath, files);

    for (var file in files) {
        console.log(`file: ${files[file].fullPath}`);

        var relativePath = files[file].relativePathithoutFilename
            .split("\\")
            .join("/");

        metadata.push(new dto.Dto.Metadata(rootUrl + relativePath, false));
    }

    writeSuccessfulHeader(res)
        .end(JSON.stringify(metadata));
}

function postResource(req: http.ServerRequest, res: http.ServerResponse) {

    saveResource(req, res);
}

function putResource(req: http.ServerRequest, res: http.ServerResponse) {

    saveResource(req, res);
}

function saveResource(req: http.ServerRequest, res: http.ServerResponse) {

    var folder = (rootPath + req.url)
        .split("/")
        .join("\\");

    var fullPath = folder + "\\data.json";

    req.on("readable", () => {
        var data = read(req);
        var json = extractJsonFromResource(data);

        mkdirp(folder, err => {
            if (processAnyError(err, res)) {
                return;
            }

            fs.writeFile(fullPath, json, () => {
                writeSuccessfulHeader(res)
                    .end(data);
            });
        });
    });
}

function read(req: http.ServerRequest): string {
    var data = "";
    var chunk: void | Object;
    while (null !== (chunk = req.read())) {
        data += chunk;
    }

    return data;
}

function extractJsonFromResource(json: string): string {
    var resource: dto.Dto.Resource = JSON.parse(json);
    return resource.json;
}

function processAnyError(err: any, res: http.ServerResponse): Boolean {
    if (err) {
        console.log(`Failed - ${err}`);
        res.writeHead(500, "Something has borked!");
        res.end();

        return true;
    }

    return false;
}

function getFiles(dir, files: dto.Dto.File[]) {

    files = files || [];

    var localFiles = fs.readdirSync(dir);

    for (var i in localFiles) {
        var name = dir + "\\" + localFiles[i];
        if (fs.statSync(name).isDirectory()) {
            getFiles(name, files);
        } else {

            var fullPath = name;
            var relativePath = name.split(rootPath)[1];
            var relativePathWithNoFilename = relativePath.split(`\\${filename}`)[0];

            files.push(new dto.Dto.File(fullPath, relativePath, relativePathWithNoFilename));
        }
    }
    return files;
}