// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"
#r @"..\packages\FSharp.Charting.2.1.0\lib\net45\FSharp.Charting.dll"

open FSharp.Data
open FSharp.Charting

[<Literal>]
let registraceDataFile = __SOURCE_DIRECTORY__ + @"\Assets\registrace-all-anonymized.csv"
type Registrace = CsvProvider<registraceDataFile, Encoding="utf-8">

let data = Registrace.GetSample().Rows |> Seq.cache

let myId = System.Guid("C9CA1545FBA1918CC36009B28EFB9D3B");
let me = data |> Seq.where (fun row -> row.Id = myId)

let people = 
   data
   |> Seq.map (fun row-> row.Id)
   |> Seq.distinct

data 
|> Seq.groupBy (fun row -> row.Id)
|> Seq.map (fun (id, data) -> (id, data |> Seq.maxBy (fun row -> row.Rok) |> (fun row -> row.Věk) ))
