module internal VsTemplar.MultiProjectTemplate


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
open VsTemplar.CommonTemplate


//more type-safety with code quotations
//match <@dest.TemplateContent.Project.TargetFileName@> with 
//| Quotations.Patterns.FieldGet (_,fi) -> fi.Name
//| x -> failwith "cannot read that property"



//TODO: adding proper project to root template and update root wizard extension (provide ready Wizard dll)
let generateRootVsTemplate (parameters:RootTemplate) = 
        
    let prepareTemplate (template:Template.VsTemplate) = 
        template.TemplateContent.Project.XElement.Remove()  //TODO: move to fillTemplateData function
        template    

    let dest = Template.GetSample() |> prepareTemplate
    let xNameThis name = dest.XElement |> xNameDefNs name
    
    let fillTemplateData (parameters:RootTemplate) (templateData:Template.TemplateData) =
        
        let stringOrEmpty (s:string) = if s <> null then s else ""

        let projectName = parameters.Name

        templateData.XElement
            |> setXElemValueNS (xNameThis "Name") projectName
            |> setXElemValueNS (xNameThis "Description") ( match parameters.Description with
                                                            | null | "" -> sprintf "Solution Template for %s" projectName
                                                            | d -> d)
            |> setXElemValueNS (xNameThis "ProjectType") ( match parameters.ProjectType with 
                                                           | Some ptype -> sprintf "%A" ptype
                                                           | _ -> "")
            |> setXElemValueNS (xNameThis "TemplateGroupID") (parameters.TemplateGroupID |> stringOrEmpty) |> removeXNodeIfEmpty
            |> setXElemValueNS (xNameThis "ProjectSubType") (parameters.ProjectSubType |> stringOrEmpty) |> removeXNodeIfEmpty
            |> setXElemValueNS (xNameThis "DefaultName") (parameters.DefaultName |> stringOrEmpty)
            |> setXElemValueNS (xNameThis "Icon") (parameters.IconPath |> stringOrEmpty) |> removeXNodeIfEmpty
            |> setXElemValueNS (xNameThis "RequiredFrameworkVersion") parameters.RequiredFrameworkVersion
            |> setXElemValueNS (xNameThis "CreateNewFolder") (parameters.CreateNewFolder.ToString())
            |> ignore

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

    dest.XElement |> setXAttrValue "Type" (TemplateType.ProjectGroup.ToString()) |> ignore
    dest.TemplateData |> fillTemplateData parameters
    dest.TemplateContent |> fillProjectCollection parameters.Content
    (dest.WizardExtension, dest.WizardData) |> setWizardExtension parameters.Wizard xNameThis

    dest.XElement
        
