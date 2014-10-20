namespace System
open System.Reflection
open System.Runtime.CompilerServices

[<assembly: AssemblyTitleAttribute("VsTemplar")>]
[<assembly: AssemblyProductAttribute("VsTemplar")>]
[<assembly: AssemblyDescriptionAttribute("Generates Visual Studio project template based on VS project(s).")>]
[<assembly: AssemblyVersionAttribute("0.1.1")>]
[<assembly: AssemblyFileVersionAttribute("0.1.1")>]
[<assembly: InternalsVisibleToAttribute("VsTemplar.Tests")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.1.1"
