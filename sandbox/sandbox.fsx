
open System

let x = 123 
let y = Random x
let _z = y.Next ()

(Random 123).Next () |> printfn "%d"

(*--------------------------------------*)

type ColorEnum = Green=0 | Blue=1 // enum
let blueEnum = ColorEnum.Blue
match blueEnum with
| Blue -> printfn "blue" // warning FS0049
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
    //list |> Seq.fold (fun sum kvp -> sum + kvp.Value) 0

let GetListValue (list:SortedList<int,int>) pos =
    1

let list = new SortedList<int,int>()
[1..10] |> Seq.iter (fun i -> [1..10] |> Seq.iter (fun j -> AddToList list j))
list.Count
CountListCounts list
