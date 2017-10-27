namespace skautskaenergie.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc

open skautskaenergie.TariffFunctions

[<Route("api/[controller]")>]
type TariffsController () =
    inherit Controller()

    let tariffs = LoadTariffs

    [<HttpGet>]
    member this.Get() =
        tariffs |> JsonResult

    [<HttpGet("names")>]
    member this.GetNames() =
        tariffs |> GetNames |> JsonResult

    [<HttpGet("{id}")>]
    member this.Get(id:string) =
        match FindByName tariffs id with
            | Some tariff -> tariff |> JsonResult:>ActionResult
            | None -> NotFoundResult():>ActionResult
