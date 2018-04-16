#r @"C:\git\sandbox\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

open FSharp.Data
open System
open System.Text.RegularExpressions

[<Measure>] type degC

let (|TemperatureInCelsius|_|) str =
   let m = Regex.Match(str,"(.*)°C") 
   if (m.Success) then 
        (m.Groups.[1].Value |> float) * 1.0<degC> |> Some
   else 
        None  

type TemperatureWithTs = {dateTime:DateTime ; temperature:float<degC>}

type InPocasi = HtmlProvider<"http://www.in-pocasi.cz/meteostanice/stanice-historie.php?stanice=zlin&historie=11-29-2017">

let rows = InPocasi.Load("http://www.in-pocasi.cz/meteostanice/stanice-historie.php?stanice=zlin&historie=04-10-2018").Tables.Table1.Rows

rows
|> Seq.map (fun row -> row.Teplota)
|> Seq.map(function
    | TemperatureInCelsius temp -> Some temp
    | _ -> None)
|> Seq.choose(id)
|> Seq.average
|> printfn "Průměrná teplota: %A"