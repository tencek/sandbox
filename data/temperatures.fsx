// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"

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

let czechCultureInfo () =
   new CultureInfo("cs-CZ")

let makeUrl (Stanice stanice) (date:DateTime) = 
    sprintf "http://www.in-pocasi.cz/aktualni-pocasi/%s/?historie=%s" stanice (date.ToString("yyyy-MM-dd", czechCultureInfo()))


type InPocasiNow = HtmlProvider<"http://www.in-pocasi.cz/aktualni-pocasi/zlin_centrum/", Encoding="utf-8">

InPocasiNow().Tables.Zlín.Rows.[0]
|> (fun row -> (row.``Čas měření``, row.Teplota))
|> (fun (time, temp) -> printfn "Teplota v %s byla %s." (time.ToString()) temp)

[<Literal>]
let historyDataFile = __SOURCE_DIRECTORY__ + @"\Assets\historicke-udaje.html"
type InPocasiHistory = HtmlProvider<historyDataFile, Encoding="utf-8">

InPocasiHistory().Tables.Data.Rows
|> Seq.map (fun row -> row.Teplota)
|> Seq.choose(function
    | TemperatureInCelsius temp -> Some temp
    | _ -> None)
|> Seq.average
|> printfn "Průměrná teplota: %A"

let loadTemperatures stanice date =
    try
        let url = makeUrl stanice date
        let tupleToTempWithTs ((time:TimeSpan), temperature) = 
            match temperature with
            | TemperatureInCelsius temp -> Some {dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds) ; temperature=temp}
            | _ -> None
        printfn "Loading data from %s" url
        System.Threading.Thread.Sleep(1000)
        Http.RequestString(url)
        |> ( fun webpage -> Regex.Replace(webpage, @"\<h1\>[^<]*\<\/h1\>", "<H1>Data</H1>", RegexOptions.Multiline))
        |> InPocasiHistory.Parse
        |> (fun historyData -> historyData.Tables.Data.Rows)
        |> Seq.map (fun row -> (row.``Čas měření``, row.Teplota))
        |> Seq.choose tupleToTempWithTs
    with
    | ex -> 
        printfn "%s" ex.Message
        Seq.empty

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

type Data2019 = CsvProvider<"https://docs.google.com/spreadsheets/d/e/2PACX-1vT4Orw8HCbYBHemHKfm7Pkoy2bLmAcjhGLM9e1wqA5xiEY-7cKkPLQ0kvNAS9ygm4TJ2nW_5i0tY1ot/pub?gid=615668307&single=true&output=csv", Encoding="utf-8">

//let newDates = 
//   [ "16.11.2019 19:17" ; "24.11.2019 21:03" ]
//   |> Seq.map parseCzechDate

//let newDates = 
//   (new Data2019()).Rows
//   |> Seq.skip 11
//   |> Seq.map (fun row -> parseCzechDate(row.Datum))
//   |> Seq.take 2

type Data2020 = CsvProvider<"https://docs.google.com/spreadsheets/d/e/2PACX-1vT4Orw8HCbYBHemHKfm7Pkoy2bLmAcjhGLM9e1wqA5xiEY-7cKkPLQ0kvNAS9ygm4TJ2nW_5i0tY1ot/pub?gid=413454607&single=true&output=csv", Encoding="utf-8", Culture="cz-CZ", Schema="Datum=string">

let newDates = 
   (new Data2020()).Rows
   |> Seq.skip 1
   |> Seq.map (fun row -> parseCzechDate(row.Datum))
   //|> Seq.rev
   //|> Seq.take 3
   //|> Seq.rev

let tempData =
    newDates
    |> Seq.pairwise
    |> Seq.collect (fun (fromDate, toDate) ->
        let dateRange = { FromDate=fromDate ; ToDate=toDate }
        seq { yield Stanice "zlin" ; yield Stanice "zlin_centrum" }
        |> Seq.map (fun stanice -> (dateRange, stanice, getAverageTemperature stanice dateRange)))
    |> Seq.cache

tempData |> Seq.iter (fun (dateRange, (Stanice stanice), averageTemp) -> printfn "%s - %s: %15s: %f °C" (dateRange.FromDate.ToString(czechCultureInfo())) (dateRange.ToDate.ToString(czechCultureInfo())) stanice averageTemp)
