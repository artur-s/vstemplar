namespace VsTemplar

module VsTemplate =
    
    type MetadataCreationParameters =
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
    ///     VsTemplate.CreateMetadata (fun p -> {p with VsProjFileLocation = ".\YourProjectHere.csproj"
    ///                                         Target =  @"D:\Temp\MyTemplate.vstemplate"})
    ///
    val CreateMetadata : setParams:(MetadataCreationParameters -> MetadataCreationParameters) -> unit

    //TODO:
    type TemplateExportParameters = 
        {   SourceProjectDirectory : string
            TargetDirectory : string }

    //TODO: update usage sample
    /// Creates zipped template file from source project.
    /// ## Parameters
    /// 
    ///  - `setParams` - Function used to update the default Parameters value.
    /// 
    val ExportAsTemplate : setParams:(TemplateExportParameters -> TemplateExportParameters) -> unit 
