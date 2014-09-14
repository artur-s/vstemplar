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
let targetDir = (exportedTemplatesTempDir @@ (Path.GetFileName(sourceProjectDir))) // TODO: temp directory in current location
Directory.CreateDirectory targetDir
let csProgFileLocation = !! (sourceProjectDir @@ "**.csproj")|> Seq.head
let targetProgFileLocation = !! (targetDir @@ "**.csproj")|> Seq.head

let getDirPath filePath =
    Path.GetDirectoryName filePath

/// replaces "PutYourApiNameHere.Xxx" with "$safeprojectname$"
let replaceProjectName targetProgFile =
        let csProjXmlNamespace = "a","http://schemas.microsoft.com/developer/msbuild/2003"
        let csprojRootNamespaceXpath = "/a:Project/a:PropertyGroup/a:RootNamespace/text()"
        let csprojAssemblyNameXpath = "/a:Project/a:PropertyGroup/a:AssemblyName/text()"
        XmlPokeNS targetProgFile [csProjXmlNamespace] csprojRootNamespaceXpath "$safeprojectname$"
        XmlPokeNS targetProgFile [csProjXmlNamespace] csprojAssemblyNameXpath "$safeprojectname$"

let copyProjectFiles sourceDir targetDir =
    Fake.FileHelper.CopyDir targetDir sourceDir allFiles |> ignore

let zipTemplateTo source destination = 
    
    FileHelper.DeleteDirs (!! (source @@ @"**/bin/" ))
    FileHelper.DeleteDirs (!! (source @@ @"**/obj/" ))

    let tempZip = source + "/" + (Path.GetFileName destination)
    printfn "tempZip: %s" tempZip
    let files = !! (source @@ "**/*")
    ZipHelper.Zip source tempZip files
    printfn "templates.zip destination: %s" destination
    CopyFile destination tempZip


copyProjectFiles sourceProjectDir targetDir

// check
let tempTarget = targetDir @@ "MyTemplate.vstemplate"
tempTarget |> printfn "%s"

VsTemplate.CreateMetadata (fun p -> {p with VsProjFileLocation = csProgFileLocation
                                            Target = tempTarget})

replaceProjectName targetProgFileLocation

getDirPath templatesDestination |> Directory.CreateDirectory
zipTemplateTo exportedTemplatesTempDir templatesDestination

// almost ready
VsTemplate.ExportAsTemplate (fun p -> {p with 
                                        SourceProjectDirectory = "C:/Projects/TFS/Git/IQ.Platform.Framework/Src/IQ.Platform.Framework.Common"
                                        TargetDirectory = "D:/Temp/ProjectTemplates/Template.zip"})


