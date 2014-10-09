module VsTemplar.Tests.ExportAsTemplateTests

open System
open Xunit
open VsTemplar
open TestHelpers
open Xunit.Extensions


[<Fact>]
let ``It should create single project template in target directory for given project`` = 

    VsTemplate.ExportAsTemplate (fun p -> 
        {p with 
            SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src\MyProject.Common"
//          SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src\MyProject.Common\MyProject.Common.fsproj"
            TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\SingleTemplate.zip"})


[<Fact>]
let ``It should create single project template containing project with custom project name parameter`` = 

    VsTemplate.ExportAsTemplate (fun p -> 
        {p with 
            SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src"
            TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\CustomProjectNameTemplate.zip"
            ProjectNameTemplateParameter = "PutYourProjectNameHere"})


[<Theory;AutoFoqData>]
let ``It should create multiple project template in target directory`` 
    (name:string)
    (description:string)
    (defaultName:string)
    = 

    let root = 
        {Name = name
         Description = description
         IconPath = null
         ProjectType = Some ProjectType.CSharp
         RequiredFrameworkVersion = "4.0"
         DefaultName = defaultName
         CreateNewFolder = false
         Content = InferredContent
         Wizard = None}

    VsTemplate.ExportAsTemplate (fun p -> 
        {p with 
            SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src"
            TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\MultipleTemplate.zip"
            Root = Some root})

