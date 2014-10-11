namespace VsTemplar

open System.Reflection


module VsTemplate =

    
    /// Creates *.vstemplate file from provided *.csproj file.
    /// ## Parameters
    /// 
    ///  - `setParams` - Function used to update the default Parameters value.
    /// 
    /// ## Sample usage
    ///
    ///     VsTemplate.CreateMetadata (fun p -> {p with VsProjFileLocation = ".\YourProjectHere.csproj"
    ///                                         Target =  @"D:\Temp\MyTemplate.vstemplate"})
    ///
    val CreateMetadataVsTemplateMetadata : setParams:(MetadataCreationParameters -> MetadataCreationParameters) -> unit

//    val CreateMetadataVsTemplateMetadataInterop : setParam:(System.Func<MetadataCreationParameters,MetadataCreationParameters>) -> unit


    //TODO: update usage sample
    /// Creates zipped template file from source project.
    /// ## Parameters
    /// 
    ///  - `setParams` - Function used to update the default Parameters value.
    /// 
    val ExportAsTemplate : setParams:(TemplateExportParameters -> TemplateExportParameters) -> unit 
