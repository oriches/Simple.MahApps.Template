"use strict";
/// <reference path="dto.ts" />
Object.defineProperty(exports, "__esModule", { value: true });
const express = require("express");
const path = require("path");
const fs = require("fs");
const os = require("os");
const mkdirp = require("mkdirp");
const rmdir = require("rimraf");
const dto = require("./dto");
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
var server = app.listen(port, os.hostname(), () => {
    var host = server.address().address;
    var port = server.address().port;
    rootUrl = `http://${host}:${port}`;
    console.log(`"Example app listening at ${rootUrl}`);
    console.log(`Root path = ${rootPath}`);
});
function audit(req, res, next) {
    const date = new Date();
    const dateTimeString = date.toDateString() + " " + date.toTimeString();
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
    const displayDate = new Date().toUTCString();
    const json = `{ \"timeStamp\" : \"${displayDate}\"}`;
    writeSuccessfulHeader(res)
        .end(json);
}
function getMetadata(req, res) {
    const metadata = [];
    metadata.push(new dto.Dto.Metadata(rootUrl + req.url, true));
    metadata.push(new dto.Dto.Metadata(rootUrl + "/heartbeat", true));
    const files = [];
    getFiles(rootPath, files);
    for (let file in files) {
        console.log(`file: ${files[file].fullPath}`);
        const relativePath = files[file].relativePathithoutFilename
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
        const fullPath = buildFullPath(req);
        fs.readFile(fullPath, (err, data) => {
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
    const folder = buildFolder(req);
    if (!doesFileExist(req)) {
        writeResourceNotFound(res)
            .end();
    }
    else {
        rmdir(folder, () => {
            writeSuccessfulEmptyHeader(res)
                .end();
        });
    }
}
function saveResource(req, res) {
    var folder = buildFolder(req);
    var fullPath = buildFullPath(req);
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
function doesFileExist(req) {
    try {
        const fullPath = buildFullPath(req);
        const stats = fs.lstatSync(fullPath);
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
    let data = "";
    let chunk;
    while (null !== (chunk = req.read())) {
        data += chunk;
    }
    return data;
}
function extractJsonFromResource(json) {
    const resource = JSON.parse(json);
    return resource.json;
}
function processAnyError(err, res) {
    if (err) {
        console.log(`Failed - ${err}`);
        res.writeHead(500, "Something has borked!");
        res.end();
        return true;
    }
    return false;
}
function getFiles(dir, files) {
    files = files || [];
    const localFiles = fs.readdirSync(dir);
    for (let i in localFiles) {
        const name = dir + "\\" + localFiles[i];
        if (fs.statSync(name).isDirectory()) {
            getFiles(name, files);
        }
        else {
            const fullPath = name;
            const relativePath = name.split(rootPath)[1];
            const relativePathWithNoFilename = relativePath.split(`\\${filename}`)[0];
            files.push(new dto.Dto.File(fullPath, relativePath, relativePathWithNoFilename));
        }
    }
    return files;
}
//# sourceMappingURL=server.js.map