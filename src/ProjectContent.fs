module ProjectContent

open System

// represents project items in vstemplate file
type ContentItem = 
    | FileContent of string
    | FolderContent of string * ContentItem list

let rec pathAndFile = function
    | x::[] -> ([],x)
    | x::xs -> let (p,f) = pathAndFile xs
               ([x] @ p, f) 
    | [] -> failwith "list cannot be empty"
//pathAndFile ["c";"dir";"subdir";"file.cs"]

let createProjectContent (itemPaths:string list) =
    let create (path,file) =
        let rec create content path =
            match path with
            | d::dirs -> create (FolderContent (d, [content])) dirs 
            | [] -> content
        List.rev path |> create (FileContent file)

    [for ip in itemPaths ->
        match ip.Split ([|@"\"|], StringSplitOptions.RemoveEmptyEntries) with
        | [||] -> failwith "no content to include"
        | includes -> 
            includes |> List.ofArray
            |> pathAndFile
            |> create
            ]

let rec mergeContentItems items =
    items |> Seq.groupBy (function 
                        | FileContent name -> name
                        | FolderContent (name, subs) -> name)
    |> Seq.map (fun (k,group) -> 
                let grouped = group 
                              |> Seq.collect (fun i -> match i with
                                                        | FolderContent (_, subs) -> subs |> Seq.ofList
                                                        | _ -> Seq.empty)
                              |> mergeContentItems
                match List.ofSeq grouped with
                 | [] -> FileContent k
                 | items -> FolderContent (k,items))

//[
//@"C\dir1\subdir1\file111";
//@"C\dir1\subdir1\file222";
//@"C\dir1\subdir1\file222";
//@"C\dir1\subdir2\file333";
//@"file1.cs";
//@"file3.fs";
//@"D\file333"
//]
//|> createProjectContent
//|> mergeContentItems

    