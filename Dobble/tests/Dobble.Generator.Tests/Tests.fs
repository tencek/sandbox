module Dobble.Tests

open Dobble
open Dobble.Tools
open Dobble.Generator
open Xunit

[<Fact>]
let ``Empty game has no card`` () =
   let emptyGameCardCount = 
      GenerateEmptyGame () 
      |> GetCardCount
   Assert.Equal(0, emptyGameCardCount)

[<Fact>]
let ``Original game has 55 cards`` () =
   let originalGameCardCount = 
      OriginalGame.Value 
      |> GetCardCount
   Assert.Equal(55, originalGameCardCount)

[<Fact>]
let ``Original game uses 57 symbols`` () =
   let originalTotalSymbolCount = 
      OriginalGame.Value 
      |> GetTotalSymbolCount
   Assert.Equal(57, originalTotalSymbolCount)

[<Fact>]
let ``Original game has 8 symbols per card`` () =
   OriginalGame.Value
   |> CheckSymbolCountPerCard 8
   |> Assert.True


let CheckEveryTwoCardsHaveRightOneSymbolInCommon game =
   let (Cards cards) = game
   cards
   |> List.collect (fun card ->
      let thisOne = List.singleton card
      List.allPairs thisOne (List.except thisOne cards))
   |> List.forall (fun (card1, card2) ->
      GetSymbolsInCommon card1 card2
      |> List.length
      |> (=) 1)

[<Fact>]
let ``Original - Every two cards have right one symbol in common`` () =
   OriginalGame.Value
   |> CheckEveryTwoCardsHaveRightOneSymbolInCommon 
   |> Assert.True

[<Fact>]
let ``Generated - Every two cards have right one symbol in common`` () =
   GenerateGame 6 3 ["dolphin";"spider";"cat";"ladybug";"chicken";"dog";"turtle";"t-rex";"dragon"]
   |> CheckEveryTwoCardsHaveRightOneSymbolInCommon 
   |> Assert.True


