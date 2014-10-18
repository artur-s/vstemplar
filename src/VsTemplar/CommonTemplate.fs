module internal VsTemplar.CommonTemplate

open System.Xml.Linq
open FSharp.Data
open XmlHelpers

type CsProject = XmlProvider<SampleData.VsProject>
type Template = XmlProvider<SampleData.VsTemplate>

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
        
