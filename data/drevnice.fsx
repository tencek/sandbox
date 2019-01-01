// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"
#r @"..\packages\FSharp.Charting.2.1.0\lib\net45\FSharp.Charting.dll"

open FSharp.Data
open FSharp.Charting

type Drevnice = 
  HtmlProvider<"http://hydro.chmi.cz/hpps/hpps_prfdata.php?seq=307366", Encoding="utf-8">

Drevnice().Tables.Table8.Rows.[0].``Stav [cm]`` |> printfn "Actual: %A"

let rows = 
    Drevnice().Tables.Table8.Rows
    |> Seq.cache

let tempGraph = 
    rows
    |> Seq.map (fun row -> (row.``Datum a čas``, row.``Teplota [°C]``))
    |> Chart.Line

let levelGraph = 
    rows
//    |> Seq.filter (fun row -> row.``Stav [cm]`` > 28)
    |> Seq.map (fun row -> (row.``Datum a čas``, row.``Stav [cm]``))
    |> Chart.Line

Chart.Combine [tempGraph ; levelGraph ]
|> Chart.Show
