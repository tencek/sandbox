// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"
#r @"..\packages\FSharp.Charting.2.1.0\lib\net45\FSharp.Charting.dll"

open FSharp.Data
open FSharp.Charting

[<Literal>]
let drevniceDataFile = __SOURCE_DIRECTORY__ + @"\Assets\drevnice.html"
type Drevnice = HtmlProvider<drevniceDataFile, Encoding="utf-8">

let currentData = 
   Drevnice.Load("http://hydro.chmi.cz/hpps/hpps_prfdata.php?seq=307366").Tables.Table8.Rows
   |> Seq.rev
   |> Seq.cache

currentData |> Seq.last |>  (fun row -> row.``Stav [cm]``) |> printfn "Actual: %A"

let tempGraph = 
    currentData
    |> Seq.map (fun row -> (row.``Datum a čas``, row.``Teplota [°C]``))
    |> (fun data -> Chart.Line (data, Name="Teplota [°C]"))

let levelGraph = 
    currentData
//    |> Seq.filter (fun row -> row.``Stav [cm]`` > 28)
    |> Seq.map (fun row -> (row.``Datum a čas``, row.``Stav [cm]``))
    |> (fun data -> Chart.Line (data, Name="Stav [cm]"))

let flowGraph = 
   currentData
//    |> Seq.filter (fun row -> row.``Stav [cm]`` > 28)
   |> Seq.map (fun row -> (row.``Datum a čas``, row.``Průtok [m3s-1]``))
   |> (fun data -> Chart.Line (data, Name="Průtok [m3s-1]"))

Chart.Combine [tempGraph ; levelGraph ; flowGraph]
|> Chart.WithLegend (Title="Dřevnice (Zlín)")
|> Chart.Show
