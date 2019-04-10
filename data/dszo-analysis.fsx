#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"

open FSharp.Data

[<Literal>]
let dataFile = __SOURCE_DIRECTORY__ + @"\Assets\dszo-output-sample.csv"
type Vehicles = CsvProvider<dataFile, Separators=";">

let vehicles = Vehicles.Load(@"C:\temp\vehicles2.csv")

vehicles.Rows
|> Seq.filter (fun v -> v.Station="Pančava")
|> Seq.map (fun v -> sprintf "  new google.maps.LatLng(%f, %f)," v.Latitude v.Longitude )
|> Seq.iter (printfn "%s")