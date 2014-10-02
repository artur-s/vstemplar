module TestHelpers

open Ploeh.AutoFixture
open Ploeh.AutoFixture.Xunit
open Ploeh.AutoFixture.AutoFoq


type AutoFoqDataAttribute() = 
    inherit AutoDataAttribute(
        Fixture().Customize(AutoFoqCustomization()))