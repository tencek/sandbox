// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"C:\git\sandbox\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

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
    System.Threading.Thread.Sleep(1000);
    InPocasi.Load(url).Tables.Table1.Rows
    |> Seq.map (fun row -> (row.Čas, row.Teplota))
    |> Seq.choose tupleToTempWithTs

let czechCultureInfo() =
    new CultureInfo("cs-CZ")

let parseDate culture dateStr =
    DateTime.Parse(dateStr, culture)

let parseDate' = czechCultureInfo() |> parseDate 

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

let dates = [
    //"29.12.2016 21:13:00";
    //"7.1.2017 10:40:00  ";
    //"9.1.2017 22:35:00  ";
    //"22.1.2017 19:41:00 ";
    //"4.2.2017 16:38     ";
    //"19.2.2017 22:54:00 ";
    //"5.3.2017 0:11:00   ";
    //"2.4.2017 12:00:00  ";
    //"4.5.2017 0:25:00   ";
    "10.5.2017 20:43:00 ";
    "15.5.2017 22:20:00 ";
    //"1.7.2017 8:40:00   ";
    //"6.7.2017 0:55:00   ";
    //"11.7.2017 13:00:00 ";
    //"16.7.2017 22:15:00 ";
    //"24.7.2017 21:36:00 ";
    //"30.7.2017 23:55:00 ";
    //"18.9.2017 22:38:00 ";
    //"2.10.2017 23:48:00 ";
    //"8.10.2017 22:50:00 ";
    //"22.10.2017 21:00:00";
    //"21.11.2017 12:47:00";
    //"26.11.2017 23:00:00";
    //"3.12.2017 20:04:00 ";
    //"9.12.2017 21:07:00 ";
    //"16.12.2017 22:56:00";
    //"24.12.2017 1:20:00 ";
    //"30.12.2017 21:25:00";
]

dates
|> Seq.ofList
|> Seq.map parseDate'
|> Seq.pairwise
|> Seq.map (fun (fromDate, toDate) -> (fromDate, toDate, getAverageTemperature "zlin_centrum" fromDate toDate))
|> Seq.iter (fun (fromDate, toDate, averageTemp) -> printfn "%s - %s: %f °C" (fromDate.ToString()) (toDate.ToString()) averageTemp)

