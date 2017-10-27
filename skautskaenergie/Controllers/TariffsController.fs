namespace skautskaenergie.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc

open skautskaenergie.Tariff

[<Route("api/[controller]")>]
type TariffsController () =
    inherit Controller()

    let tariffs = skautskaenergie.Tariff.tariffs

    [<HttpGet>]
    member this.Get() =
        JsonResult tariffs

    [<HttpGet("names")>]
    member this.GetNames() =
        tariffs
        |> Array.map ( function
            | SingleRateTariff srt -> srt.Name
            | DoubleRateTariff drt -> drt.Name)
        |> JsonResult

    [<HttpGet("{id}")>]
    member this.Get(id:string) =
        id
