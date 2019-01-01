﻿// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"
#r @"System.Xml.Linq"

open FSharp.Data
open System

let trace label x =
    printfn "%s: %A" label x
    x

type Zastavky =  HtmlProvider<"http://www.dszo.cz/komunikace/?page=zastavky">

Zastavky().Lists.``Informační portál``.Html |> printfn "%A"
    
Zastavky().Html |> printfn "%A"

type DSZO = JsonProvider<"""{"data": [["407","2","01:05","ZahradnickĂˇ","BaĹĄova nemocnice","3/2","2125"],["408","2","02:56","Otrokovice, Ĺľel.st.","BaĹĄova nemocnice","2/2","1182"]]}""">

let _buses = DSZO.Load("http://www.dszo.cz/online/tabs2.php").Data

///////////////////////////////////////////////

printfn "================================="

type Linka11 = HtmlProvider<"https://www.dszo.cz/jizdni-rady/?linka=11&typ=trolejbus#jr-bank1">
Linka11().Tables.``Aktuální jízdní řád``.Rows |> Seq.iter ( fun row -> row.ToValueTuple() |> printfn "| ROW: %A" )

type Linka11Gemini = HtmlProvider<"https://www.dszo.cz/jr/workhtml.php?htmlfile=L2JhbmsxL3Ryb2xlamJ1c3kvMTFhXzA5Lmh0bWw=&grafikon=eCxz">
Linka11Gemini().Tables.Table5.Rows |> Seq.iter ( fun row -> row |> printfn "| ROW: %A" )
Linka11Gemini().Tables.Table6.Rows |> Seq.iter ( fun row -> row |> printfn "| ROW: %A" )


////////////////////////////

type Calendar = JsonProvider<"https://ical-to-json.herokuapp.com/convert.json?url=https%3A%2F%2Fcalendar.google.com%2Fcalendar%2Fical%2Fzlin6.cz_822fssjj8hlr2khh6tt6v5d1qc%2540group.calendar.google.com%2Fpublic%2Fbasic.ics">
let x = 
    Calendar.Load("https://ical-to-json.herokuapp.com/convert.json?url=https%3A%2F%2Fcalendar.google.com%2Fcalendar%2Fical%2Fzlin6.cz_822fssjj8hlr2khh6tt6v5d1qc%2540group.calendar.google.com%2Fpublic%2Fbasic.ics").Vcalendar.[0].Vevent
    |> Seq.choose (fun event -> event.Rrule)
    |> Seq.iter (fun rule -> printfn "%A" rule)

/////////////////////////////

[<Literal>]
let HristeCelek = __SOURCE_DIRECTORY__ + @"\Assets\hriste_celek.xml"
type Hriste = XmlProvider<HristeCelek, Encoding="UTF-8">
let hriste = Hriste.Load(@"https://www.zlin.eu/data/dataupload/omz/hriste/google_maps/xml/hriste_celek.xml")

hriste.Rows
|> Seq.map (fun hriste -> hriste.TypHriste)
|> Seq.distinct
|> Seq.sort
|> Seq.iter (printfn "%s")

hriste.Rows
|> Seq.countBy (fun hriste -> hriste.TypHriste)
