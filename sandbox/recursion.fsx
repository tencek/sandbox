#time "on"

let rec movingAverages list = 
    match list with
    // if input is empty, return an empty list
    | [] -> []
    // for one item, return an empty list
    | [_] -> []
    // otherwise process pairs of items from the input 
    | x::y::rest -> 
        let avg = ( x + y )
        //build the result by recursing the rest of the list
        avg::movingAverages (y::rest)

[1.0 .. 10000.0] |> movingAverages



let rec movingAveragesTail list averagesSoFar = 
    match list with
    // if input is empty, return an empty list
    | [] -> averagesSoFar |> List.rev
    // for one item, return an empty list
    | [_] -> averagesSoFar |> List.rev
    // otherwise process pairs of items from the input 
    | x::y::rest -> 
        let avg = ( x + y ) / 2.0 
        //build the result by recursing the rest of the list
        movingAveragesTail (y::rest) (avg::averagesSoFar) 

[1.0 .. 500000000.0] |> movingAveragesTail []
