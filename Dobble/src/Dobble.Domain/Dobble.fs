namespace Dobble

type Symbol = Name of string

type Card = Symbols of Set<Symbol>

type Deck = Cards of Set<Card>

module Tools = 
   let DeckAllSymbols deck = 
      let (Cards cards) = deck
      cards
      |> Set.map (fun (Symbols symbols) -> symbols)
      |> Set.unionMany

   let CardsSymbolsInCommon card1 card2 =
      let (Symbols s1, Symbols s2) = (card1, card2)
      Set.intersect s1 s2

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
