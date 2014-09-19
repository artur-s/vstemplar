module  VsTemplar.Tests.ExportAsTemplateTests

open System
open Xunit
open Xunit.Extensions
open VsTemplar

[<Fact>]
let ``It should create zipped template in target directory`` = 

    VsTemplate.ExportAsTemplate (fun p -> {p with 
                                            SourceProjectDirectory = "C:/Projects/TFS/Git/IQ.Platform.Framework/Src/IQ.Platform.Framework.Common"
                                            TargetDirectory = "D:/Temp/ProjectTemplates/Template.zip"})
