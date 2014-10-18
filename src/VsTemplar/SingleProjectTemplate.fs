module internal VsTemplar.SingleProjectTemplate

open System.Linq
open System.Xml.Linq
open XmlHelpers
open VsTemplar.CommonTemplate


let generateSingleProjectVsTemplate (parameters:MetadataCreationParameters) =

    let sourceProj = CsProject.Load(parameters.VsProjFileLocation)

    let projectName = 
        match sourceProj.PropertyGroups |> Seq.tryPick (fun pg -> pg.RootNamespace) with
        | Some pn -> pn
        | _ -> failwithf "Cannot find project name in %s" parameters.VsProjFileLocation

    // http://msdn.microsoft.com/en-us/library/5we0w25d.aspx
    

    let destTemplate = Template.GetSample()
    let dest = Template.GetSample()
    let xNameThis name = destTemplate.XElement |> xNameDefNs name
    let projectType = projectType parameters

    dest.TemplateContent.ProjectCollection.XElement.Remove()

    dest.TemplateData.XElement 
    |> setXElemValueNS (xNameThis "Name") projectName
    |> setXElemValueNS (xNameThis "DefaultName") projectName
    |> setXElemValueNS (xNameThis "Description") ( match parameters.Description with
                                                    | null | "" -> sprintf "template generated from %s project" projectName
                                                    | d -> d)
    |> setXElemValueNS (xNameThis "ProjectType") (projectType.ToString())
    |> ignore


    let project = dest.TemplateContent.Project;
    project.XElement.RemoveNodes()

    let projFileName = projectFileName parameters //(sprintf "%s.%s" projectName (projectExtension projectType))
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
        printfn "folder added %s" targetFolderName
        folder

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
                printfn "Compiles:" 
                yield! (getCompiles sourceProj)
                printfn "Contents:" 
                yield! (getContents sourceProj)
                printfn "Nones:" 
                yield! (getNones sourceProj)
                printfn "EmbeddedResources:" 
                yield! (getEmbeddedResources sourceProj)
//              yield! (getOtherFolders sourceProj)
                }
            |> Array.ofSeq

        let projContent = allItems sourceProj
                            |> List.ofArray
                            |> ProjectContent.createProjectContent
                            |> ProjectContent.mergeContentItems

        projContent |> Seq.iter addProjectContent
        projContent
    
       
    let items = processCsProjectItems sourceProj project

    let xNameThis name = destTemplate.XElement |> xNameDefNs name
    (dest.WizardExtension, dest.WizardData) |> setWizardExtension (parameters.WizardTemplate) xNameThis
    
    dest.XElement

