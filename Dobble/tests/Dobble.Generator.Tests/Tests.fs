module Tests

open Dobble
open Dobble.Generator
open Xunit

[<Fact>]
let ``Empty game generator test`` () =
   let game = GenerateEmptyGame ()
   match game with
   | Cards cards ->
      Assert.Equal(0, cards.Length)
