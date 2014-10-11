namespace VsTemplar

type WizardData = string

type WizardExtension =
    {
        Assembly : System.Reflection.AssemblyName
        FullClassName : string        
    }

type WizardTemplate =
    {
        Extension : WizardExtension
        Data : WizardData
    }

type MetadataCreationParameters =
    {   VsProjFileLocation : string
        Description : string
        Target: string
        WizardTemplate : WizardTemplate option } // seq

type ProjectType =
        | CSharp = 1
        | FSharp = 2
        | VisualBasic = 3
        | VS = 4
        | Web = 5

// ref: http://msdn.microsoft.com/en-us/library/ms171397.aspx
type ProjectTemplateLinkItem =
    {   Name:string
        Location:string }
    
// ref: http://msdn.microsoft.com/en-us/library/ms171399.aspx
and SolutionFolderItem =
    {   Name:string
        ProjectStructureItem: SolutionItem seq}
    
and SolutionItem =
    | ProjectTemplateLink of ProjectTemplateLinkItem
    | SolutionFolder of SolutionFolderItem

type SolutionContent = 
    | ExplicitContent of SolutionItem seq
    | InferredContent

type RootTemplate =
    {   Name:string
        Description:string
        IconPath:string
        ProjectType:ProjectType option
        ProjectSubType:string
        TemplateGroupID:string
        RequiredFrameworkVersion:string
        DefaultName:string
        CreateNewFolder:bool
        Content:SolutionContent
        Wizard:WizardTemplate option} //TODO: use default if none

type TemplateExportParameters = 
    {   SourceProjectDirectory : string
        TargetDirectory : string
        /// a template parameter for project name in VS project file. Default is '$safeprojectname$'
        ProjectNameTemplateParameter:string
        Root:RootTemplate option
        ChildWizard:WizardTemplate option}

