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
      |> Set.toSeq
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

   let CreateNextCard cards symbolsPerCard symbols =
      let symbolsCount = CardsCountSymbols cards
      let newSymbols = 
         cards
            |> Set.toSeq
            |> Seq.fold (fun newSymbols card -> 
               match (CardsSymbolsInCommon (Symbols newSymbols) card).Count with
               | 0 ->
                  let (Symbols currentCardSymbols) = card
                  let doNotUseAnyMore =
                     cards 
                     |> Set.filter (fun (Symbols symbols) -> 
                        symbols
                        |> Set.intersect newSymbols
                        |> Set.isEmpty
                        |> not)
                     |> Seq.collect ( fun (Symbols symbols) -> symbols)
                     |> Set.ofSeq
                  let newSymbol = 
                     symbolsCount
                     |> Seq.filter (fun (symbol, _count) -> 
                        Set.contains symbol currentCardSymbols 
                        && not (Set.contains symbol doNotUseAnyMore) )
                     |> Seq.sortBy (fun (_symbol, count) -> count)
                     |> Seq.head
                     |> fst
                  Set.add newSymbol newSymbols
               | _ -> 
                  newSymbols) Set.empty
      let fill =
         symbols 
         |> Seq.filter (fun symbol -> 
            symbolsCount
            |> Seq.map fst
            |> Seq.contains symbol
            |> not)
         |> Seq.take (symbolsPerCard - newSymbols.Count)
         |> Set.ofSeq
      
      Set.union newSymbols fill
      |> Symbols

   let GenerateGame cardCount symbolsPerCard symbolNames =
      let symbols = symbolNames |> Seq.map Name
      seq { 1 .. cardCount}
      |> Seq.fold (fun cards _i -> 
         let newCard = CreateNextCard cards symbolsPerCard symbols
         Set.add newCard cards ) Set.empty 
      |> Cards
