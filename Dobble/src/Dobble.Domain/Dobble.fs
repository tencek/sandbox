namespace Dobble

type Symbol = Symbol of string

type Card = Symbols of Symbol list

type Game = Cards of Card list

module Tools = 
   let GetCardCount game = 
      let (Cards cards) = game
      cards.Length

   let CheckSymbolCountPerCard symbolCount game =
      let (Cards cards) = game
      cards
      |> List.forall ( 
         fun (Symbols symbols) ->
            symbols.Length = symbolCount)

   let GetSymbolsInCommon card1 card2 =
      let (Symbols s1, Symbols s2) = (card1, card2)
      Set.intersect (Set.ofList s1) (Set.ofList s2)
      |> List.ofSeq

   let GetTotalSymbolCount game = 
      let (Cards cards) = game
      cards
      |> List.collect (fun (Symbols symbols) -> symbols)
      |> List.distinct
      |> List.length



