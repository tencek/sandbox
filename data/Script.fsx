// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
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

let (|Regex|_|) pattern input =
    let m = System.Text.RegularExpressions.Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None

let page = 
    Http.RequestString (@"https://www.proglas.cz/program/audioarchiv/?fulltext=Don%20Bosco&cycle=17&date_from=&date_to=&page=1#Content", responseEncodingOverride="utf-8")

let lines = 
    let page = page.Replace("&nbsp;", " ")
    page.Split('\n')
    |> Seq.choose (fun line ->
        line
        |> function
        | Regex """<a href="/program/detail-poradu/\?id=([0-9]+)" class="audioarchive-label" title=".*">.*- *(.*) -[^0-9]*([0-9]+)\..*</a>""" [id ; title ; chapter ] -> Some (title.Trim(), int chapter, sprintf @"https://audioarchiv.proglas.cz/mp3/015/audio_%d.mp3" (int id))
        | _ -> None)
    |> Seq.sortBy (fun (_title,chapter,_url) -> chapter)
    |> Seq.map (fun (title,chapter,url) ->
        (title,chapter,Http.AsyncRequestStream(url)))
    |> Seq.iter (fun (title,chapter,response) -> 
        let response = Async.RunSynchronously response
        let fileName = sprintf @"C:\temp\Don-Bosco-%02d-%s.mp3" chapter title
        printfn "%s" fileName
        let outFileStream = IO.File.Create(fileName)
        response.ResponseStream.CopyTo(outFileStream))


let ListChapters = 
    async {
        let! indexPages = 
            seq {1..4}
            |> Seq.map (fun i ->
                sprintf @"https://www.proglas.cz/program/audioarchiv/?fulltext=Don%%20Bosco&cycle=17&date_from=&date_to=&page=%d#Content" i)
            |> Seq.map (fun url->
                Http.AsyncRequestString(url, responseEncodingOverride="utf-8"))
            |> Async.Parallel
        return 
            indexPages
            |> Seq.collect (fun indexPage ->
                let indexPage = indexPage.Replace("&nbsp;", " ")
                indexPage.Split('\n')
                |> Seq.choose (
                    function
                    | Regex """<a href="/program/detail-poradu/\?id=([0-9]+)" class="audioarchive-label" title=".*">.*- *(.*) -[^0-9]*([0-9]+)\..*</a>""" [id ; title ; chapter ] -> Some (title.Trim(), int chapter, sprintf @"https://audioarchiv.proglas.cz/mp3/015/audio_%d.mp3" (int id))
                    | _ -> None))
        }

ListChapters |> Async.RunSynchronously |> Seq.iter (fun chapter -> printfn "%A" chapter)