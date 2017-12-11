
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