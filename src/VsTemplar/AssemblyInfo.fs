namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("VsTemplar")>]
[<assembly: AssemblyProductAttribute("VsTemplar")>]
[<assembly: AssemblyDescriptionAttribute("Generates *.vstemplate file based on VS project file: *.csproj, *.fsproj, etc.")>]
[<assembly: AssemblyVersionAttribute("0.0.6")>]
[<assembly: AssemblyFileVersionAttribute("0.0.6")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.6"
