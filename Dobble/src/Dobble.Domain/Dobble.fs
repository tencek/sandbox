namespace Dobble

type Symbol = Name of string

type Card = Symbols of Set<Symbol>

type Game = Cards of Set<Card>

module Tools = 
   let GetCardCount game = 
      let (Cards cards) = game
      cards.Count

   let CheckSymbolCountPerCard symbolCount game =
      let (Cards cards) = game
      cards
      |> Set.forall ( 
         fun (Symbols symbols) ->
            symbols.Count = symbolCount)

   let GetSymbolsInCommon card1 card2 =
      let (Symbols s1, Symbols s2) = (card1, card2)
      Set.intersect s1 s2

   let GetTotalSymbolCount game = 
      let (Cards cards) = game
      cards
      |> Seq.map (fun (Symbols symbols) -> symbols)
      |> Set.unionMany
      |> Set.count

