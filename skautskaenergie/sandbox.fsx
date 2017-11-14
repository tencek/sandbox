type Expr<'a> = 
    | True
    | False
    | And of Expr<'a> * Expr<'a>
    | Or of Expr<'a> * Expr<'a>
    | Not of Expr<'a>
    | Value of 'a


let rec eval eval_base expr = 
    let eval' x = eval eval_base x
    match expr with
    | True -> true
    | False -> false
    | And (a,b) -> eval' a && eval' b
    | Or (a,b) -> eval' a || eval' b
    | Not a -> not (eval' a)
    | Value x -> eval_base x

///////////////////////////////////////////////////////////

let eval_string (x:string) =
    match (x.ToLower()) with
    | x when x = "true" -> true
    | x when x = "1" -> true
    | x when x = "yes" -> true
    | _ -> false

let expr:Expr<string> = Not (Or (And (Value "true", Value "1"), And (Value "X", Value "yes")))

let result = eval eval_string expr

let prec = 1000000m;
let s1 = [0m..prec]
let pos = s1 |> List.map(fun i -> 1m / (1m + 4m*i))  |> List.fold(fun state x -> state + x) 0m
let neg = s1 |> List.map(fun i -> 1m / (-3m - 4m*i)) |> List.fold(fun state x -> state + x) 0m
let pi = 4m * (pos + neg)
printfn "%A" pi