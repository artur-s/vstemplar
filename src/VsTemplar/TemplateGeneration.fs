module internal VsTemplar.TemplateGeneration


#if INTERACTIVE
#r "System.Xml"
#r "System.Xml.Linq"
#r "../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#load "XmlHelpers.fs"
#endif


open System
open System.Linq
open System.Xml.Linq
open FSharp.Data
open XmlHelpers


    // TODO: more type-safety with code quotations
    //match <@dest.TemplateContent.Project.TargetFileName@> with 
    //| Quotations.Patterns.FieldGet (_,fi) -> fi.Name
    //| x -> failwith "cannot read that property"

    // ------------------------------------------------------------------------------------------------------
  
    type CsProject = XmlProvider<SampleData.VsProject>
    type Template = XmlProvider<SampleData.VsTemplate>

    let xName name (fromElement:XElement) = xNameNS name (fromElement.GetDefaultNamespace().NamespaceName)

    let projectType parameters =
        match System.IO.Path.GetExtension parameters.VsProjFileLocation with
        | ext when ext.Length > 0 ->
            match ext with
            //TODO: ProjectType enum
            | ".csproj" -> "CSharp"
            | ".fsproj" -> "FSharp"
            | ".vbproj" -> "VisualBasic"
            | _ -> failwith "Not supported project type"
        | _ -> failwith "Cannot determine Visual Studio project type"

    let generateSingleProjectVsTemplate (parameters:MetadataCreationParameters) =

        let sourceProj = CsProject.Load(parameters.VsProjFileLocation)
    
        let projectName = 
            match sourceProj.PropertyGroups |> Seq.tryPick (fun pg -> pg.RootNamespace) with
            | Some pn -> pn
            | _ -> failwithf "Cannot find project name in %s" parameters.VsProjFileLocation

        // http://msdn.microsoft.com/en-us/library/5we0w25d.aspx
        

        let destTemplate = Template.GetSample()
        let dest = Template.GetSample()

        dest.TemplateContent.ProjectCollection.XElement.Remove()
//        dest.WizardData.XElement.Remove()


        let xNameThis name = destTemplate.XElement |> xName name

        dest.TemplateData.XElement 
        |> setXElemValueNS (xNameThis "Name") projectName
        |> setXElemValueNS (xNameThis "DefaultName") projectName
        |> setXElemValueNS (xNameThis "Description") ( match parameters.Description with
                                                        | null | "" -> sprintf "template generated from %s project" projectName
                                                        | d -> d)
        |> setXElemValueNS (xNameThis "ProjectType") (projectType parameters)
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


        //TODO: add proper project to root template and update root wizard extension (provide ready Wizard dll)
        
        
        let setWizardExtension (wizardParams:WizardTemplate option) (wizardElement:Template.WizardExtension) (wizardData:Template.WizardData) =
            match wizardParams with
            | Some wx -> wizardElement.XElement 
                         |> setXElemValueNS (xNameThis "Assembly") (wx.Extension.Assembly.FullName) 
                         |> setXElemValueNS (xNameThis "FullClassName") (wx.Extension.FullClassName) 
                         |> ignore
//                         wizardData
            | _ -> 
                wizardElement.XElement.Remove() |> ignore
                wizardData.XElement.Remove() |> ignore
        
        dest.WizardData |> (dest.WizardExtension |> setWizardExtension (parameters.WizardTemplate))
        
//TODO: dest.WizardData


        dest.XElement

  

    //TODO: adding proper project to root template and update root wizard extension (provide ready Wizard dll)
    let generateRootVsTemplate (parameters:RootTemplate) = 
        
        let prepareTemplate (template:Template.VsTemplate) = 
            template.TemplateContent.Project.XElement.Remove()  //TODO: move to fillTemplateData function
//            template.WizardData.XElement.Remove()
//            template.WizardExtension.XElement.Remove()    
            template    

        let dest = Template.GetSample() |> prepareTemplate
        let xNameThis name = dest.XElement |> xName name 
        
        let fillTemplateData (parameters:RootTemplate) (templateData:Template.TemplateData) =
            
            let projectName = parameters.Name

            templateData.XElement
                |> setXElemValueNS (xNameThis "Name") projectName
                |> setXElemValueNS (xNameThis "Description") ( match parameters.Description with
                                                                | null | "" -> sprintf "Solution Template for %s" projectName
                                                                | d -> d)
                |> setXElemValueNS (xNameThis "ProjectType") ( match parameters.ProjectType with 
                                                               | Some ptype -> sprintf "%A" ptype
                                                               | _ -> "")
                |> setXElemValueNS (xNameThis "Icon") (if parameters.IconPath <> null then parameters.IconPath else "")
                |> setXElemValueNS (xNameThis "RequiredFrameworkVersion") parameters.RequiredFrameworkVersion
    //            |> setXElemValueNS (xNameThis "TemplateGroupID") parameters.IconPath
                |> setXElemValueNS (xNameThis "CreateNewFolder") (parameters.CreateNewFolder.ToString())
                |> ignore
            //TODO: finish

        let fillProjectCollection (ExplicitContent projectsStructure) (templateContent:Template.TemplateContent) =

            let addTemplateLink (item:ProjectTemplateLinkItem) (collectionOfFolder:XElement) =
                
                let link = new Template.ProjectTemplateLink(item.Name, "")
                            |> (fun tl -> tl.XElement.RemoveNodes(); tl)
                link.XElement |> setXThisValue item.Location |> ignore
                addChildXElem link.XElement collectionOfFolder |> ignore
                link
                                
        
            //TODO: recursive, to support folders
            let addCollection structure (xParent:XElement) =
                structure
                |> Seq.iter (function 
                            | ProjectTemplateLink item -> (xParent |> addTemplateLink item |> ignore) 
                            | _ -> ())

            let collection = templateContent.ProjectCollection
            collection.XElement.RemoveNodes()
            addCollection projectsStructure collection.XElement

        let fillWizardExtension (wizardParameters: WizardTemplate option) (wizardExtension:Template.WizardExtension) =
            //TODO: generate WizardExtension
            ()

        dest.TemplateData |> fillTemplateData parameters
        dest.TemplateContent |> fillProjectCollection parameters.Content
        dest.WizardExtension |> fillWizardExtension parameters.Wizard

        dest.XElement
        
