module  VsTemplar.Tests.ExportAsTemplateTests

open System
open Xunit
open Xunit.Extensions
open VsTemplar

[<Fact>]
let ``It should create zipped template in target directory`` = 

    VsTemplate.ExportAsTemplate (fun p -> {p with 
                                            SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src"
//                                            SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src\MyProject.Common\MyProject.Common.fsproj"
                                            TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\Template.zip"})
