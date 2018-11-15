// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

open FSharp.Data
open System.Text.RegularExpressions
open System
open System.Globalization

let trace label x =
    printfn "%s: %A" label x
    x

[<Measure>] type degC

let (|TemperatureInCelsius|_|) str =
   let m = Regex.Match(str,"(.*)°C") 
   if (m.Success) then 
        (m.Groups.[1].Value |> float) * 1.0<degC> |> Some
   else 
        None  

type TemperatureWithTs = {dateTime:DateTime ; temperature:float<degC>}

let makeUrl stanice (date:DateTime) = 
    sprintf "http://www.in-pocasi.cz/meteostanice/stanice-historie.php?stanice=%s&historie=%s" stanice (date.ToString("MM-dd-yyyy"))

type InPocasi = HtmlProvider<"http://www.in-pocasi.cz/meteostanice/stanice-historie.php?stanice=zlin&historie=11-29-2017">

InPocasi().Tables.Table1.Rows
|> Seq.map (fun row -> row.Teplota)
|> Seq.map(function
    | TemperatureInCelsius temp -> Some temp
    | _ -> None)
|> Seq.choose(id)
|> Seq.average
|> printfn "Průměrná teplota: %A"

let loadTemperatures stanice date =
    let url = makeUrl stanice date
    let tupleToTempWithTs ((timeoption:DateTime option), temperature) = 
        match (timeoption, temperature) with
            | (Some time, temperature) -> 
                match temperature with
                | TemperatureInCelsius temp -> Some {dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second) ; temperature=temp}
                | _ -> None
            | _ -> None
    printfn "Loading data from %s" url
    System.Threading.Thread.Sleep(1000)
    InPocasi.Load(url).Tables.Table1.Rows
    |> Seq.map (fun row -> (row.Čas, row.Teplota))
    |> Seq.choose tupleToTempWithTs

let czechCultureInfo() =
    new CultureInfo("cs-CZ")

let parseDate culture dateStr =
    DateTime.Parse(dateStr, culture)

let parseCzechDate = czechCultureInfo() |> parseDate 

let dateRange (fromDate:System.DateTime) (toDate:System.DateTime) = 
    match (toDate.Date - fromDate.Date).Days with
    | days when days > 0 ->
        seq { 0 .. days }
        |> Seq.map (fun i -> fromDate.Date.AddDays(float i))
    | days when days < 0 ->
        seq { days .. 0 }
        |> Seq.rev
        |> Seq.map (fun i -> fromDate.Date.AddDays(float i))
    | _zero ->
        Seq.singleton(fromDate.Date)

let getAverageTemperature stanice fromDate toDate =
    dateRange fromDate toDate
    |> Seq.map (loadTemperatures stanice)
    |> Seq.concat
    |> Seq.filter (fun tempWithTs -> tempWithTs.dateTime >= fromDate && tempWithTs.dateTime <= toDate)
    |> Seq.averageBy (fun tempWithTs -> tempWithTs.temperature)

type Data2018 = CsvProvider<"https://docs.google.com/spreadsheets/d/e/2PACX-1vT4Orw8HCbYBHemHKfm7Pkoy2bLmAcjhGLM9e1wqA5xiEY-7cKkPLQ0kvNAS9ygm4TJ2nW_5i0tY1ot/pub?gid=950757578&single=true&output=csv">
let dates =  
    (new Data2018()).Rows
    |> Seq.map (fun row -> row.Datum)
    |> Seq.rev
    |> Seq.take 2
    |> Seq.rev


seq { yield "zlin" ; yield "zlin_centrum" }
|> Seq.iter (fun stanice -> 
    dates
    |> Seq.map parseCzechDate
    |> Seq.pairwise
    |> Seq.map (fun (fromDate, toDate) -> (fromDate, toDate, getAverageTemperature stanice fromDate toDate))
    |> Seq.iter (fun (fromDate, toDate, averageTemp) -> printfn "%s - %s: %f °C" (fromDate.ToString()) (toDate.ToString()) averageTemp)
)
