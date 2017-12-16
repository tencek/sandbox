open System.IO

type Direction = N | NE | SE | S | SW | NW

let (|Direction|_|) str = 
    match str with
    | "n" -> Some N
    | "ne" -> Some NE
    | "se" -> Some SE
    | "s" -> Some S
    | "sw" -> Some SW
    | "nw" -> Some NW
    | _ -> None

let toFullPath fileName = 
    __SOURCE_DIRECTORY__ + @"\" + fileName

let readLines (filePath:string) = seq {
    use streamReader = new StreamReader(filePath)
    while not streamReader.EndOfStream do
        yield streamReader.ReadLine ()
    }

let toSteps (str:string) = 
    str.Split(',')

let toDirection str = 
    match str with
    | Direction direction -> Some direction
    | _ -> None

let trace label x =
    printfn "| %s: %A" label x
    x

let stepCounts =
    "steps.txt"
    |> toFullPath
    |> readLines
    |> trace "lines"
    |> Seq.collect toSteps
    |> Seq.map toDirection
    |> Seq.choose id
    |> trace "steps"
    |> Seq.countBy id
    |> Map.ofSeq
    |> trace "counts"

let Nsteps = stepCounts.Item N - stepCounts.Item S
let NESteps = stepCounts.Item NE - stepCounts.Item SW
let NWSteps = stepCounts.Item NW - stepCounts.Item SE