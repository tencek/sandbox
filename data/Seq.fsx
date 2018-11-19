seq { 1 .. 4 }
|> Seq.allPairs <| seq { 1 .. 3 }
|> Seq.iter (printf "%A")

seq { 1 .. 9 }
|> Seq.append <| seq { 1 .. 3 }
|> Seq.iter (printf "%A")

seq { 1.0 .. 55.0 }
|> Seq.average

seq { 1 .. 7 }
|> Seq.averageBy double

let s = 
    seq { printf "loading.. " ; yield "non-cached" ; printf "loading.. " ; yield "non-cached" ; printfn "END!"}
let sc = 
    seq { printf "loading.. " ; yield "cached" ; printf "loading.. " ; yield "cached" ; printfn "END!"}
    |> Seq.cache
s |> Seq.iter (printf "%A")
s |> Seq.iter (printf "%A")
s |> Seq.iter (printf "%A")
sc |> Seq.iter (printf "%A")
sc |> Seq.iter (printf "%A")
sc |> Seq.iter (printf "%A")
printfn "!"

let list = new System.Collections.Generic.SortedList<int,string>()
list.Add(5,"5")
list.Add(3,"3")
list.Add(9,"9")
list
|> Seq.cast
|> Seq.iter (fun x -> printf "%O" x)


let doAfter s =
    printfn ":%A" s
    // return the result
    "555"

let result = Printf.ksprintf doAfter "%d" 1


















