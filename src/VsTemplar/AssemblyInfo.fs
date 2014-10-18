namespace System
open System.Reflection
open System.Runtime.CompilerServices

[<assembly: AssemblyTitleAttribute("VsTemplar")>]
[<assembly: AssemblyProductAttribute("VsTemplar")>]
[<assembly: AssemblyDescriptionAttribute("Generates *.vstemplate file based on VS project file: *.csproj, *.fsproj, etc.")>]
[<assembly: AssemblyVersionAttribute("0.1.0")>]
[<assembly: AssemblyFileVersionAttribute("0.1.0")>]
[<assembly: InternalsVisibleToAttribute("VsTemplar.Tests")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.1.0"
