// http://trelford.com/blog/post/F-XML-Comparison-(XElement-vs-XmlDocument-vs-XmlReaderXmlWriter-vs-Discriminated-Unions).aspx
namespace VsTemplar

module VsTemplate =
    open System
    open System.IO
    open Fake
    open TemplateGeneration

    let CreateMetadataVsTemplateMetadata
        (setParams:MetadataCreationParameters -> MetadataCreationParameters) =
        
        let defaults = { 
            VsProjFileLocation = null
            Description = null
            Target = null
            WizardTemplate = None} // []

        let parameters = setParams defaults
        let template = generateSingleProjectVsTemplate parameters
        template.Save(parameters.Target)


    open VsTemplar

    let ExportAsTemplate
        (setParams:TemplateExportParameters -> TemplateExportParameters) =

        /// in a project file replaces project name with a template parameter
        let replaceProjectName parameter targetProgFile =
            let csProjXmlNamespace = ["a","http://schemas.microsoft.com/developer/msbuild/2003"]
            let csprojRootNamespaceXpath = "/a:Project/a:PropertyGroup/a:RootNamespace/text()"
            let csprojAssemblyNameXpath = "/a:Project/a:PropertyGroup/a:AssemblyName/text()"
            for xpath in [csprojRootNamespaceXpath;csprojAssemblyNameXpath] do
                XmlPokeNS targetProgFile csProjXmlNamespace xpath parameter

        let zipTemplateTo source destination = 
    
            FileHelper.DeleteDirs (!! (source @@ @"**/bin/" ))
            FileHelper.DeleteDirs (!! (source @@ @"**/obj/" ))

            let tempZip = source + "/" + (Path.GetFileName destination)
            printfn "tempZip: %s" tempZip
            let files = !! (source @@ "**/*")
            try
                ZipHelper.Zip source tempZip files
            with
                | ex -> printf "exception when zipping files: %s" ex.Message

            printfn "templates.zip destination: %s" destination
            CopyFile destination tempZip

        let defaults = {
            SourceProjectDirectory = null
            TargetDirectory = null
            ProjectNameTemplateParameter = "$safeprojectname$"
            Root = None}

        let validateParameters ps =
            if String.IsNullOrWhiteSpace (ps.SourceProjectDirectory) then
                invalidArg "SourceProjecDirectory" "Source project location cannot be empty"
            ps

        let parameters = defaults |> setParams |> validateParameters

        let sourceProjectsDirs = 
            // subdirectories containing VS project file: *.csproj, *.fsproj, *.vbproj
            !! (parameters.SourceProjectDirectory @@ @"**/*.?sproj")
        
        let progFileLocations = 
            match sourceProjectsDirs |> List.ofSeq with
            | [] -> invalidArg "setParams.SourceProjecDirectory" "Source project location does not contain any VS projects"
            | locations -> locations

        let dirPath filePath = Path.GetDirectoryName filePath
        let fileName filePath = Path.GetFileName filePath
        let extension file = Path.GetExtension file

        let templatesDestination = 
            match parameters.TargetDirectory with
            | null -> parameters.SourceProjectDirectory
            | tg when (extension tg).Length > 0 -> tg
            //TODO: for single project templates, default name of zip file should be the same as project name        
            | tg -> tg @@ "Template.zip"

        let exportedTemplatesTempDir = sprintf "%s%s%A" (Path.GetTempPath()) "VsTemplar_" (Guid.NewGuid())
        let templateFileName = "MyTemplate.vstemplate"
        
        for projFileLocation in progFileLocations do

            let sourceProjectDir = dirPath projFileLocation

            // TODO: temp directory in current location
            let targetDir = (exportedTemplatesTempDir @@ (fileName sourceProjectDir))
            Directory.CreateDirectory targetDir |> ignore
            let targetProgFileName = fileName projFileLocation
            let targetProgFileLocation = targetDir @@ targetProgFileName
        
      
            let copyProjectFiles sourceDir targetDir =
                Fake.FileHelper.CopyDir targetDir sourceDir allFiles |> ignore

            copyProjectFiles sourceProjectDir targetDir

            // check
            let tempTarget = targetDir @@ templateFileName
            tempTarget |> printfn "%s"

            CreateMetadataVsTemplateMetadata (fun p -> {p with 
                                                            VsProjFileLocation = projFileLocation
                                                            Target = tempTarget})

            if parameters.ProjectNameTemplateParameter <> null
                then replaceProjectName parameters.ProjectNameTemplateParameter targetProgFileLocation

       
        let templateSources = !! (exportedTemplatesTempDir @@ "*/") |> List.ofSeq
        
        // creating root template
        if templateSources.Length > 1 then 
            match parameters.Root with
            | Some root ->
                let projectsRelativeLocations = 
                    progFileLocations |> List.map (fun pl -> 
                        let relativePath = (dirPath >> fileName) pl
                        (relativePath, relativePath @@ templateFileName))

                let content = seq { for (name,location) in projectsRelativeLocations
                                    -> ProjectTemplateLink { Name = name; Location = location}}
                                    |> SolutionContent
                let root = generateRootVsTemplate {root with RootTemplate.Content = content } 
                root.Save(exportedTemplatesTempDir @@ "RootTemplate.vstemplate")
            | _ -> ()

        let templateSourceToZip = 
            match templateSources with
            | singleProject::[] -> singleProject
            | _ -> exportedTemplatesTempDir

        // ensure destination folder
        dirPath templatesDestination |> Directory.CreateDirectory |> ignore
        
        zipTemplateTo templateSourceToZip templatesDestination

    //TODO: clear after copying
