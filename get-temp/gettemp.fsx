#r "netstandard"
#r "System.Net.Http"
#r @"C:\Users\Manžel\.nuget\packages\fsharp.data\3.0.0\lib\netstandard2.0\FSharp.Data.dll"

open System.Net.Http
open System.Text.RegularExpressions
open FSharp.Data
open System

let getAllAsync () = async {
        use client = new HttpClient()
        let! html = 
            client.GetStringAsync("https://www.in-pocasi.cz/meteostanice/")
            |> Async.AwaitTask

        let regex = new Regex(Regex.Escape("""href="stanice.php?stanice=""")+"""([^"]+)""")
        return
            regex.Matches(html)
            |> Seq.cast<Match>
            |> Seq.map (fun mat -> mat.Groups.[1].Value)
            |> Seq.sort
            |> Seq.distinct
}

let listAll () =
    getAllAsync ()
    |> Async.RunSynchronously
    |> Seq.iter (printfn "%s")
    0

type InPocasiNow = FSharp.Data HtmlProvider<"https://www.in-pocasi.cz/meteostanice/stanice.php?stanice=zlin_centrum", Encoding="utf-8">

//InPocasiNow().Tables.Table1.Rows.[0]
//|> (fun row -> (row.``Äas``, row.Teplota))
//|> (fun (time, temp) -> printfn "Teplota v %s byla %s." (time.ToString()) temp)


listAll ()
