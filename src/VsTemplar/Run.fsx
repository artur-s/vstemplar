module Run

#r "../../packages/FAKE.Core.3.4.3/tools/FakeLib.dll"
#r "./bin/VsTemplar.dll"

open Fake
open System
open System.IO
open VsTemplar

let sourceProjectDir = "C:/Projects/TFS/Git/IQ.Platform.Framework/Src/IQ.Platform.Framework.Common" // source project dir
let templatesDestination = "D:/Temp/ProjectTemplates/Template.zip" // user should provide

let exportedTemplatesTempDir = sprintf "%s%s%A" (Path.GetTempPath()) "VsTemplar_" (Guid.NewGuid())
let targetDir = (exportedTemplatesTempDir @@ (Path.GetFileName(sourceProjectDir)))
Directory.CreateDirectory targetDir
let csProgFileLocation = !! (sourceProjectDir @@ "**.csproj")|> Seq.head


// check
let tempTarget = targetDir @@ "MyTemplate.vstemplate"
tempTarget |> printfn "%s"

VsTemplate.Create (fun p -> {p with VsProjFileLocation = csProgFileLocation
                                    Target = tempTarget})


//VsTemplate.Build ()



/// replaces "PutYourApiNameHere.Xxx" with "$safeprojectname$"
let replaceProjectName progFileLocation =
//        let targetCsProgFile = !! (targetDir @@ "**.csproj")|> Seq.head
        let csProjXmlNamespace = "a","http://schemas.microsoft.com/developer/msbuild/2003"
        let csprojRootNamespaceXpath = "/a:Project/a:PropertyGroup/a:RootNamespace/text()"
        let csprojAssemblyNameXpath = "/a:Project/a:PropertyGroup/a:AssemblyName/text()"
        XmlPokeNS progFileLocation [csProjXmlNamespace] csprojRootNamespaceXpath "$safeprojectname$"
        XmlPokeNS progFileLocation [csProjXmlNamespace] csprojAssemblyNameXpath "$safeprojectname$"

//let copyProjectFiles =
//     FileHelper.DeleteDirs (!! (exportedTemplatesTempDir @@ @"**/bin/" ))
//     FileHelper.DeleteDirs (!! (exportedTemplatesTempDir @@ @"**/obj/" ))


let zipTemplates destination = 
     let tempZip = exportedTemplatesTempDir + "/" + (Path.GetFileName destination)
     printfn "tempZip: %s" tempZip
     let files = !! (exportedTemplatesTempDir @@ "**/*")
     ZipHelper.Zip exportedTemplatesTempDir tempZip files
     printfn "templates.zip destination: %s" destination
     CopyFile destination tempZip

zipTemplates templatesDestination



replaceProjectName csProgFileLocation


