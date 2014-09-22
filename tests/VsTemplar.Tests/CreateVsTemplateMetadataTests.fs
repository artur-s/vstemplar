module  VsTemplar.Tests.CreateVsTemplateMetadataTests

open System
open System.Reflection
open Xunit
open Xunit.Extensions
open VsTemplar

[<Fact>]
let ``It should create VsTemplate metadata file based on provided VS project path`` = 

    VsTemplate.CreateMetadataVsTemplateMetadata(fun p -> 
        {p with 
            VsProjFileLocation = @"C:\Projects\Git\temp\MyProject\Src\MyProject.Common\MyProject.Common.csproj"
            //TODO: if only directory path is provided, create file with project name, e.g. MyProject.Common.vstemplate
            // Target = @"C:\Projects\Git\temp\MyProject\Template" 
            Target = @"C:\Projects\Git\temp\MyProject\Template\MyTemplateNoWizard.vstemplate"
            })

[<Fact>]
let ``It should create VsTemplate metadata file based on provided VS project path and WizardTemplate type`` = 

    VsTemplate.CreateMetadataVsTemplateMetadata(fun p -> 
        {p with 
            VsProjFileLocation = @"C:\Projects\Git\temp\MyProject\Src\MyProject.Common\MyProject.Common.csproj"
            Target = @"C:\Projects\Git\temp\MyProject\Template\MyTemplateWithWizard.vstemplate"
            WizardTemplate = Some { Assembly = AssemblyName "MyProject.Template.Wizard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c9a76f51a8a9555f"
                                    FullClassName = "MyProject.Template.Wizard.ChildWizard"}
            })
