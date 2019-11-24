"use strict";
/// <reference path="dto.ts" />
Object.defineProperty(exports, "__esModule", { value: true });
var express = require("express");
var path = require("path");
var fs = require("fs");
var os = require("os");
var mkdirp = require("mkdirp");
var rmdir = require("rimraf");
var colors = require("colors");
var dto = require("./dto");
var port = process.env.port || 1337;
var app = express();
var rootPath = path.resolve(".").toLowerCase() + "\\" + "resources";
var rootUrl = "";
var filename = "data.json";
app.all("*", audit);
app.get("/heartbeat", getHeartbeat);
app.get("/metadata", getMetadata);
app.get("*", getResource);
app.post("*", postResource);
app.put("*", putResource);
app.delete("*", deleteResource);
app.use(failed);
var server = app.listen(port, os.hostname(), function () {
    var host = server.address().address;
    var port = server.address().port;
    rootUrl = "http://" + host + ":" + port;
    console.log("\"Example app listening at " + rootUrl);
    console.log("Root path = " + rootPath);
});
function audit(req, res, next) {
    var date = new Date();
    var dateTimeString = date.toDateString() + " " + date.toTimeString();
    console.log();
    console.log(dateTimeString + " - " + req.method + " -> " + req.url.green);
    next();
}
function failed(err, req, res, next) {
    console.log(err.stack);
    res.status(500).send("Oops, something went wrong!");
    next();
}
function writeResourceNotFound(res) {
    res.writeHead(404, "Resource not found!");
    return res;
}
function writeSuccessfulHeader(res) {
    res.writeHead(200, { "Content-Type": "application/json" });
    return res;
}
function writeSuccessfulEmptyHeader(res) {
    res.writeHead(200);
    return res;
}
function getHeartbeat(req, res) {
    var displayDate = new Date().toUTCString();
    var json = "{ \"timeStamp\" : \"" + displayDate + "\"}";
    writeSuccessfulHeader(res)
        .end(json);
}
function getMetadata(req, res) {
    var metadata = [];
    metadata.push(new dto.Dto.Metadata(rootUrl + req.url, true));
    metadata.push(new dto.Dto.Metadata(rootUrl + "/heartbeat", true));
    var files = [];
    getFiles(rootPath, files);
    for (var file in files) {
        console.log("file: " + files[file].fullPath);
        var relativePath = files[file].relativePathithoutFilename
            .split("\\")
            .join("/");
        metadata.push(new dto.Dto.Metadata(rootUrl + relativePath, false));
    }
    writeSuccessfulHeader(res)
        .end(JSON.stringify(metadata));
}
function getResource(req, res) {
    if (!doesFileExist(req)) {
        writeResourceNotFound(res)
            .end();
    }
    else {
        var fullPath = buildFullPath(req);
        fs.readFile(fullPath, function (err, data) {
            writeSuccessfulHeader(res)
                .end(data);
        });
    }
}
function postResource(req, res) {
    saveResource(req, res);
}
function putResource(req, res) {
    saveResource(req, res);
}
function deleteResource(req, res) {
    var folder = buildFolder(req);
    if (!doesFileExist(req)) {
        writeResourceNotFound(res)
            .end();
    }
    else {
        rmdir(folder, function () {
            writeSuccessfulEmptyHeader(res)
                .end();
        });
    }
}
function saveResource(req, res) {
    var folder = buildFolder(req);
    var fullPath = buildFullPath(req);
    req.on("readable", function () {
        var data = read(req);
        var json = extractJsonFromResource(data);
        mkdirp(folder, function (err) {
            if (processAnyError(err, res)) {
                return;
            }
            fs.writeFile(fullPath, json, function () {
                writeSuccessfulHeader(res)
                    .end(data);
            });
        });
    });
}
function doesFileExist(req) {
    try {
        var fullPath = buildFullPath(req);
        var stats = fs.lstatSync(fullPath);
        if (!stats.isFile()) {
            return false;
        }
        return true;
    }
    catch (err) {
        return false;
    }
}
function buildFullPath(req) {
    return buildFolder(req) + "\\data.json";
}
function buildFolder(req) {
    return (rootPath + req.url)
        .split("/")
        .join("\\");
}
function read(req) {
    var data = "";
    var chunk;
    while (null !== (chunk = req.read())) {
        data += chunk;
    }
    return data;
}
function extractJsonFromResource(json) {
    var resource = JSON.parse(json);
    return resource.json;
}
function processAnyError(err, res) {
    if (err) {
        console.log("Failed - " + err);
        res.writeHead(500, "Something has borked!");
        res.end();
        return true;
    }
    return false;
}
function getFiles(dir, files) {
    files = files || [];
    var localFiles = fs.readdirSync(dir);
    for (var i in localFiles) {
        var name = dir + "\\" + localFiles[i];
        if (fs.statSync(name).isDirectory()) {
            getFiles(name, files);
        }
        else {
            var fullPath = name;
            var relativePath = name.split(rootPath)[1];
            var relativePathWithNoFilename = relativePath.split("\\" + filename)[0];
            files.push(new dto.Dto.File(fullPath, relativePath, relativePathWithNoFilename));
        }
    }
    return files;
}
//# sourceMappingURL=server.js.map