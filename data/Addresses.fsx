#r @"..\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

open System.Web
open System.Text
open FSharp.Data
open System.Threading
open System.Text.RegularExpressions

[<Literal>]
let BaseUrl = "http://apl.czso.cz/irso4"

type Address = {Street:string ; Number:int}

let MakeAddress street number =
  { Street=street
    Number=number }

let UrlEncode text = 
    let enc = Encoding.GetEncoding(1250)
    HttpUtility.UrlEncode(text, enc)

let MakeUrl address =
    sprintf "%s/budlist.jsp?b=11&textobce=%s&textulice=%s&cisdom=%d" <| BaseUrl <| UrlEncode "Zlín" <| UrlEncode address.Street <| address.Number

let RequestString url = 
    printfn "%s" url
    Thread.Sleep 500
    Http.RequestString(url)

let GetDetailsUrl searchPageContent = 
    let m = Regex.Match(searchPageContent, """<a title= "Detailní údaje o budově" href="(.*)">Detail...</a>""")
    if m.Success 
    then sprintf "%s/%s" <| BaseUrl <| m.Groups.[1].Value |> Some 
    else 
        printfn "DATA ERROR!"
        None

let GetNumberOfPeople detailsPageContent =
    let m = Regex.Match(detailsPageContent, """<tr>
        <td>Počet evidovaných obyvatel v budově:</td>
        <td>.*</td>
        <td class="right">.*</td>
        <td>(\d+) ?- ?(\d+)</td>
      </tr>""")
    if m.Success 
    then (int m.Groups.[1].Value, int m.Groups.[2].Value) |> Some 
    else
        let m = Regex.Match(detailsPageContent, """<tr>
            <td>Počet evidovaných obyvatel v budově:</td>
            <td>.*</td>
            <td class="right">.*</td>
            <td>(\d+)</td>
          </tr>""")
        if m.Success 
        then (int m.Groups.[1].Value, int m.Groups.[1].Value) |> Some 
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
                (min a b, max a b) |> Some
            else
                printfn "DATA ERROR 2!"
                None

let GetNumberOfFlats detailsPageContent =
    let m = Regex.Match(detailsPageContent, """<tr>
        <td>Počet bytů v budově \(vchodu\):</td>
        <td>(\d+)</td>""")
    if m.Success 
    then (int m.Groups.[1].Value, int m.Groups.[1].Value) |> Some 
    else
        printfn "DATA ERROR 3!"
        None

[<Literal>]
let dataFile = __SOURCE_DIRECTORY__ + @"\Assets\adresy-okoli-400m.csv"
type Data = CsvProvider<dataFile, Separators=";">


let data = 
    (new Data()).Rows
    |> Seq.map (fun row -> (row.``Název ulice``, row.``Číslo domovní``))

//let data = 
//    [
//        ("Křiby",4711)
//        //("Křiby",4712) 
//    ]

let htmlData = 
    data
    |> Seq.map (fun pair -> MakeAddress <| fst pair <| snd pair )
    |> Seq.map MakeUrl
    |> Seq.map RequestString
    |> Seq.map GetDetailsUrl
    |> Seq.choose id
    |> Seq.map (fun url -> url.Replace("&amp;","&"))
    |> Seq.map (fun url -> (url, RequestString url))
    |> Seq.cache

let people = 
    htmlData
    |> Seq.map (snd >> GetNumberOfPeople)
    |> Seq.choose id

let minpeople = people |> Seq.sumBy (fun minmax -> fst minmax)
let maxpeople = people |> Seq.sumBy (fun minmax -> snd minmax)

let flats = 
    htmlData
    |> Seq.map (snd >> GetNumberOfFlats)
    |> Seq.choose id

let minflats = flats |> Seq.sumBy (fun minmax -> fst minmax)
let maxflats = flats |> Seq.sumBy (fun minmax -> snd minmax)


Http.RequestString("http://apl.czso.cz/irso4/buddet.jsp?budid=3321999&idob=1018991417&b=11&textobce=Zl%EDn&textulice=K%F8iby&cisdom=4711")