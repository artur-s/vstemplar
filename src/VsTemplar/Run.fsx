module Run

#r "./bin/VsTemplar.dll"

open System
open VsTemplar

// check
//Environment.CurrentDirectory <- ""
let csProgFileLocation = ".\YourProjectHere.csproj"
let target = @"D:\Temp\MyTemplate.vstemplate"
VsTemplate.create csProgFileLocation target


