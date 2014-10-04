module TestHelpers

open Ploeh.AutoFixture
open Ploeh.AutoFixture.Xunit
open Ploeh.AutoFixture.AutoFoq


type AutoFoqDataAttribute() = 
    inherit AutoDataAttribute(
        Fixture().Customize(AutoFoqCustomization()))


let pairwiseAllEqual first second predicates =
    first |> Seq.zip second
    |> Seq.map (fun (f,s) -> predicates |> Seq.forall (fun p -> p s f))
    |> Seq.reduce (&&)
    
