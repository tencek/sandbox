namespace Dobble

open Dobble
open FSharp.Data

type OrigGame = CsvProvider<"resources/dobble-original-game.csv">

module Tools = 
   let GameCardCount game = 
      let (Cards cards) = game
      cards.Count

   let CardsSymbolsInCommon card1 card2 =
      let (Symbols s1, Symbols s2) = (card1, card2)
      Set.intersect s1 s2

   let GameTotalSymbolCount game = 
      let (Cards cards) = game
      cards
      |> Seq.map (fun (Symbols symbols) -> symbols)
      |> Set.unionMany
      |> Set.count

   let CardsCountSymbols cards =
      cards
      |> Seq.fold ( fun counts card -> 
         let (Symbols symbols) = card
         let newCounts = Seq.countBy (id) symbols
         Seq.append counts newCounts
         |> Seq.groupBy ( fun (symbol, _count) -> symbol)
         |> Seq.map (fun (symbol, counts) -> 
            let symbolCount = 
               counts 
               |> Seq.sumBy (fun (_symbol, count) -> count)
            (symbol, symbolCount))
         ) Seq.empty

module Generator =

   open Tools

   let GenerateEmptyGame () =
        Cards Set.empty

   let OriginalGame =
      lazy 
         OrigGame.GetSample().Rows
         |> Seq.map (fun row -> 
            [ row.Symbol1 ; row.Symbol2 ; row.Symbol3 ; row.Symbol4 ; row.Symbol5 ; row.Symbol6 ; row.Symbol7 ; row.Symbol8 ]
            |> Set.ofList
            |> Set.map Name
            |> Symbols
            )
         |> Set.ofSeq
         |> Cards

   let GenerateGame cardCount symbolsPerCard symbolNames =
      let symbols = Seq.map Name symbolNames
      Seq.init cardCount (fun i -> symbols |> Seq.skip i |> Seq.take 3 |> Set.ofSeq |> Symbols )
      |> Set.ofSeq
      |> Cards

   let CreateNewCard cards symbols =
      cards
      |> Seq.fold (fun newCard card -> 
         match (CardsSymbolsInCommon newCard card).Count with
         | 0 ->
            let (Symbols currentCardSymbols) = card
            let (Symbols newCardSymbols) = newCard
            let newSymbol = 
               CardsCountSymbols cards
               |> Seq.filter (fun (symbol, _count) -> Set.contains symbol currentCardSymbols)
               |> Seq.sortBy (fun (_symbol, count) -> count)
               |> Seq.head
               |> fst
            newCardSymbols 
            |> Set.add newSymbol
            |> Symbols
         | _ -> 
            newCard) (Symbols Set.empty)

   let GenerateGameTest cardCount symbolsPerCard symbolNames =
      let symbols = Seq.map Name symbolNames
      let addNewCard cards =
         ()
      seq { 1 .. cardCount}
      |> Seq.fold (fun cards _i -> 
         let symbols = symbols |> Seq.head |> Set.singleton
         let card = Symbols symbols
         Set.add card cards) Set.empty 
