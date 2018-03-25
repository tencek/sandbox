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

type Zastavky =  HtmlProvider<"http://www.dszo.cz/komunikace/?page=zastavky">

Zastavky().Lists.``InformaÄnÃ­ portÃ¡l``.Html |> printfn "%A"
    
Zastavky().Html |> printfn "%A"

type DSZO = JsonProvider<"""{"data": [["407","2","01:05","ZahradnickĂˇ","BaĹĄova nemocnice","3/2","2125"],["408","2","02:56","Otrokovice, Ĺľel.st.","BaĹĄova nemocnice","2/2","1182"]]}""">

let _buses = DSZO.Load("http://www.dszo.cz/online/tabs2.php").Data

///////////////////////////////////////////////

printfn "================================="

type Linka11 = HtmlProvider<"http://www.dszo.cz/?section=jr&file=jr&linkaid=11">
Linka11().Tables.Table12.Rows |> Seq.iter ( fun row -> row.ToValueTuple() |> printfn "| ROW: %A" )

type Linka11Gemini = HtmlProvider<"http://www.dszo.cz/jr/workhtml.php?htmlfile=MjAxNy0xMC0xMS8xMWFfMDkuaHRtbA==&version=0">
Linka11Gemini().Tables.Table5.Rows |> Seq.iter ( fun row -> row |> printfn "| ROW: %A" )
Linka11Gemini().Tables.Table6.Rows |> Seq.iter ( fun row -> row |> printfn "| ROW: %A" )


////////////////////////////

type Calendar = JsonProvider<"https://ical-to-json.herokuapp.com/convert.json?url=https%3A%2F%2Fcalendar.google.com%2Fcalendar%2Fical%2Fzlin6.cz_822fssjj8hlr2khh6tt6v5d1qc%2540group.calendar.google.com%2Fpublic%2Fbasic.ics">
let x = 
    Calendar.Load("https://ical-to-json.herokuapp.com/convert.json?url=https%3A%2F%2Fcalendar.google.com%2Fcalendar%2Fical%2Fzlin6.cz_822fssjj8hlr2khh6tt6v5d1qc%2540group.calendar.google.com%2Fpublic%2Fbasic.ics").Vcalendar.[0].Vevent
    |> Seq.choose (fun event -> event.Rrule)
    |> Seq.iter (fun rule -> printfn "%A" rule)