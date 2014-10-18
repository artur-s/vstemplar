module Run

#r "../../packages/FAKE.Core/tools/FakeLib.dll"
#r "./bin/VsTemplar.dll"

open Fake
open System
open System.IO
open VsTemplar


let sourceProjectDir = "C:/Projects/TFS/Git/MyProject/Src" // source project dir
let templatesDestination = "D:/Temp/ProjectTemplates/Template.zip" // user should provide


VsTemplate.CreateMetadataVsTemplateMetadata (fun p -> {p with 
                                                        VsProjFileLocation = "C:/Projects/Git/MyProject/Src/MyProject.Common/MyProject.Common.csproj"
                                                        Target = "template.vstemplate"})

// almost ready
VsTemplate.ExportAsTemplate (fun p -> {p with 
                                        SourceProjectDirectory = "C:/Projects/Git/MyProject/Src/MyProject.Common"
                                        TargetDirectory = "D:/Temp/ProjectTemplates/Template.zip"})


