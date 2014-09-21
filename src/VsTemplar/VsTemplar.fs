// http://trelford.com/blog/post/F-XML-Comparison-(XElement-vs-XmlDocument-vs-XmlReaderXmlWriter-vs-Discriminated-Unions).aspx
namespace VsTemplar

#if INTERACTIVE
#r "System.Xml.Linq"
#r "../../packages/FSharp.Data.2.0.4/lib/net40/FSharp.Data.dll"
#endif

module VsTemplate =
    open System
    open System.IO
    open System.Linq
    open System.Xml.Linq
    open FSharp.Data
    open Fake

    // ------------------------------------------------------------------------------------------------------
    let ofOption = function | null -> None | i -> Some i
    let xName name = XName.op_Implicit name 
    let xNameNS name ns = XName.Get(name, ns)

    let getXElemNS childName (elem:XElement) = 
        ofOption <| elem.Element(childName)
    let getXElem childName (elem:XElement) = 
        getXElemNS (xName childName) elem
    
    let setXElemValueNS childName newValue (elem:XElement) = 
        match getXElemNS childName elem with
        | Some child -> child.Value <- newValue
        | None -> ()
        elem
    let setXElemValue childName newValue (elem:XElement) = 
        setXElemValueNS (xName childName) newValue elem

    let addChildXElem (child:XElement) (elem:XElement) =
        elem.Add(child)
        elem

    let getXAttr attrName (elem:XElement) = 
        ofOption <| elem.Attribute(xName attrName)

    let setXAttrValue attrName newValue (elem:XElement) =
        match getXAttr attrName elem with
        | Some attr -> attr.Value <- newValue
        | None -> ()
        elem

    // TODO: more type-safety with code quotations
    //match <@dest.TemplateContent.Project.TargetFileName@> with 
    //| Quotations.Patterns.FieldGet (_,fi) -> fi.Name
    //| x -> failwith "cannot read that property"

    // ------------------------------------------------------------------------------------------------------
  
    type WizardTemplate =
        {
            Assembly : System.Reflection.AssemblyName
            FullClassName : string
        }

    type MetadataCreationParameters =
        {   VsProjFileLocation : string
            Description : string
            Target: string
            WizardTemplate : WizardTemplate option} // seq 
    
    type CsProject = XmlProvider<SampleData.VsProject>
    type Template = XmlProvider<SampleData.VsTemplate>
    

    let generateVSTemplate (parameters:MetadataCreationParameters) =


        let sourceProj = CsProject.Load(parameters.VsProjFileLocation)
    
        let projectName = 
            match sourceProj.PropertyGroups |> Seq.tryPick (fun pg -> pg.RootNamespace) with
            | Some pn -> pn
            | _ -> failwithf "Cannot find project name in %s" parameters.VsProjFileLocation

        // http://msdn.microsoft.com/en-us/library/5we0w25d.aspx
        let projectType =
            match System.IO.Path.GetExtension parameters.VsProjFileLocation with
            | ext when ext.Length > 0 ->
                match ext with
                | ".csproj" -> "CSharp"
                | ".fsproj" -> "FSharp"
                | ".vbproj" -> "VisualBasic"
                | _ -> failwith "Not supported project type"
            | _ -> failwith "Cannot determine Visual Studio project type"

        let destTemplate = Template.GetSample()
        let dest = Template.GetSample()
        // dest.XElement.ToString()
        // destTemplate.XElement.ToString()

        let xNameThis name = xNameNS name (destTemplate.XElement.GetDefaultNamespace().NamespaceName) // Xmlns

        dest.TemplateData.XElement 
        |> setXElemValueNS (xNameThis "Name") projectName
        |> setXElemValueNS (xNameThis "DefaultName") projectName
        |> setXElemValueNS (xNameThis "Description") ( match parameters.Description with
                                                        | null | "" -> sprintf "template generated from %s project" projectName
                                                        | d -> d)
        |> setXElemValueNS (xNameThis "ProjectType") projectType
        |> ignore

        // <TemplateContent> element
        // <Project> element

        let project = dest.TemplateContent.Project;
        project.XElement.RemoveNodes()
    //    xProject.ToString()|> ignore

        let projFileName = (sprintf "%s.csproj" projectName)
        project.XElement
        |> setXAttrValue "TargetFileName" projFileName
        |> setXAttrValue "File" projFileName
        |> ignore

        let addProjectFile targetFileName (projectOrFolderElement:XElement) =
            let item = destTemplate.TemplateContent.Project.ProjectItems |> Seq.head |> (fun i -> new XElement(i.XElement))
                        |> setXAttrValue "TargetFileName" targetFileName
            //printfn "file added %s" targetFileName
            item.Value <- targetFileName
            addChildXElem item projectOrFolderElement

        let addProjectFolder targetFolderName (projectOrFolderElement:XElement) =
            let folder = destTemplate.TemplateContent.Project.Folders |> Seq.head |> (fun i -> new XElement(i.XElement))
                        |> setXAttrValue "Name" targetFolderName
                        |> setXAttrValue "TargetFolderName" targetFolderName
            folder.RemoveNodes()
            addChildXElem folder projectOrFolderElement |> ignore
