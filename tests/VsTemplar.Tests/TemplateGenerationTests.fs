module VsTemplar.Tests.GenerateRootTemplateTests

open Xunit
open VsTemplar
open CommonTemplate
open MultiProjectTemplate
open Xunit.Extensions
open TestHelpers
//open FsUnit
//open FsUnit.Xunit  // https://github.com/fsharp/FsUnit

let shouldequal<'T> (expected:'T) (actual:'T) = Assert.Equal(expected,actual)

let rootParams = {
        Name = "Sample Project"
        Description = "This is a sample project"
        ProjectType = Some ProjectType.FSharp
        ProjectSubType = ""
        TemplateGroupID = ""
        RequiredFrameworkVersion = "4.0"
        Wizard = None
        IconPath = ""
        DefaultName = "NewSampleProject"
        CreateNewFolder = false
        Content = ExplicitContent []}

[<Fact>]
let ``It should create root VsTemplate XML containing provided template data`` = 

    // act
    let result = generateRootVsTemplate rootParams
    let template = Template.VsTemplate(result)
    
    // assert
    template.TemplateData.Name |> shouldequal rootParams.Name
    template.TemplateData.Description |> shouldequal rootParams.Description
    template.TemplateData.ProjectType |> shouldequal (rootParams.ProjectType.Value.ToString())
    template.TemplateData.RequiredFrameworkVersion |> shouldequal rootParams.RequiredFrameworkVersion


[<Theory;AutoFoqData>]
let ``It should create root VsTemplate XML containing correct ProjectTemplateLink elements`` 
     (templateLinks: ProjectTemplateLinkItem seq) 
    = 
    
    let solutionItems:SolutionItem seq = templateLinks |> Seq.map (ProjectTemplateLink)
    let parameters = {rootParams with Content = ExplicitContent solutionItems}
    
    // act
    let result = generateRootVsTemplate parameters
    let template = Template.VsTemplate(result)
    
    
    // assert
    template.Type |> shouldequal (TemplateType.ProjectGroup.ToString())
    let actualLinks = template.TemplateContent.ProjectCollection.ProjectTemplateLinks
    actualLinks.Length |> shouldequal (templateLinks |> Seq.length)

    [(fun (actual:Template.ProjectTemplateLink) input -> actual.ProjectName = input.Name)
     (fun (actual:Template.ProjectTemplateLink) input -> actual.Value = input.Location)]
    |> pairwiseAllEqual actualLinks templateLinks 
    |> shouldequal true

// ref: http://nikosbaxevanis.com/blog/2013/10/19/auto-mocking-with-foq-and-autofixture/
//      http://blog.ploeh.dk/2010/10/08/AutoDataTheorieswithAutoFixture/