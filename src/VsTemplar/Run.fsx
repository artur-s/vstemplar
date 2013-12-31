module Run

#r "./bin/ProjToTemplate.dll"

open System
open Templarius

// check
//Environment.CurrentDirectory <- "D:/Projects/TFS/w3/Platform/Dev/IQ.Platform.WebApi.Template"
let csProgFileLocation = "D:/Projects/TFS/w3/Platform/Dev/IQ.Platform.WebApi.Template/TemplatesBackup/source_PutYourApiNameHere/src/PutYourApiNameHere.WebApi/PutYourApiNameHere.WebApi.csproj"
let target = @"D:\Temp\MyTemplate.vstemplate"
Templarius.createVSTemplate csProgFileLocation target


