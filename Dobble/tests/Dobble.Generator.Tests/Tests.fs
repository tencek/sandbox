module Dobble.Tests

open Dobble
open Dobble.Tools
open Dobble.Original
open Dobble.Generator
open Xunit

[<Fact>]
let ``Empty game has no card`` () =
   let emptyGameCardCount = 
      GenerateEmptyGame () 
      |> ( fun (Cards cards) -> cards.Count)
   Assert.Equal(0, emptyGameCardCount)

[<Fact>]
let ``Original game has 55 cards`` () =
   let originalGameCardCount = 
      Original.Game.Value 
      |> ( fun (Cards cards) -> cards.Count)
   Assert.Equal(55, originalGameCardCount)

[<Fact>]
let ``Original game uses 57 symbols`` () =
   let originalTotalSymbolCount = 
      Original.Game.Value 
      |> GameAllSymbols
      |> Set.count
   Assert.Equal(57, originalTotalSymbolCount)

[<Fact>]
let ``Original game has 8 symbols per card`` () =
   let expectedSymbolCount = 8
   let (Cards cards) = Original.Game.Value
   cards
   |> Set.iter ( 
      fun (Symbols symbols) ->
         Assert.Equal(expectedSymbolCount, symbols.Count))

[<Fact>]
let ``Original game symbol counts`` () =
   let (Cards generatedCards) = Original.Game.Value
   let symbolCounts = 
      CardsCountSymbols generatedCards
      |> Seq.map (fun ((Name symbolName), count) -> (symbolName, count))
      |> Seq.sortBy (fun (symbolName, _count) -> symbolName.ToLower ())
      |> List.ofSeq
   
   let expectedSymbolCounts = 
      [("anchor", 8); ("apple", 8); ("bomb", 8); ("bulb", 7); ("cactus", 7); ("candle", 8); ("car", 8);
      ("carrot", 8); ("cat", 8); ("cheese", 8); ("chicken", 8); ("clock", 8); ("clover leaf", 8); ("clown", 8);
      ("crosshair", 8); ("dobble", 8); ("dog", 7); ("dolphin", 8); ("dragon", 8); ("dummy", 7); 
      ("exclamation mark", 7); ("eye", 7); ("fire", 8); ("flash", 8); ("flower", 7); ("G-clef", 8); 
      ("ghost", 8); ("hammer", 7); ("heart", 8); ("ice cube", 7); ("igloo", 8); ("Jolly Roger", 7); ("key", 8);
      ("knight", 8); ("ladybug", 7); ("lips", 8); ("maple leaf", 7); ("mark", 8); ("moon", 8); ("no-entry", 8);
      ("nursing bottle", 8); ("padlock", 8); ("pencil", 8); ("question mark", 7); ("raindrop", 8); 
      ("scissors", 8); ("snowflake", 8); ("snowman", 6); ("spider", 8); ("sun", 8); ("sunglasses", 8); 
      ("t-rex", 7); ("tree", 8); ("turtle", 8); ("web", 8); ("yin-yang", 8); ("zebra", 8)]
   
   Assert.Equal<(string * int) list>(expectedSymbolCounts, symbolCounts) 

let CheckEveryTwoCardsHaveRightOneSymbolInCommon game =
   let (Cards cards) = game
   cards
   |> Seq.collect (fun card ->
      let thisOne = Seq.singleton card
      Seq.allPairs thisOne (Seq.except thisOne cards))
   |> Seq.forall (fun (card1, card2) ->
      CardsSymbolsInCommon card1 card2
     |> Seq.length
     |> (=) 1)

[<Fact>]
let ``Original - Every two cards have right one symbol in common`` () =
   Original.Game.Value
   |> CheckEveryTwoCardsHaveRightOneSymbolInCommon 
   |> Assert.True

[<Fact>]
let ``Generated - Every two cards have right one symbol in common`` () =
   GenerateGame 7 3 ["dolphin";"spider";"cat";"ladybug";"chicken";"dog";"turtle";"t-rex";"dragon"]
   |> CheckEveryTwoCardsHaveRightOneSymbolInCommon 
   |> Assert.True

[<Fact>]
let ``1st generated card is 123`` () =
   // arrange
   let symbols = ["1";"2";"3";"4";"5";"6";"7"] |> List.map Name
   let cards = Set.empty 
   let expectedCard =  ["1";"2";"3"] |> List.map Name  |> Set.ofList |> Symbols
   // act
   let newCard = CreateNextCard cards 3 symbols
   // assert
   Assert.Equal(expectedCard, newCard)

[<Fact>]
let ``2nd generated card is 145`` () =
   // arrange
   let symbols = ["1";"2";"3";"4";"5";"6";"7"] |> List.map Name
   let cards = 
      [["1";"2";"3"]] 
      |> List.map (List.map Name >> Set.ofList >> Symbols) 
      |> Set.ofList 
   let expectedCard =  ["1";"4";"5"] |> List.map Name  |> Set.ofList |> Symbols
   // act
   let newCard = CreateNextCard cards 3 symbols
   // assert
   Assert.Equal(expectedCard, newCard)

