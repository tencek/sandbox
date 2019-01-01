// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"
#r @"System.Xml.Linq"

open FSharp.Data
open System
open System.Text.RegularExpressions
open System.Globalization

let trace label x =
    printfn "%s: %A" label x
    x

let czechCultureInfo () =
    new CultureInfo("cs-CZ")

let parseDouble (culture:IFormatProvider) floatStr = 
    Double.Parse(floatStr, culture)

let parseCzechDouble = czechCultureInfo () |> parseDouble

[<Measure>] type km

let parseLength délka = 
    let (|LengthInKm|_|) str =
       let m = Regex.Match(str,"(.*)km") 
       if (m.Success) then 
            (parseCzechDouble m.Groups.[1].Value) |> Some
       else 
            None  
    let (|LengthInM|_|) str =
       let m = Regex.Match(str,"(.*)m") 
       if (m.Success) then 
            (parseCzechDouble m.Groups.[1].Value) |> Some
       else 
            None  

    let (|JustLength|_|) str =
       try 
          parseCzechDouble str |> Some
       with
          | _ex -> None
 
    match délka with
    | LengthInKm length -> length * 1.0<km>
    | LengthInM length -> length * 0.001<km>
    | JustLength length -> length * 1.0<km>
    | "" -> 0.0<km>
    | other -> failwithf "Incorrect délka value: %s" other 

type Year = int
type Provoz = Year option

let parseProvoz (vProvozu:string) = 
        let matches = Regex.Matches(vProvozu,".*([0-9]{4}).*")
        match matches.Count with
        | 0 -> None
        | _ ->
            matches
            |> Seq.cast<Match>
            |> Seq.map (fun mat -> mat.Groups.[1].Value |> int)
            |> Seq.max
            |> Some

type D1 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D1", Culture="cs-CZ">
let d1 = 
    D1().Tables.``Přehled úseků[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D1", row.úsek, row.délka, row.``uvedení do provozu``))
    |> Seq.map (trace "D1")

type D2 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D2", Culture="cs-CZ">
let d2 = 
    D2().Tables.``Historie výstavby[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D2", row.Úsek, row.Délka, row.Zprovoznění))
    |> Seq.map (trace "D2")

type D3 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D3", Culture="cs-CZ">
let d3 = 
    D3().Tables.``Přehled úseků[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D3", row.Úsek, row.Délka, row.``Uvedení do provozu``))
    |> Seq.map (trace "D3")


type D4 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D4", Culture="cs-CZ">
let d4 = 
    D4().Tables.``Úseky[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D4", row.Úsek, row.Délka, row.``Uvedení do provozu``))
    |> Seq.map (trace "D4")


type D5 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D5", Culture="cs-CZ">
let d5 = 
    D5().Tables.``Dílčí stavby[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D5", row.Úsek, row.``Délka (km)``.ToString(czechCultureInfo()), row.``V provozu``.ToString(czechCultureInfo())))
    |> Seq.map (trace "D5")

type D6 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D6", Culture="cs-CZ">
let d6 = 
    D6().Tables.``Přehled úseků[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D6", row.název, row.délka, row.``uvedení do provozu``))
    |> Seq.map (trace "D6")

type D7 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D7", Culture="cs-CZ">
let d7 = 
    D7().Tables.``Historie výstavby[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D7", row.Úsek, row.Délka, row.Zprovoznění))
    |> Seq.map (trace "D7")

type D8 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D8", Culture="cs-CZ">
let d8 = 
    D8().Tables.``Úseky dálnice[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D8", row.``název úseku``, row.``délka úseku``, row.``uvedení do provozu``.ToString(czechCultureInfo())))
    |> Seq.map (trace "D8")

type D10 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D10", Culture="cs-CZ">
let d10 = 
    D10().Tables.``Historie výstavby[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D10", row.Úsek, row.Délka, row.``Uvedení do provozu``))
    |> Seq.map (trace "D10")

type D11 = HtmlProvider<"https://cs.wikipedia.org/wiki/D%C3%A1lnice_D11", Culture="cs-CZ">
let d11 = 
    D11().Tables.``Přehled úseků[editovat | editovat zdroj]``.Rows
    |> Seq.map (fun row -> ("D11", row.Úsek, row.Délka, row.``Uvedení do provozu``))
    |> Seq.map (trace "D11")



[d1 ; d2 ; d3 ; d4 ; d5 ; d6 ; d7 ; d8 ; d10 ; d11 ] 
|> Seq.concat
|> Seq.map (fun (dx, cislo, delka, vProvozu) -> (dx, cislo, parseLength delka, parseProvoz vProvozu))
|> Seq.groupBy (fun (_dx, _usek, _delka, vProvozu) -> vProvozu)
|> Seq.map (fun (vProvozu, useky) -> (vProvozu, useky |> Seq.sumBy (fun (_dx, _usek, delka, _vprovozu) -> delka) ))
|> Seq.sortBy (fun (vProvozu, _sumDelekUseku) -> vProvozu)
|> Seq.iter (fun (vProvozu, sumDelekUseku) -> printfn "%A\t%A" vProvozu sumDelekUseku)
