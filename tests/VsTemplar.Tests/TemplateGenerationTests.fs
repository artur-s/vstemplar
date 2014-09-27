module VsTemplar.Tests.GenerateRootTemplateTests

open System
open Xunit
open VsTemplar
open TemplateGeneration


[<Fact>]
let ``It should create VsTemplate XML containing correct ProjectTemplateLink elements`` = 

    let metadata = {
        VsProjFileLocation = ""
        Description = "test description"
        Target = ""
        WizardTemplate = None}

    let export = {
        SourceProjectDirectory = ""
        TargetDirectory = ""
        ProjectNameTemplateParameter = ""
        }

    let result = TemplateGeneration.generateRootVsTemplate metadata export []

//    result.

    Assert.False(true, "test not implemented")

