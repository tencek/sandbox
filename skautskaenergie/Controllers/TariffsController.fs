namespace skautskaenergie.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc

[<Route("api/[controller]")>]
type TariffsController () =
    inherit Controller()

    [<HttpGet>]
    member this.Get() =
        [|"D01d"; "D02d" ; "..."|]

    [<HttpGet("{id}")>]
    member this.Get(id:string) =
        id
