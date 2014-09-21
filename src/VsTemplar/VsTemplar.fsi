namespace VsTemplar

open System.Reflection

module VsTemplate =

    type WizardTemplate =
        {
            Assembly : System.Reflection.AssemblyName
            FullClassName : string
        }

    type MetadataCreationParameters =
        {   VsProjFileLocation : string
            Description : string
            Target: string
            WizardTemplate : WizardTemplate option } // seq
    
    
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

    //TODO:
    type TemplateExportParameters = 
        {   SourceProjectDirectory : string
            TargetDirectory : string
            /// a template parameter for project name in VS project file. Default is '$safeprojectname$'
            ProjectNameTemplateParameter:string }

    //TODO: update usage sample
    /// Creates zipped template file from source project.
    /// ## Parameters
    /// 
    ///  - `setParams` - Function used to update the default Parameters value.
    /// 
    val ExportAsTemplate : setParams:(TemplateExportParameters -> TemplateExportParameters) -> unit 
