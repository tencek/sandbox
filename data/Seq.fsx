seq { 1 .. 4 }
|> Seq.allPairs <| seq { 1 .. 3 }
|> Seq.iter (printf "%A")

seq { 1 .. 9 }
|> Seq.append <| seq { 1 .. 3 }
|> Seq.iter (printf "%A")

seq { 1.0 .. 55.0 }
|> Seq.average
|> printfn "%A"

