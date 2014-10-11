module Option


let Else second = function
    | Some _ as first -> first
    | None -> second

let filter p = function
    | Some v -> if p v then Some v else None
    | _ -> None

let ofNullable<'T when 'T : null> (value:'T) = 
    match value with 
    | null -> None 
    | i -> Some i
