module gettemp

open CommandLine
open System.Net.Http
open System.Text.RegularExpressions


type Options = {
  [<Option('l', "list-all", Required = false)>] listAll : bool;
}

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

let run (options:Options) =
    if options.listAll then
        listAll ()
    else
        0

let fail errors = 
    printfn "Failed: %A" errors
    -1

[<EntryPoint>]
let main argv =
    let cmdParseResult = CommandLine.Parser.Default.ParseArguments<Options>(argv)
    let retVal = 
        match cmdParseResult with
        | :? Parsed<Options> as parsed -> run parsed.Value
        | :? NotParsed<Options> as notParsed -> fail notParsed.Errors
        | _ -> -1
    retVal