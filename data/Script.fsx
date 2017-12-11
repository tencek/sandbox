// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"C:\git\sandbox\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

open FSharp.Data
open System.Text.RegularExpressions
open System

[<Measure>] type degC

let (|TemperatureInCelsius|_|) str =
   let m = Regex.Match(str,"(.*)°C") 
   if (m.Success) then 
        (m.Groups.[1].Value |> float) * 1.0<degC> |> Some
   else 
        None  

let makeUrl stanice (date:DateTime) = 
    sprintf "http://www.in-pocasi.cz/meteostanice/stanice-historie.php?stanice=%s&historie=%s" stanice (date.ToString("MM-dd-yyyy"))

type InPocasi = 
  HtmlProvider<"http://www.in-pocasi.cz/meteostanice/stanice-historie.php?stanice=zlin&historie=11-29-2017">

InPocasi().Tables.Table1.Rows
|> Seq.map (fun row -> row.Teplota)
|> Seq.map(function
    | TemperatureInCelsius temp -> Some temp
    | _ -> None)
|> Seq.choose(id)
|> Seq.average
|> printfn "Průměrná teplota: %A"

let parseDate dateStr =
    DateTime.Parse(dateStr, )

let fromDate  = parseDate "7.1.2017 10:40:00"
let toDate = parseDate "9.1.2017 22:35:00"

let dateList (fromDate:DateTime) (toDate:DateTime) = 
    toDate.Date - fromDate.Date
    //seq { for i in fromDate .. toDate -> i }
    //seq { yield fromDate.Date ; yield toDate.Date }

printfn "%A" (toDate.Date - fromDate.Date).Days
printfn "%A" toDate

dateList fromDate toDate |> printfn "%A"

type Zastavky =  HtmlProvider<"http://www.dszo.cz/komunikace/?page=zastavky">

Zastavky().Lists.``InformaÄnÃ­ portÃ¡l``.Html |> printfn "%A"
    
Zastavky().Html |> printfn "%A"

type DSZO = JsonProvider<"""{"data": [["407","2","01:05","ZahradnickĂˇ","BaĹĄova nemocnice","3/2","2125"],["408","2","02:56","Otrokovice, Ĺľel.st.","BaĹĄova nemocnice","2/2","1182"]]}""">

let _buses = DSZO.Load("http://www.dszo.cz/online/tabs2.php").Data
