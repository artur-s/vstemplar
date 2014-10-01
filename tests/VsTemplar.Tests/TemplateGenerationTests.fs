module VsTemplar.Tests.GenerateRootTemplateTests

open Xunit
open VsTemplar
open TemplateGeneration

//open FsUnit.Xunit  // https://github.com/fsharp/FsUnit

let shouldequal<'T> (actual:'T) (expected:'T) = Assert.Equal(expected,actual)

let rootParams = {
        Name = "Sample Project"
        Description = "This is a sample project"
        ProjectType = Some ProjectType.FSharp
        RequiredFrameworkVersion = "4.0"
        Wizard = None
        IconPath = ""
        DefaultName = "NewSampleProject"
        CreateNewFolder = false
        Content = SolutionContent []}

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

[<Fact>]
let ``It should create root VsTemplate XML containing correct ProjectTemplateLink elements`` = 
    
    let templateLinks = [ProjectTemplateLink {ProjectTemplateLinkItem.Name = "a name"; Location = "SampleProject.Model\MyTemplate.vstemplate"}]
    let parameters = {rootParams with Content = SolutionContent templateLinks}
    
    // act
    let result = generateRootVsTemplate parameters
    let template = Template.VsTemplate(result)
    
    // assert
    template.TemplateContent.ProjectCollection.ProjectTemplateLinks.Length |> shouldequal templateLinks.Length
//    (template.TemplateContent.ProjectCollection.ProjectTemplateLinks |> Seq.head).ProjectName |> shouldequal (templateLinks|> Seq.head)
