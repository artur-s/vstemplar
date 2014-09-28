module VsTemplar.Tests.GenerateRootTemplateTests

open Xunit
open VsTemplar
open TemplateGeneration
//open FsUnit.Xunit  // https://github.com/fsharp/FsUnit

let shouldequal<'T> (actual:'T) (expected:'T) = Assert.Equal(expected,actual)

[<Fact>]
let ``It should create VsTemplate XML containing provided template data`` = 
    
    let rootParams = {
        Name = "Sample Project"
        Description = "This is a sample project"
        ProjectType = Some ProjectType.FSharp
        RequiredFrameworkVersion = "4.0"
        Wizard = None
        IconPath = ""
        DefaultName = "NewSampleProject"
        CreateNewFolder = false}

    let result = generateRootVsTemplate rootParams []
    let template = Template.VsTemplate(result)
    
    template.TemplateData.Name |> shouldequal rootParams.Name
    template.TemplateData.Description |> shouldequal rootParams.Description
    template.TemplateData.ProjectType |> shouldequal (rootParams.ProjectType.Value.ToString())
    template.TemplateData.RequiredFrameworkVersion |> shouldequal rootParams.RequiredFrameworkVersion


//let ``It should create VsTemplate XML containing correct ProjectTemplateLink elements`` = 
//    Assert.False true //TODO: