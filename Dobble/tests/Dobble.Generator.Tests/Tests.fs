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

[<Fact>]
let ``Original game symbol counts`` () =
   let (Cards generatedCards) = OriginalGame.Value
   let symbolCounts = 
      CountSymbols generatedCards
      |> Seq.map (fun ((Name symbolName), count) -> (symbolName, count))
      |> Seq.sortBy (fun (symbolName, _count) -> symbolName.ToLower ())
      |> List.ofSeq
   
   let expectedSymbolCounts = 
      [("anchor", 8); ("apple", 8); ("bomb", 8); ("bulb", 7); ("cactus", 7); ("candle", 8); ("car", 8);
      ("carrot", 8); ("cat", 8); ("cheese", 8); ("chicken", 8); ("clock", 8); ("clover leaf", 8); ("clown", 8);
      ("crosshair", 8); ("dobble", 8); ("dog", 7); ("dolphin", 8); ("dragon", 8); ("drop", 8); ("dummy", 7); 
      ("exclamation mark", 7); ("eye", 7); ("fire", 8); ("flash", 8); ("flower", 7); ("G-clef", 8); 
      ("ghost", 8); ("hammer", 7); ("heart", 8); ("ice cube", 7); ("igloo", 8); ("Jolly Roger", 7); ("key", 8);
      ("knight", 8); ("ladybug", 7); ("lips", 8); ("maple leaf", 7); ("mark", 8); ("moon", 8); ("no-entry", 8);
      ("nursing bottle", 8); ("padlock", 8); ("pencil", 8); ("question mark", 7); ("scissors", 8); 
      ("snowflake", 8); ("snowman", 6); ("spider", 8); ("sun", 8); ("sunglasses", 8); ("t-rex", 7); 
      ("tree", 8); ("turtle", 8); ("web", 8); ("yin-yang", 8); ("zebra", 8)]
   
   Assert.Equal<(string * int) list>(expectedSymbolCounts, symbolCounts) 

let CheckEveryTwoCardsHaveRightOneSymbolInCommon game =
   let (Cards cards) = game
   cards
   |> Seq.collect (fun card ->
      let thisOne = Seq.singleton card
      Seq.allPairs thisOne (Seq.except thisOne cards))
   |> Seq.forall (fun (card1, card2) ->
      GetSymbolsInCommon card1 card2
     |> Seq.length
     |> (=) 1)

[<Fact>]
let ``Original - Every two cards have right one symbol in common`` () =
   OriginalGame.Value
   |> CheckEveryTwoCardsHaveRightOneSymbolInCommon 
   |> Assert.True

[<Fact>]
[<Trait("tag", "KnownBug")>]
let ``Generated - Every two cards have right one symbol in common`` () =
   GenerateGame 6 3 ["dolphin";"spider";"cat";"ladybug";"chicken";"dog";"turtle";"t-rex";"dragon"]
   |> CheckEveryTwoCardsHaveRightOneSymbolInCommon 
   |> Assert.True

let game = GenerateGame 7 3 ["dolphin";"spider";"cat";"ladybug";"chicken";"dog";"turtle";"t-rex";"dragon"]

let (Cards generatedCards) = game
let symbolCounts = 
   CountSymbols generatedCards

["A";"b";"a";"C";"c";"B"] |> List.sortBy (fun str -> str.ToLower ())

