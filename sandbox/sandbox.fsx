
open System

module QRGen =

   type ErrorCorrectionLevel =
      | Low07
      | Medium15
      | Quartile25
      | High30

   type EncodingMode =
      | Numeric
      | Alphanumeric
      | Byte
      | Kanji
      | ECI

   type Size = 
      | Px21
      | Px25
      | Px29
      | Px33
      | Px37
      | Px41
      | Px45
      | Px49
      | Px53
      | Px57
      | Px61
      | Px65
      | Px69
      | Px73
      | Px77
      | Px81
      | Px85
      | Px89
      | Px93
      | Px97
      | Px101
      | Px105
      | Px109
      | Px113
      | Px117
      | Px121
      | Px125
      | Px129
      | Px133
      | Px137
      | Px141
      | Px145
      | Px149
      | Px153
      | Px157
      | Px161
      | Px165
      | Px169
      | Px173
      | Px177

   type MaskPattern = 
      | Mask0
      | Mask1
      | Mask2
      | Mask3
      | Mask4
      | Mask5
      | Mask6
      | Mask7

   type ErrorCorrectionLevelOption =
      | ErrorCorrectionLevel of ErrorCorrectionLevel
      | HighestPossible
   
   type EncodingModeOption =
      | EncodingMode of EncodingMode
      | EncodingByContent

   type SizeOption =
      | Size of Size
      | SmallestPossible

   type MaskPatternOption = 
      | MaskPattern of MaskPattern
      | MaskByMinimumPenalty

   type Options = 
      {
         ErrorCorrectionLevelOption : ErrorCorrectionLevelOption
         EncodingModeOption : EncodingModeOption
         SizeOption : SizeOption
         MaskPatternOption : MaskPatternOption
      }

   type GenerationStrategy = 
      | MinimumSize of (ErrorCorrectionLevel * EncodingModeOption * MaskPatternOption)
      | MaximumErrorCorrectioLevel of (Size * EncodingModeOption * MaskPatternOption)

   type Error = string

   let DefaultStrategy = MinimumSize (ErrorCorrectionLevel.Quartile25 , EncodingByContent , MaskByMinimumPenalty)

   let DefaultOptions = 
      { 
         ErrorCorrectionLevelOption = ErrorCorrectionLevel Quartile25
         EncodingModeOption = EncodingByContent
         SizeOption = SmallestPossible
         MaskPatternOption = MaskByMinimumPenalty
      }

   type IGenerate = GenerationStrategy * string -> Result<bool,Error>



open QRGen

let defaultOptions = QRGen.DefaultOptions

let myOptions = 
   { 
      ErrorCorrectionLevelOption = ErrorCorrectionLevel High30
      EncodingModeOption = EncodingMode Alphanumeric
      SizeOption = Size Px101
      MaskPatternOption = MaskPattern Mask7
   }

let x = 123 
let y = Random x
let _z = y.Next ()

(Random 123).Next () |> printfn "%d"

(*--------------------------------------*)

type ColorEnum = Green=0 | Blue=1 // enum
let blueEnum = ColorEnum.Blue
match blueEnum with
| ColorEnum.Blue -> printfn "blue" // warning FS0049
| _ -> printfn "something else"

System.Console.WriteLine("{0}",12345)

///////////////////////////////////////////////

type EmailAddress = EmailAddress of string

// using the constructor as a function

"a" |> EmailAddress

["a"; "b"; "c"] |> List.map EmailAddress

// inline deconstruction

let a' = "a" |> EmailAddress

let (EmailAddress _x) = a'

let _xy = seq { 0 .. 10 .. 100}

let addresses =
    ["a"; "b"; "c"]
    |> List.map EmailAddress

let addresses' =
    addresses
    |> List.map (fun (EmailAddress e) -> e)

////////////////////////////////////////////////////////////

let subtractUnsigned (x : uint32) (y : uint32) =
    assert (x > y)
    let z = x - y
    z
// This code does not generate an assertion failure.
let result1 = subtractUnsigned 2u 1u
// This code generates an assertion failure.
let result2 = subtractUnsigned 1u 2u

let add x = ( fun y -> x + y)

printfn "%A" add

Some 3 |> Option.bind (fun x -> sprintf "%d%d" x x |> Some)


[ 1; 2; 3] |> List.map2 (fun x y -> x + y) [ 3 ; 2 ; 1] |> printfn "%A"

///////////////////////////////////////////////

let rec sumList list = 
    match list with
    | [] -> 0
    | head::tail -> head + sumList tail

let rec sumList2 sumSoFar list = 
    match list with
    | [] -> sumSoFar
    | head::tail -> sumList2 (sumSoFar + head) tail

[1m .. 1000000m] |> sumList2 0m

//////////////////////////////////////////////

type State = New|Draft|Published|Inactive|Discontinued

let StateToInt state =
    match state with
    | New -> 1
    | Draft -> 2
    | Inactive -> 3
    | Discontinued -> 4

let out state = 
    printfn "state = %O = %d" state (StateToInt state)

out Inactive

let hwn n = 
    [1..n] |> Seq.iter (fun _x -> printfn "Hello World")

hwn 5



#time "on"
open System.Collections
open System.Collections.Generic
open System.Linq

let AddToList (list:SortedList<int,int>) key = 
    if 
        list.ContainsKey(key)
    then
        list.[key] <- list.[key] + 1
    else
        list.Add(key,1)

let CountListCounts (list:SortedList<int,int>) = 
    list |> Seq.fold (fun sum kvp -> sum + kvp.Value) 0

let GetListValue (list:SortedList<int,int>) pos =
    1

let list = new SortedList<int,int>()
[1..10] |> Seq.iter (fun i -> [1..10] |> Seq.iter (fun j -> AddToList list j))
list.Count
CountListCounts list

type Node = { Label:string ; Children:seq<Node> } 

let getTree n = 
    let rec getChildren n =
        seq { 1 .. (n-1) }
        |> Seq.map (fun n -> { Label=sprintf "%d" n ; Children = getChildren n })
    { Label=sprintf "%d" n ; Children = getChildren n}

let printGraph root = 
    let rec printNode parent node = 
        printfn "%s [label=%s]" node.Label node.Label
        printfn "%s->%s" parent.Label node.Label
        node.Children |> Seq.iter (printNode node)
    printfn "%s [label=%s]" root.Label root.Label
    root.Children |> Seq.iter (printNode root)

getTree 5 |> printGraph

#time "off"

let a = new DateTime(2020, 10, 25, 1, 30, 0, DateTimeKind.Local)
let b = new DateTime(2020, 10, 25, 2, 00, 0, DateTimeKind.Local)
let c = new DateTime(2020, 10, 25, 2, 30, 0, DateTimeKind.Local)
let d = new DateTime(2020, 10, 25, 3, 00, 0, DateTimeKind.Local)
let e = new DateTime(2020, 10, 25, 3, 30, 0, DateTimeKind.Local)

c.Subtract(a)
c.ToUniversalTime().Subtract(a.ToUniversalTime())

a.IsDaylightSavingTime () |> sprintf "%A"
b.IsDaylightSavingTime () |> sprintf "%A"
c.IsDaylightSavingTime () |> sprintf "%A"
d.IsDaylightSavingTime () |> sprintf "%A"
e.IsDaylightSavingTime () |> sprintf "%A"

