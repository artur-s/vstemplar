// --------------------------------------------------------------------------------------
// FAKE build script 
// --------------------------------------------------------------------------------------

#r @"packages/FAKE.Core/tools/FakeLib.dll"
open Fake 
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open System

// --------------------------------------------------------------------------------------
// START TODO: Provide project-specific details below
// --------------------------------------------------------------------------------------

// Information about the project are used
//  - for version and project name in generated AssemblyInfo file
//  - by the generated NuGet package 
//  - to run tests and to publish documentation on GitHub gh-pages
//  - for documentation, you also need to edit info in "docs/tools/generate.fsx"

// The name of the project 
// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
let project = "VsTemplar"//"FSharp.ProjectTemplate"

// Short summary of the project
// (used as description in AssemblyInfo and as a short summary for NuGet package)
let summary = "Generates *.vstemplate file based on VS project file: *.csproj, *.fsproj, etc."

// Longer description of the project
// (used as a description for NuGet package; line breaks are automatically cleaned up)
let description = """
  A tool that generates *.vstemplate file based on 
  provided Visual Studio project file (*.csproj, *.fsproj, etc.)"""

// List of author names (for NuGet package)
let authors = [ "Artur S" ]
// Tags for your project (for NuGet package)
let tags = "vstemplate template fsharp "

// File system information 
// (<solutionFile>.sln is built during the building process)
let solutionFile  = "VsTemplar"
// Pattern specifying assemblies to be tested using NUnit
let testAssemblies = ["tests/*/bin/*/VsTemplar*Tests*.dll"]

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted 
let gitHome = "https://artur_s@bitbucket.org/artur_s"
// The name of the project on GitHub
let gitName = "vstemplar"

// --------------------------------------------------------------------------------------
// END TODO: The rest of the file includes standard build steps 
// --------------------------------------------------------------------------------------
let buildDir = "./bin/"
let buildMergedDir = buildDir @@ "merged"
let nugetDir = "./nuget/"
let packagesDir = "./packages/"

// Read additional information from the release notes document
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let release = parseReleaseNotes (IO.File.ReadAllLines "RELEASE_NOTES.md")

// Generate assembly info files with the right version & up-to-date information
Target "AssemblyInfo" (fun _ ->
  let fileName = "src/" + project + "/AssemblyInfo.fs"
  CreateFSharpAssemblyInfo fileName
      [ Attribute.Title project
        Attribute.Product project
        Attribute.Description summary
        Attribute.Version release.AssemblyVersion
        Attribute.FileVersion release.AssemblyVersion
        Attribute.InternalsVisibleTo "VsTemplar.Tests" ] 
)

// --------------------------------------------------------------------------------------
// Clean build results & restore NuGet packages

Target "RestorePackages" RestorePackages

Target "Clean" (fun _ ->
    CleanDirs ["bin"; "temp"]
)

Target "CleanDocs" (fun _ ->
        CleanDirs ["docs/output"]
)

// --------------------------------------------------------------------------------------
// Build library & test project

//Target "Build" (fun _ ->
//    !! solutionFile
//    |> MSBuildRelease "" "Rebuild"
//    |> ignore
//)
Target "Build" (fun _ ->
    { BaseDirectory = __SOURCE_DIRECTORY__
      Includes = [ solutionFile +       ".sln"
//                   solutionFile + ".Tests.sln" 
                    ]
      Excludes = [] } 
    |> MSBuildRelease "bin" "Rebuild"
    |> ignore
)

// --------------------------------------------------------------------------------------
// Run the unit tests using test runner & kill test runner when complete

Target "RunTests" (fun _ ->
    ActivateFinalTarget "CloseTestRunner"

    { BaseDirectory = __SOURCE_DIRECTORY__
      Includes = testAssemblies
      Excludes = [] } 
    |> NUnit (fun p ->
        { p with
            DisableShadowCopy = true
            TimeOut = TimeSpan.FromMinutes 20.
            OutputFile = "TestResults.xml" })
)

FinalTarget "CloseTestRunner" (fun _ ->  
    ProcessHelper.killProcess "nunit-agent.exe"
)

// --------------------------------------------------------------------------------------
// Build a NuGet package
//
Target "MergeAssemblies" (fun _ ->
    CreateDir buildMergedDir

    let toPack =
        ["VsTemplar.dll"; "FSharp.Core.dll"; "FakeLib.dll"; "FSharp.Data.dll"; (*"FSharp.Data.TypeProviders.dll"; *) 
         "ICSharpCode.SharpZipLib.dll"; "Zlib.Portable.dll"]
        |> List.map (fun l -> buildDir @@ l)
        |> separated " "

    let result =
        ExecProcess (fun info ->
            info.FileName <- currentDirectory @@ "tools" @@ "ILRepack" @@ "ILRepack.exe"
            info.Arguments <- sprintf "/internalize /verbose /lib:%s /ver:%s /out:%s %s" buildDir release.AssemblyVersion (buildMergedDir @@ "VsTemplar.dll") toPack
            ) (TimeSpan.FromMinutes 5.)

    if result <> 0 then failwithf "Error during ILRepack execution."
)

Target "NuGet" (fun _ ->
    // Format the description to fit on a single line (remove \r\n and double-spaces)
    let description = description.Replace("\r", "")
                                 .Replace("\n", "")
                                 .Replace("  ", " ")


//    let nugetToolsDir = nugetDir @@ "tools"
//    !! (buildDir @@ "**/*.dll") |> Copy nugetToolsDir

    NuGet (fun p -> 
        { p with   
            Authors = authors
            Project = project
            Summary = summary
            Description = description
            Version = release.NugetVersion
            ReleaseNotes = String.Join(Environment.NewLine, release.Notes)
            Tags = tags
            OutputPath = "bin"
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey"
//            Dependencies = ["FSharp.Data", GetPackageVersion packagesDir "FSharp.Data"]
            })
        (nugetDir + project + ".nuspec")
)

// --------------------------------------------------------------------------------------
// Generate the documentation

Target "GenerateDocs" (fun _ ->
    executeFSIWithArgs "docs/tools" "generate.fsx" ["--define:RELEASE"] [] |> ignore
)

// --------------------------------------------------------------------------------------
// Release Scripts

Target "ReleaseDocs" (fun _ ->
    let ghPages      = "gh-pages"
    let ghPagesLocal = "temp/gh-pages"
    Repository.clone "temp" (gitHome + "/" + gitName + ".git") ghPages
    Branches.checkoutBranch ghPagesLocal ghPages
    fullclean ghPagesLocal
    CopyRecursive "docs/output" ghPagesLocal true |> printfn "%A"
    CommandHelper.runSimpleGitCommand ghPagesLocal "add ." |> printfn "%s"
    let cmd = sprintf """commit -a -m "Update generated documentation for version %s""" release.NugetVersion
    CommandHelper.runSimpleGitCommand ghPagesLocal cmd |> printfn "%s"
    Branches.push ghPagesLocal
)

Target "Release" DoNothing

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "All" DoNothing

"Clean"
  ==> "RestorePackages"
  ==> "AssemblyInfo"
  ==> "Build"
//  ==> "RunTests"
  ==> "All"

"All" 
  ==> "CleanDocs"
//  ==> "GenerateDocs"
//  ==> "ReleaseDocs"
  ==> "MergeAssemblies"
  ==> "NuGet"
  ==> "Release"

RunTargetOrDefault "All"
