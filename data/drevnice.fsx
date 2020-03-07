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
type Row = Drevnice.Table8.Row

let createChartLine (rowMapFunc:Row -> #value) title (data:seq<Row>) = 
   data
   |> Seq.map (fun row -> (row.``Datum a čas``, rowMapFunc row))
   |> (fun data -> Chart.Line (data, Name=title))

let currentData = 
   Drevnice.Load("http://hydro.chmi.cz/hpps/hpps_prfdata.php?seq=307366").Tables.Table8.Rows
   |> Seq.rev
   |> Seq.cache

currentData 
   |> Seq.last 
   |>  (fun row -> row.``Stav [cm]``)
   |> printfn "Actual: %d cm"

currentData 
   |> Seq.maxBy (fun row -> row.``Stav [cm]``) 
   |> (fun row -> (row.``Stav [cm]``, row.``Datum a čas``)) 
   |> ( fun (level, date) -> printfn "Top: %d cm at %s" level date)

currentData
   |> Seq.windowed 3
   |> Seq.filter (fun [|a;b;c|] -> true)

[ 
   createChartLine (fun row -> row.``Teplota [°C]``) "Teplota [°C]"
   createChartLine (fun row -> row.``Stav [cm]``) "Stav [cm]"
   createChartLine (fun row -> row.``Průtok [m3s-1]``) "Průtok [m3s-1]"
]
|> Seq.map ( fun createChartLine -> createChartLine currentData)
|> Chart.Combine
|> Chart.WithLegend (Title="Dřevnice (Zlín)")
|> Chart.Show
