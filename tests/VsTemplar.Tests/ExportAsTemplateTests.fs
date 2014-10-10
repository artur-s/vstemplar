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

module ``When root template items should be inferred`` =

    [<Theory;AutoFoqData>]
    let ``It should create multiple project template in target directory with root template containing inferred solution items`` 
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
                TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\MultipleTemplateInferred.zip"
                Root = Some root})

module ``When root template items should are provided`` =
    open System.Reflection

    [<Theory;AutoFoqData>]
    let ``It should create multiple project template in target directory with root template containing provided solution items`` 
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
                Content = ExplicitContent [
                            ProjectTemplateLink {ProjectTemplateLinkItem.Name = "$safeprojectname$.Model"; Location = "PutYourApiNameHere.Model\MyTemplate.vstemplate"} 
                            ProjectTemplateLink {ProjectTemplateLinkItem.Name = "$safeprojectname$.WebApi"; Location = "PutYourApiNameHere.WebApi\MyTemplate.vstemplate"}]
                Wizard = Some { 
                            Extension = { Assembly = AssemblyName "MyNamespace.Template.Wizard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c9a76f51a8a9555f"
                                          FullClassName = "MyNamespace.Template.Wizard.RootWizard"}
                            Data = ""}}

        VsTemplate.ExportAsTemplate (fun p -> 
            {p with 
                SourceProjectDirectory = @"C:\Projects\Git\temp\MyProject\Src"
                TargetDirectory = @"C:\Projects\Git\temp\MyProject\Template\MultipleTemplateProvidedProjectItems.zip"
                Root = Some root})
