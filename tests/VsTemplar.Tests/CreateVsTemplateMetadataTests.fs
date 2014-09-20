module  VsTemplar.Tests.CreateVsTemplateMetadataTests

open System
open Xunit
open Xunit.Extensions
open VsTemplar

[<Fact>]
let ``It should create VsTemplate metadata file based on provided VS project path`` = 

    VsTemplate.CreateMetadataVsTemplateMetadata(fun p -> {p with 
                                                                VsProjFileLocation = @"C:\Projects\Git\temp\MyProject\Src\IQ.Platform.Framework.Common\IQ.Platform.Framework.Common.csproj"
                                                                //TODO: if only directory path is provided, create file with project name, e.g. IQ.Platform.Framework.Common.vstemplate
                                                                // Target = @"C:\Projects\Git\temp\MyProject\Template" 
                                                                Target = @"C:\Projects\Git\temp\MyProject\Template\MyTemplate.vstemplate"
                                                                })
