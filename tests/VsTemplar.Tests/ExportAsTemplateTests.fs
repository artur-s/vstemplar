module VsTemplar.Tests.ExportAsTemplateTests

open System
open Xunit
open Xunit.Extensions
open VsTemplar


[<Fact>]
let ``It should create single project template in target directory for given project`` = 

    VsTemplate.ExportAsTemplate (fun p -> 
        {p with 
            SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src"
//          SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src\MyProject.Common\MyProject.Common.fsproj"
            TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\SingleTemplate.zip"})

[<Fact>]
let ``It should create single project template containing project with custom project name parameter`` = 

    VsTemplate.ExportAsTemplate (fun p -> 
        {p with 
            SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src"
            TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\CustomProjectNameTemplate.zip"
            ProjectNameTemplateParameter = "PutYourProjectNameHere"})


//[<Fact(Skip="TODO")>]
//let ``It should create multiple project template in target directory`` = 
//
//    VsTemplate.ExportAsTemplate (fun p -> 
//        {p with 
//            SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src\MyProject.Common"
////          SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src\MyProject.Common\MyProject.Common.fsproj"
//            TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\MultipleTemplate.zip"})

