namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("VsTemplar")>]
[<assembly: AssemblyProductAttribute("VsTemplar")>]
[<assembly: AssemblyDescriptionAttribute("A tool that generates *.vstemplate file based on provided Visual Studio project file *.csproj, *.fsproj, etc.")>]
[<assembly: AssemblyVersionAttribute("0.0.1")>]
[<assembly: AssemblyFileVersionAttribute("0.0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.1"
