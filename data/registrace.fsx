// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"

open FSharp.Data
open System

[<Literal>]
let RegistraceDataFile = __SOURCE_DIRECTORY__ + @"\Assets\registrace-all-anonymized.csv"
type Registrace = CsvProvider<RegistraceDataFile, Encoding="utf-8", Schema="Id=string">

type MemberStats = {
   id:string
   yearIn:int
   ageIn:int
   yearOut: int option 
   ageOut:int option
   regs:seq<Registrace.Row>
}

let data = Registrace.GetSample().Rows |> Seq.cache

let dataOfInterest = data |> Seq.filter (fun row -> row.``Typ členství`` = "Řádné")

let dataMaxYear = 
   dataOfInterest 
   |> Seq.map (fun row -> row.Registrace) 
   |> Seq.max

let dataById = dataOfInterest |> Seq.groupBy (fun row -> row.Id)

let allMembers = 
   dataById
   |> Seq.map (fun (id, rows) -> 
      let (yearIn, ageIn) = rows |> Seq.minBy (fun row -> row.Registrace) |> (fun row -> (row.Registrace, row.Věk))
      let (yearOut, ageOut) = rows |> Seq.maxBy (fun row -> row.Registrace) |> (fun row -> (row.Registrace, row.Věk))
      if yearOut = dataMaxYear then
         {id=id ; yearIn=yearIn ; ageIn=ageIn ; yearOut=None ; ageOut=None ; regs = rows }
      else
         {id=id ; yearIn=yearIn ; ageIn=ageIn ; yearOut=Some yearOut ; ageOut=Some ageOut ; regs = rows }
   )

let membersAlive = 
   allMembers
   |> Seq.where (fun stat -> stat.yearOut.IsNone)

let membersAlreadyLeft = 
   allMembers
   |> Seq.where (fun stat -> stat.yearOut.IsSome)


allMembers |> Seq.length
membersAlive |> Seq.length
membersAlreadyLeft |> Seq.length

membersAlreadyLeft 
|> Seq.where (fun mem -> (mem.yearOut = Some 2019) && (mem.ageOut < Some 18))
|> Seq.sortBy (fun mem -> mem.ageOut)
|> Seq.iter (fun mem -> printfn "%A" mem)

let myId = @"C9CA1545FBA1918CC36009B28EFB9D3B";
let me = dataById |> Seq.where (fun (id, data) -> id = myId)

