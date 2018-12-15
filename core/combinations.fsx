let rec createCombinations aList = 
    match aList with
    | [ elm ] -> [ [ elm ] ]
    | aList ->
        [ 0..(List.length aList - 1) ]
        |> List.collect (fun index -> 
            let (picked, rest) = 
                aList 
                |> List.indexed
                |> List.partition ( fun (idx, _elm) -> idx = index )
            let pickedVal = 
                picked
                |> List.exactlyOne
                |> snd
            rest 
            |> List.map snd 
            |> createCombinations
            |> List.map ( fun list -> pickedVal::list )
        )

let input = [ 1 ; 1 ; 1 ; 2 ; 2 ; 2 ; 3 ; 3 ; 4 ; 4 ]
let combinations = createCombinations input |> List.distinct |> List.length