//            printfn "folder added %s" targetFolderName
            folder

    //    addProjectFile "packages.config" project |> ignore
    //    addProjectFile "GenericTransportMapper.cs" project |> ignore
    //    addProjectFolder "Security" project |> ignore
   
        let processCsProjectItems (sourceProj:CsProject.Project) vsproject =
        
            let getCompiles (sourceProj:CsProject.Project) = 
                match sourceProj.ItemGroups |> Seq.tryFind (fun ig -> ig.Compiles.Any()) with
                | Some ig -> ig.Compiles |> Array.map (fun c -> c.Include) 
                | _ -> [||]

            let getContents (sourceProj:CsProject.Project) = 
                match sourceProj.ItemGroups |> Seq.tryFind (fun ig -> ig.Contents.Any()) with
                | Some ig -> ig.Contents |> Array.map (fun c -> c.Include) 
                | _ -> [||]
        
    //        TODO: generate as folders (currently as item)
            let getOtherFolders (sourceProj:CsProject.Project) = 
                match sourceProj.ItemGroups |> Seq.tryFind (fun ig -> ig.Folders.Any()) with
                | Some ig -> ig.Folders |> Array.map (fun c -> c.Include) 
                | _ -> [||]

            let getNones (sourceProj:CsProject.Project) = 
                match sourceProj.ItemGroups |> Seq.tryFind (fun ig -> ig.Nones.Any()) with
                | Some ig -> ig.Nones |> Array.map (fun c -> c.Include) 
                | _ -> [||]

            let getEmbeddedResources (sourceProj:CsProject.Project) = 
                match sourceProj.ItemGroups |> Seq.tryFind (fun ig -> ig.EmbeddedResources.Any()) with
                | Some ig -> ig.EmbeddedResources |> Array.map (fun c -> c.Include) 
                | _ -> [||]
        
            let addProjectContent projectContent =
                let rec addContent contentItem (xParent:XElement) =
                    match contentItem with
                    | ProjectContent.FolderContent (dir, subs) -> 
                        let newParent = addProjectFolder dir xParent
                        for s in subs do
                            addContent s newParent
                    | ProjectContent.FileContent file -> 
                        addProjectFile file xParent |> ignore
                addContent projectContent project.XElement

            let allItems sourceProj = 
                seq{
//                    printfn "Compiles:" 
                    yield! (getCompiles sourceProj)
//                    printfn "Contents:" 
                    yield! (getContents sourceProj)
//                    printfn "Nones:" 
                    yield! (getNones sourceProj)
//                    printfn "EmbeddedResources:" 
                    yield! (getEmbeddedResources sourceProj)
    //                yield! (getOtherFolders sourceProj)
                    }
                |> Array.ofSeq

            let projContent = allItems sourceProj
                                |> List.ofArray
                                |> ProjectContent.createProjectContent
                                |> ProjectContent.mergeContentItems

            projContent |> Seq.iter addProjectContent
            projContent
        
           
        let items = processCsProjectItems sourceProj project
        
        let setWizardExtension wizardParams (wizardElement:XElement) =
            match wizardParams with
            | Some wx -> wizardElement 
                         |> setXElemValueNS (xNameThis "Assembly") (wx.Assembly.FullName) 
                         |> setXElemValueNS (xNameThis "FullClassName") (wx.FullClassName) 
                         |> ignore
            | _ -> wizardElement.Remove()
        
        dest.WizardExtension.XElement |> setWizardExtension (parameters.WizardTemplate)

        dest.XElement

    
    let CreateMetadataVsTemplateMetadata
        (setParams:MetadataCreationParameters -> MetadataCreationParameters) =
        
        let defaults = { 
            VsProjFileLocation = null
            Description = null
            Target = null
            WizardTemplate = None} // []

        let parameters = setParams defaults
        let template = generateVSTemplate parameters
        template.Save(parameters.Target)


    type TemplateExportParameters = 
        {   SourceProjectDirectory : string
            TargetDirectory : string
            /// a template parameter for project name in VS project file. Default is '$safeprojectname$'
            ProjectNameTemplateParameter:string }

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


    let ExportAsTemplate
        (setParams:TemplateExportParameters -> TemplateExportParameters) =

        let defaults = {
            SourceProjectDirectory = null
            TargetDirectory = null
            ProjectNameTemplateParameter = "$safeprojectname$"}

        let validateParameters ps =
            if String.IsNullOrWhiteSpace (ps.SourceProjectDirectory) then
                invalidArg "SourceProjecDirectory" "Source project location cannot be empty"
            ps

        let parameters = defaults |> setParams |> validateParameters

        let sourceProjectsDirs = 
            // subdirectories containing VS project file: *.csproj, *.fsproj, *.vbproj
            !! (parameters.SourceProjectDirectory @@ @"**/*.?sproj")
        
        // TODO: support multible. Single project for now
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
            let tempTarget = targetDir @@ "MyTemplate.vstemplate"
            tempTarget |> printfn "%s"

            CreateMetadataVsTemplateMetadata (fun p -> {p with 
                                                            VsProjFileLocation = projFileLocation
                                                            Target = tempTarget})

            if parameters.ProjectNameTemplateParameter <> null
                then replaceProjectName parameters.ProjectNameTemplateParameter targetProgFileLocation


        let templateSource = 
            match !! (exportedTemplatesTempDir @@ "*/") |> List.ofSeq with
            | singleProject::[] -> singleProject
            | _ -> exportedTemplatesTempDir

        // ensure destination folder
        dirPath templatesDestination |> Directory.CreateDirectory |> ignore
        
        zipTemplateTo templateSource templatesDestination

    //TODO: clear after copying
