namespace Dobble

open Dobble
open FSharp.Data

type OrigGame = CsvProvider<"resources/dobble-original-game.csv">

module Generator =
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
      Seq.init cardCount (fun _i -> symbols |> Seq.head |> Set.singleton |> Symbols )
      |> Set.ofSeq
      |> Cards
