// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

open FSharp.Data

type Position = { Lat:float; Lng:float }

type Vehicle = 
    {
        Number : int
        LineNumber : int
        Delay : System.TimeSpan
        Station : string
        Direction : string
        Shift : string
        Driver : int
        Position : Position option
    }

type DataItem = Vehicle of Vehicle | RawData of string []

type Vehicles = JsonProvider<"http://www.dszo.cz/online/tabs2.php", Encoding="utf-8">

let (|Regex|_|) pattern input =
    let m = System.Text.RegularExpressions.Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None

let positions = 
    Http.RequestString("http://www.dszo.cz/online/pokus.php", responseEncodingOverride="utf-8").Split('\n')
    |> Seq.map (fun line -> 
        match line.Trim() with
        | Regex @"window\.epoint([0-9]+)=new google\.maps\.LatLng\((.+),(.+)\);" [vehicleNum ; lat ; lng ] -> Some (int vehicleNum, {Lat=float lat ; Lng = float lng})
        | _ -> None
    )
    |> Seq.choose id
    |> Map.ofSeq

let vehicles = 
    Vehicles.Load("http://www.dszo.cz/online/tabs2.php").Data
    |> Seq.map (fun item -> 
        try
            Vehicle {
                Number = int item.Strings.[0]
                LineNumber = int item.Strings.[1]
                Delay = System.TimeSpan.Parse("00:"+item.Strings.[2])
                Station = item.Strings.[3]
                Direction = item.Strings.[4]
                Shift = item.Strings.[5]
                Driver = int item.Strings.[6]
                Position = None
            }
        with
            _exn -> RawData item.Strings
    )
    |> Seq.map (fun item -> 
        match item with
        | Vehicle vehicle ->  vehicle
        | RawData rawData -> failwithf "XXX: %A" rawData.[0]
    )
    |> Seq.sortBy (fun vehicle -> vehicle.Number)
    |> Seq.map (fun vehicle -> (vehicle.Number, vehicle))
    |> Map.ofSeq
    |> Map.map (fun number vehicle -> { vehicle with Position = positions.Item number |> Some } )

let oldVehicles = 
    vehicles
    |> Map.filter (fun number vehicle -> [170 ; 346 ; 350] |> List.contains number)

printfn "vse: %A" vehicles
printfn "stare: %A" oldVehicles

/// load http://www.dszo.cz/komunikace/?page=zastavky
// Search for function zastavky(stav, mapa)
/// grep regexp: zast([0-9])+ = new google\.maps\.LatLng\([0-9]+\.[0-9]+,[0-9]+\.[0-9]+\);var cstring\1 ='<div class="googleinfow"><strong>[^<]+<br />
