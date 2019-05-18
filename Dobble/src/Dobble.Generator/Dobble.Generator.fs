module Dobble.Generator

open Dobble.Tools

let GenerateEmptyGame () =
      Cards Set.empty

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
               newSymbol
               |> (fun (Name symbolName) -> symbolName)
               |> printf "%s, "
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
      newCard 
      |> (fun (Symbols symbols) -> symbols)
      |> Set.map ( fun (Name symbolName) -> symbolName)
      |> Set.toList
      |> List.sort
      |> printfn "New card: %A"
      Set.add newCard cards ) Set.empty 
   |> Cards
