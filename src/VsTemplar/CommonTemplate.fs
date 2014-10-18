module internal VsTemplar.CommonTemplate

open System.IO
open System.Xml.Linq
open FSharp.Data
open XmlHelpers

type CsProject = XmlProvider<SampleData.VsProject>
type Template = XmlProvider<SampleData.VsTemplate>

let projectFileName parameters =
    Path.GetFileName parameters.VsProjFileLocation

let projectType parameters =
    match Path.GetExtension parameters.VsProjFileLocation with
    | ext when ext.Length > 0 ->
        match ext with
        //TODO: ProjectType enum
        | ".csproj" -> ProjectType.CSharp
        | ".fsproj" -> ProjectType.FSharp
        | ".vbproj" -> ProjectType.VisualBasic
        | _ -> failwith "Not supported project type"
    | _ -> failwith "Cannot determine Visual Studio project type"

let projectExtension = function
    | ProjectType.CSharp -> ".csproj"
    | ProjectType.FSharp -> ".fsproj"
    | ProjectType.VisualBasic -> ".vbproj"
    | _ -> failwith "Not supported project extension"

let setWizardExtension 
    (wizardParams:WizardTemplate option) 
    (xName: string -> XName) 
    (wizardElement:Template.WizardExtension, wizardData:Template.WizardData) 
    =
    match wizardParams with
    | Some wx -> wizardElement.XElement 
                    |> setXElemValueNS (xName "Assembly") (wx.Extension.Assembly.FullName) 
                    |> setXElemValueNS (xName "FullClassName") (wx.Extension.FullClassName) |> ignore
                 wizardData.XElement |> setXThisValue wx.Data
                    |> ignore
    | _ -> 
        wizardElement.XElement.Remove() |> ignore
        wizardData.XElement.Remove() |> ignore
        
