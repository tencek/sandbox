#r @"..\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

open System.Web
open System.Text
open FSharp.Data
open System.Threading
open System.Text.RegularExpressions

[<Literal>]
let BaseUrl = "http://apl.czso.cz/irso4"

type Address = {Street:string ; Number:int}

type HtmlData = HtmlData of string

type Range = { Min:int ; Max:int }

let MakeAddress street number = { Street=street ; Number=number }

let UrlEncode text = 
    let enc = Encoding.GetEncoding(1250)
    HttpUtility.UrlEncode(text, enc)

let MakeUrl address =
    sprintf "%s/budlist.jsp?b=11&textobce=%s&textulice=%s&cisdom=%d" <| BaseUrl <| UrlEncode "Zlín" <| UrlEncode address.Street <| address.Number

let RequestString url = 
    printfn "%s" url
    Thread.Sleep 500
    Http.RequestString(url)

let GetDetailsUrl (HtmlData searchPageContent) = 
    let m = Regex.Match(searchPageContent, """<a title= "Detailní údaje o budově" href="(.*)">Detail...</a>""")
    if m.Success 
    then sprintf "%s/%s" <| BaseUrl <| m.Groups.[1].Value |> Some 
    else 
        printfn "DATA ERROR!"
        None

let GetNumberOfPeople (HtmlData detailsPageContent) =
    let m = Regex.Match(detailsPageContent, """<tr>
        <td>Počet evidovaných obyvatel v budově:</td>
        <td>.*</td>
        <td class="right">.*</td>
        <td>(\d+) ?- ?(\d+)</td>
      </tr>""")
    if m.Success 
    then {Min=int m.Groups.[1].Value; Max=int m.Groups.[2].Value} |> Some 
    else
        let m = Regex.Match(detailsPageContent, """<tr>
            <td>Počet evidovaných obyvatel v budově:</td>
            <td>.*</td>
            <td class="right">.*</td>
            <td>(\d+)</td>
          </tr>""")
        if m.Success 
        then {Min=int m.Groups.[1].Value; Max=int m.Groups.[1].Value} |> Some 
        else
            let m = Regex.Match(detailsPageContent, """<tr>
        <td>Počet obyvatel v budově dle SLDB - trvalý pobyt:</td>
        <td>.*</td>
        <td class="right">.*</td>
        <td>(\d+)</td>
      </tr>
      <tr>
        <td>Počet obyvatel v budově dle SLDB - obvyklý pobyt:</td>
        <td>.*</td>
        <td class="right">.*</td>
        <td>(\d+)</td>
      </tr>""")
            if m.Success
            then 
                let a = int m.Groups.[1].Value
                let b = int m.Groups.[2].Value
                {Min=min a b; Max=max a b} |> Some
            else
                printfn "DATA ERROR 2!"
                None

let GetNumberOfFlats (HtmlData detailsPageContent) =
    let m = Regex.Match(detailsPageContent, """<tr>
        <td>Počet bytů v budově \(vchodu\):</td>
        <td>(\d+)</td>""")
    if m.Success 
    then {Min=int m.Groups.[1].Value; Max=int m.Groups.[1].Value} |> Some 
    else
        printfn "DATA ERROR 3!"
        None

[<Literal>]
let dataFile = __SOURCE_DIRECTORY__ + @"\Assets\adresy-okoli-400m.csv"
type Adresses = CsvProvider<dataFile, Separators=";">


//let addresses = 
//    (new Adresses()).Rows
//    |> Seq.map (fun row -> (row.``Název ulice``, row.``Číslo domovní``))

let addresses = 
    [
        ("Křiby",4711)
        //("Křiby",4712) 
    ]

let htmlData = 
    addresses
    |> Seq.map (fun pair -> MakeAddress <| fst pair <| snd pair )
    |> Seq.map (fun address -> 
        address
        |> MakeUrl
        |> RequestString
        |> HtmlData
        |> GetDetailsUrl
        |> Option.map (fun url -> url.Replace("&amp;","&") |> RequestString |> HtmlData )
        |> Option.map (fun html -> (address, html)))
    |> Seq.choose id
    |> Seq.cache

let numbers = 
    htmlData
    |> Seq.map (fun (address, html) ->
        (address, (GetNumberOfPeople html), (GetNumberOfFlats html))
    )

let people = 
    htmlData
    |> Seq.map (snd >> GetNumberOfPeople)
    |> Seq.choose id

let minpeople = people |> Seq.sumBy (fun range -> range.Min)
let maxpeople = people |> Seq.sumBy (fun range -> range.Max)

let flats = 
    htmlData
    |> Seq.map (snd >> GetNumberOfFlats)
    |> Seq.choose id

let minflats = flats |> Seq.sumBy (fun range -> range.Min)
let maxflats = flats |> Seq.sumBy (fun range -> range.Max)
