namespace VsTemplar


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

 //TODO:
type TemplateExportParameters = 
    {   SourceProjectDirectory : string
        TargetDirectory : string
        /// a template parameter for project name in VS project file. Default is '$safeprojectname$'
        ProjectNameTemplateParameter:string }