[<Fact>]
let ``3rd generated card is 246`` () =
   // arrange
   let symbols = ["1";"2";"3";"4";"5";"6";"7"] |> List.map Name
   let cards = 
      [["1";"2";"3"];["1";"4";"5"]] 
      |> List.map (List.map Name >> Set.ofList >> Symbols) 
      |> Set.ofList 
   let expectedCard =  ["2";"4";"6"] |> List.map Name  |> Set.ofList |> Symbols
   // act
   let newCard = CreateNextCard cards 3 symbols
   // assert
   Assert.Equal(expectedCard, newCard)

[<Fact>]
let ``4th generated card is 356`` () =
   // arrange
   let symbols = ["1";"2";"3";"4";"5";"6";"7"] |> List.map Name
   let cards = 
      [["1";"2";"3"];["1";"4";"5"];["2";"4";"6"]] 
      |> List.map (List.map Name >> Set.ofList >> Symbols)
      |> Set.ofList
   let expectedCard =  ["3";"5";"6"] |> List.map Name  |> Set.ofList |> Symbols
   // act
   let newCard = CreateNextCard cards 3 symbols
   // assert
   Assert.Equal(expectedCard, newCard)

[<Fact>]
let ``5th generated card is 167`` () =
   // arrange
   let symbols = ["1";"2";"3";"4";"5";"6";"7"] |> List.map Name
   let cards = 
      [["1";"2";"3"];["1";"4";"5"];["2";"4";"6"];["3";"5";"6"]] 
      |> List.map (List.map Name >> Set.ofList >> Symbols)
      |> Set.ofList
   let expectedCard =  ["1";"6";"7"] |> List.map Name  |> Set.ofList |> Symbols
   // act
   let newCard = CreateNextCard cards 3 symbols
   // assert
   Assert.Equal(expectedCard, newCard)

[<Fact>]
let ``6th generated card is 257`` () =
   // arrange
   let symbols = ["1";"2";"3";"4";"5";"6";"7"] |> List.map Name
   let cards = 
      [["1";"2";"3"];["1";"4";"5"];["2";"4";"6"];["3";"5";"6"];["1";"6";"7"]] 
      |> List.map (List.map Name >> Set.ofList >> Symbols)
      |> Set.ofList
   let expectedCard =  ["2";"5";"7"] |> List.map Name  |> Set.ofList |> Symbols
   // act
   let newCard = CreateNextCard cards 3 symbols
   // assert
   Assert.Equal(expectedCard, newCard)

[<Fact>]
let ``7th generated card is 347`` () =
   // arrange
   let symbols = ["1";"2";"3";"4";"5";"6";"7"] |> List.map Name
   let cards = 
      [["1";"2";"3"];["1";"4";"5"];["2";"4";"6"];["3";"5";"6"];["1";"6";"7"];["2";"5";"7"]] 
      |> List.map (List.map Name >> Set.ofList >> Symbols)
      |> Set.ofList
   let expectedCard =  ["3";"4";"7"] |> List.map Name  |> Set.ofList |> Symbols
   // act
   let newCard = CreateNextCard cards 3 symbols
   // assert
   Assert.Equal(expectedCard, newCard)

[<Fact>]
let ``Generated game 7,3`` () =
   // arrange
   let symbolNames = ["1";"2";"3";"4";"5";"6";"7"]
   let expectedGame = 
      [["1";"2";"3"];["1";"4";"5"];["2";"4";"6"];["3";"5";"6"];["1";"6";"7"];["2";"5";"7"];["3";"4";"7"]] 
      |> List.map (List.map Name >> Set.ofList >> Symbols)
      |> Set.ofList
      |> Cards
   // act
   let generatedGame = GenerateGame 7 3 symbolNames
   // assert
   Assert.Equal(expectedGame, generatedGame)

[<Fact>]
[<Trait("tag", "KnownBug")>]
let ``Regenerated original game`` () = 
   // arrange
   let symbolNames = 
      Original.Game.Value
      |> GameAllSymbols
      |> Set.map (fun (Name symbolName) -> symbolName)
   let symbolsPerCard =
      Original.Game.Value
      |> (fun (Cards cards) -> 
            cards 
            |> Seq.head 
            |> (fun (Symbols symbols) -> symbols.Count))
   let cardCount = 
      Original.Game.Value
      |> (fun (Cards cards) -> cards.Count)
   //act
   let generatedGame = GenerateGame cardCount symbolsPerCard symbolNames
   // assert
   Assert.Equal(Original.Game.Value, generatedGame)

[<Fact>]
let ``Generated game 55,8`` () =
   // arrange
   let AZ = "ABCDEFGHIJKLMNOPQRSTUVWXZYZ" |> Seq.map string
   let AAZZ = AZ |> Seq.collect (fun a -> AZ |> Seq.map (fun b -> a+b))
   let symbolNames = Seq.append AZ AAZZ
   let expectedSymbolsCount = 57
   // act
   let generatedGame = GenerateGame 55 8 symbolNames
   let generatedSymbolsCount = 
      generatedGame 
      |> GameAllSymbols
      |> Set.count
   // assert
   Assert.Equal(expectedSymbolsCount, generatedSymbolsCount)

