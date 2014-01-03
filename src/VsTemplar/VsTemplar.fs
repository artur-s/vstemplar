// http://trelford.com/blog/post/F-XML-Comparison-(XElement-vs-XmlDocument-vs-XmlReaderXmlWriter-vs-Discriminated-Unions).aspx
namespace VsTemplar

module VsTemplate =

//#r "System.Xml.Linq"
//#r "../packages/FSharp.Data.1.1.10/lib/net40/FSharp.Data.dll"

    open System
    open FSharp.Data
    open System.Linq
    open System.Xml.Linq


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

    type CsProject = XmlProvider<"./SampleData/Sample.csproj.xml">
    type Template = XmlProvider<"./SampleData/Sample.vstemplate.xml">


    let generateVSTemplate (csProgFileLocation:string) =


        let sourceProj = CsProject.Load(csProgFileLocation)
    
        let projectName = 
            match sourceProj.GetPropertyGroups() |> Seq.tryPick (fun pg -> pg.RootNamespace) with
            | Some pn -> pn
            | _ -> failwithf "Cannot find project name in %s" csProgFileLocation


        let destTemplate = Template.GetSample()
        let dest = Template.GetSample()
        // dest.XElement.ToString()
        // destTemplate.XElement.ToString()

        let xNameThis name = xNameNS name destTemplate.Xmlns

        dest.TemplateData.XElement 
        |> setXElemValueNS (xNameThis "Name") projectName
        |> setXElemValueNS (xNameThis "DefaultName") projectName
        |> setXElemValueNS (xNameThis "Description") (sprintf "this is template under construction for %s" projectName)
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
            let item = destTemplate.TemplateContent.Project.GetProjectItems() |> Seq.head |> (fun i -> new XElement(i.XElement))
                        |> setXAttrValue "TargetFileName" targetFileName
            //printfn "file added %s" targetFileName
            item.Value <- targetFileName
            addChildXElem item projectOrFolderElement

        let addProjectFolder targetFolderName (projectOrFolderElement:XElement) =
            let folder = destTemplate.TemplateContent.Project.GetFolders() |> Seq.head |> (fun i -> new XElement(i.XElement))
                        |> setXAttrValue "Name" targetFolderName
                        |> setXAttrValue "TargetFolderName" targetFolderName
            folder.RemoveNodes()
            addChildXElem folder projectOrFolderElement |> ignore
//            printfn "folder added %s" targetFolderName
            folder

    //    addProjectFile "packages.config" project |> ignore
    //    addProjectFile "GenericTransportMapper.cs" project |> ignore
    //    addProjectFolder "Security" project |> ignore
   
        let processCsProjectItems (sourceProj:CsProject.DomainTypes.Project) vsproject =

        
            let getCompiles (sourceProj:CsProject.DomainTypes.Project) = 
                match sourceProj.GetItemGroups() |> Seq.tryFind (fun ig -> ig.GetCompiles().Any()) with
                | Some ig -> ig.GetCompiles() |> Array.map (fun c -> c.Include) 
                | _ -> [||]

            let getContents (sourceProj:CsProject.DomainTypes.Project) = 
                match sourceProj.GetItemGroups() |> Seq.tryFind (fun ig -> ig.GetContents().Any()) with
                | Some ig -> ig.GetContents() |> Array.map (fun c -> c.Include) 
                | _ -> [||]
        
    //        TODO: generate as folders (currently as item)
            let getOtherFolders (sourceProj:CsProject.DomainTypes.Project) = 
                match sourceProj.GetItemGroups() |> Seq.tryFind (fun ig -> ig.GetFolders().Any()) with
                | Some ig -> ig.GetFolders() |> Array.map (fun c -> c.Include) 
                | _ -> [||]

            let getNones (sourceProj:CsProject.DomainTypes.Project) = 
                match sourceProj.GetItemGroups() |> Seq.tryFind (fun ig -> ig.GetNones().Any()) with
                | Some ig -> ig.GetNones() |> Array.map (fun c -> c.Include) 
                | _ -> [||]

            let getEmbeddedResources (sourceProj:CsProject.DomainTypes.Project) = 
                match sourceProj.GetItemGroups() |> Seq.tryFind (fun ig -> ig.GetEmbeddedResources().Any()) with
                | Some ig -> ig.GetEmbeddedResources() |> Array.map (fun c -> c.Include) 
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
        dest.XElement


    let create csProgFileLocation (target:string) =
        let template = generateVSTemplate csProgFileLocation
        template.Save(target)

