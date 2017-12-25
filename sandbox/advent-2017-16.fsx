#load "advent.fsx"
open Advent
open System.Text.RegularExpressions

type Program = char
type Count = int
type Position = int

type DanceMove = 
    | Spin of Count
    | Exchange of (Position * Position)
    | Partner of (Program * Program)

type ProgramList = Program list

let (|CountPattern|_|) str =
   match System.Int32.TryParse(str) with
   | (true,intValue) when (intValue >= 0 && intValue <= 15) -> Some intValue
   | _ -> None

let (|PositionPattern|_|) str =
   match System.Int32.TryParse(str) with
   | (true,intValue) when (intValue >= 0 && intValue <= 15) -> Some intValue
   | _ -> None

let (|Prefix|_|) (prefix:string) (str:string) =
    if str.StartsWith(prefix) then
        let rest = str.Substring(prefix.Length)
        Some(rest)
    else
        None

let (|Positions|_|) (str:string) = 
    let m = Regex.Match(str, @"([0-9]+)/([0-9]+)")
    if (m.Success) then 
        match (m.Groups.[1].Value, m.Groups.[2].Value) with
        | (PositionPattern p1, PositionPattern p2) -> Some (p1, p2)
        | _ -> None
    else
        None

let (|Programs|_|) (str:string) = 
    let m = Regex.Match(str, @"([a-p])/([a-p])")
    if (m.Success) then 
        (m.Groups.[1].Value.[0], m.Groups.[2].Value.[0]) |> Some
    else
        None

let (|SpinPattern|_|) str = 
    match str with 
    | Prefix "s" rest -> 
        match rest with 
        | CountPattern count -> Spin count |> Some
        | _ -> None
    |_ -> None

let (|ExchangePattern|_|) str = 
    match str with
    | Prefix "x" rest ->
        match rest with
        | Positions positions -> Exchange positions |> Some
        | _ -> None
    | _ -> None

let (|PartnerPattern|_|) str = 
    match str with
    | Prefix "p" rest ->
        match rest with
        | Programs programs -> Partner programs |> Some
        | _ -> None
    | _ -> None

let parseLine (line:string) = 
    line.Split(',')

let parseDanceMove str = 
    match str with
    | SpinPattern spin -> Some spin
    | ExchangePattern exchange -> Some exchange
    | PartnerPattern partner -> Some partner
    | _ -> failwithf "%s is not a dance move!" str

let strToProgramList str =
    str |> Seq.toList 

let programListToString (programList:Program list) = 
    System.String.Concat(programList)

let spin count programList = 
    let splitIndex = List.length programList - count
    let first, second = List.splitAt splitIndex programList
    List.append second first

// test
("abcde" |> strToProgramList |> spin 2 |> programListToString) = "deabc"

let exchange pos1 pos2 programList =
    programList 
    |> List.permute (fun i -> 
        match i with
        | i when i = pos1 -> pos2
        | i when i = pos2 -> pos1
        | _ -> i)

// test
("abcde" |> strToProgramList |> exchange 3 4 |> programListToString) = "abced"

let partner program1 program2 programList = 
    let pos1 = programList |> List.findIndex ((=) program1)
    let pos2 = programList |> List.findIndex ((=) program2) 
    exchange pos1 pos2 programList

// test
("abcde" |> strToProgramList |> partner 'a' 'e' |> programListToString) = "ebcda"

let processDanceMove move programList =
    match move with
    | Spin count -> 
        spin count programList
    | Exchange positions ->
        let pos1, pos2 = positions
        exchange pos1 pos2 programList
    | Partner programs -> 
        let program1, program2 = programs
        partner program1 program2 programList

let danceMoves = 
    "advent-2017-16-input.txt"
    |> toFullPath
    |> readLines
    |> trace "lines"
    |> Seq.map parseLine
    |> Seq.concat
    |> Seq.map parseDanceMove
    |> Seq.choose id

let doDance danceMoves danceMoveProcessor programList =
    danceMoves
    |> Seq.fold(fun programList move -> danceMoveProcessor move programList) programList

///////////////////////////////////////////////////////////////////////////////

let initialList = 
    "abcdefghijklmnop" 
    |> strToProgramList

let firstDance = 
    initialList
    |> doDance danceMoves processDanceMove

firstDance
|> programListToString
|> trace "1st dance"

///////////////////////////////////////////////////////////////////////////////

// too slow
let mapFunc listSrc listDst indexSrc =
    let itemSrc = listSrc |> List.item indexSrc
    listDst |> List.findIndex ((=) itemSrc)

// test
(initialList |> List.permute (mapFunc initialList firstDance)) = firstDance

let createMapping listSrc listDst =
    listSrc
    |> List.map(fun item -> 
        listDst |> List.findIndex ((=) item))

let permute (mapping:int list) programList = 
    programList
    |> List.permute ( fun i -> mapping.[i])


let mapping = createMapping initialList firstDance

// test
(initialList |> permute mapping) = firstDance


#time
seq {1..1000000000}
|> Seq.fold (fun programList iteration -> 
    if iteration % 1000000 = 0 then
        printfn "Iteration %d" iteration
    programList 
    |> permute mapping)
    initialList
|> programListToString
|> trace "1000000000th dance"
#time
