namespace VsTemplar

module VsTemplate =
    
    type Parameters =
        {   VsProjFileLocation : string
            Description : string
            Target : string }
    
    val create : parameters:(Parameters -> Parameters) -> unit
