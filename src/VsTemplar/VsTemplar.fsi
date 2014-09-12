namespace VsTemplar

module VsTemplate =
    
    type Parameters =
        {   VsProjFileLocation : string
            Description : string
            Target : string }
    
    /// Creates *.vstemplate file from provided *.csproj file.
    /// ## Parameters
    /// 
    ///  - `setParams` - Function used to update the default Parameters value.
    /// 
    /// ## Sample usage
    ///
    ///     VsTemplate.create (fun p -> {p with VsProjFileLocation = ".\YourProjectHere.csproj"
    ///                                         Target =  @"D:\Temp\MyTemplate.vstemplate"})
    ///
    val Create : setParams:(Parameters -> Parameters) -> unit


//    val 
