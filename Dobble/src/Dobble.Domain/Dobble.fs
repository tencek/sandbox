namespace Dobble

type Symbol = Symbol of string

type Card = Symbols of Symbol list

type Game = Cards of Card list

module Tools = 
   let GetCardCount = 
      function
      | Cards cards -> cards.Length

   let CheckSymbolCountPerCard symbolCount game =
      game
      |> function
         | Cards cards ->
            cards
            |> List.forall (function
               | Symbols symbols -> symbols.Length = symbolCount)

   let GetTotalSymbolCount = 
      function
      | Cards cards -> 
         cards
         |> List.collect (fun (Symbols symbols) -> symbols)
         |> List.distinct
         |> List.length



