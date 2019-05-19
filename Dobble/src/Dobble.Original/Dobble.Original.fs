module Dobble.Original

open FSharp.Data
open Dobble

type private OrigDeck = CsvProvider<"resources/dobble-original-game.csv">

let Deck =
   lazy 
      OrigDeck.GetSample().Rows
      |> Seq.map (fun row -> 
         set [ row.Symbol1 ; row.Symbol2 ; row.Symbol3 ; row.Symbol4 ; row.Symbol5 ; row.Symbol6 ; row.Symbol7 ; row.Symbol8 ]
         |> Set.map Name
         |> Symbols
         )
      |> Set.ofSeq
      |> Cards
