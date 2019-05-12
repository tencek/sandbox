module Tests

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


let CheckEveryTwoCardsHaveRightOneSymbolInCommon =
   function
   | Dobble.Cards cards ->
      cards
      |> List.collect (fun card ->
         let thisOne = (List.singleton card)
         List.allPairs thisOne (List.except thisOne cards))
      |> List.forall (fun (Symbols s1, Symbols s2) ->
         Set.intersect (Set.ofList s1) (Set.ofList s2)
         |> (fun intersection -> intersection.Count = 1))

[<Fact>]
let ``Every two cards have right one symbol in common`` () =
   OriginalGame.Value
   |> CheckEveryTwoCardsHaveRightOneSymbolInCommon 
   |> Assert.True

