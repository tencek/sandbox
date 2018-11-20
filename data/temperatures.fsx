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

type Stanice = Stanice of string

type DateRange = { FromDate:DateTime ; ToDate:DateTime }

let makeUrl (Stanice stanice) (date:DateTime) = 
    sprintf "http://www.in-pocasi.cz/meteostanice/stanice-historie.php?stanice=%s&historie=%s" stanice (date.ToString("MM-dd-yyyy"))

type InPocasi = HtmlProvider<"http://www.in-pocasi.cz/meteostanice/stanice-historie.php?stanice=zlin&historie=11-01-2018", Encoding="utf-8">

InPocasi().Tables.Table1.Rows
|> Seq.map (fun row -> row.Teplota)
|> Seq.choose(function
    | TemperatureInCelsius temp -> Some temp
    | _ -> None)
|> Seq.average
|> printfn "Průměrná teplota: %A"

let loadTemperatures stanice date =
    try
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
    with
    | ex -> 
        printfn "%s" ex.Message
        Seq.empty

let czechCultureInfo () =
    new CultureInfo("cs-CZ")

let parseDate culture dateStr =
    DateTime.Parse(dateStr, culture)

let parseDouble (culture:IFormatProvider) floatStr = 
    Double.Parse(floatStr, culture)

let parseCzechDate = czechCultureInfo () |> parseDate 
let parseCzechDouble = czechCultureInfo () |> parseDouble

let createDateList dateRange = 
    match (dateRange.ToDate.Date - dateRange.FromDate.Date).Days with
    | days when days > 0 ->
        seq { 0 .. days }
        |> Seq.map (fun i -> dateRange.FromDate.Date.AddDays(float i))
    | days when days < 0 ->
        seq { days .. 0 }
        |> Seq.rev
        |> Seq.map (fun i -> dateRange.FromDate.Date.AddDays(float i))
    | _zero ->
        Seq.singleton(dateRange.FromDate.Date)

let getAverageTemperature stanice dateRange =
    createDateList dateRange
    |> Seq.map (loadTemperatures stanice)
    |> Seq.concat
    |> Seq.filter (fun tempWithTs -> tempWithTs.dateTime >= dateRange.FromDate && tempWithTs.dateTime <= dateRange.ToDate)
    |> Seq.averageBy (fun tempWithTs -> tempWithTs.temperature)

type Data2018 = CsvProvider<"https://docs.google.com/spreadsheets/d/e/2PACX-1vT4Orw8HCbYBHemHKfm7Pkoy2bLmAcjhGLM9e1wqA5xiEY-7cKkPLQ0kvNAS9ygm4TJ2nW_5i0tY1ot/pub?gid=950757578&single=true&output=csv", Encoding="utf-8">
//let newDates =  
//    (new Data2018()).Rows
//    |> Seq.skip 2
//    |> Seq.map (fun row -> (parseCzechDate(row.Datum), row.``Avg(t)``))
//    |> Seq.choose(function
//        | (date, temp) when 
//            temp.Length = 0 &&
//            date > new DateTime (2018, 9, 15) -> Some date
//        | _ -> None)

let newDates = 
    (new Data2018()).Rows
    |> Seq.skip 2
    |> Seq.map (fun row -> parseCzechDate(row.Datum))
    |> Seq.rev
    |> Seq.take 2
    |> Seq.rev

let tempData =
    newDates
    |> Seq.pairwise
    |> Seq.collect (fun (fromDate, toDate) ->
        let dateRange = { FromDate=fromDate ; ToDate=toDate }
        seq { yield Stanice "zlin" ; yield Stanice "zlin_centrum" }
        |> Seq.map (fun stanice -> (dateRange, stanice, getAverageTemperature stanice dateRange)))
    |> Seq.cache

tempData |> Seq.iter (fun (dateRange, (Stanice stanice), averageTemp) -> printfn "%s - %s: %15s: %f °C" (dateRange.FromDate.ToString()) (dateRange.ToDate.ToString()) stanice averageTemp)
