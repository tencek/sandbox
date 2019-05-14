namespace Dobble

open Dobble
open FSharp.Data

type OrigGame = CsvProvider<"resources/dobble-original-game.csv">

module Generator =
   let GenerateEmptyGame () =
        Cards List.empty

   let OriginalGame =
      lazy 
         OrigGame.GetSample().Rows
         |> Seq.map (fun row -> 
            [ row.Symbol1 ; row.Symbol2 ; row.Symbol3 ; row.Symbol4 ; row.Symbol5 ; row.Symbol6 ; row.Symbol7 ; row.Symbol8 ]
            |> List.map Name
            |> Symbols
            )
         |> List.ofSeq
         |> Cards


   let GenerateGame cardCount symbolNames =
      List.init cardCount (fun _i -> symbolNames |> List.head |> Name |> List.singleton |> Symbols )
      |> Cards
