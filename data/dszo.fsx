// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"

open FSharp.Data

type Coordinates = { Lat:float; Lng:float }

type Vehicle = 
    {
        Number : int
        LineNumber : int
        Delay : System.TimeSpan
        Station : string
        Direction : string
        Shift : string
        Driver : int
        Coordinates : Coordinates
    }

type Vehicles = JsonProvider<"http://www.dszo.cz/online/tabs2.php", Encoding="utf-8">

let (|Regex|_|) pattern input =
    let m = System.Text.RegularExpressions.Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None


let loadVehicles () =
    let coordinates = 
        Http.RequestString("http://www.dszo.cz/online/pokus.php", responseEncodingOverride="utf-8").Split('\n')
        |> Seq.map (fun line -> 
            match line.Trim() with
            | Regex @"window\.epoint([0-9]+)=new google\.maps\.LatLng\((.+),(.+)\);" [vehicleNum ; lat ; lng ] -> Some (int vehicleNum, {Lat=float lat ; Lng = float lng})
            | _ -> None
        )
        |> Seq.choose id
        |> Map.ofSeq

    Vehicles.Load("http://www.dszo.cz/online/tabs2.php").Data
    |> Seq.map (fun item -> 
        try
            {
                Number = int item.Strings.[0]
                LineNumber = int item.Strings.[1]
                Delay = System.TimeSpan.Parse("00:"+item.Strings.[2])
                Station = item.Strings.[3]
                Direction = item.Strings.[4]
                Shift = item.Strings.[5]
                Driver = int item.Strings.[6]
                Coordinates = coordinates.Item (int item.Strings.[0])
            } |> Some
        with
            exn -> 
                printfn "Data error: %A" exn
                printfn "Item not parsed: %A" item
                None
    )
    |> Seq.choose id
    |> Seq.sortBy (fun vehicle -> vehicle.Number)

let vehicles = 
    loadVehicles ()
    |> Seq.map (fun vehicle -> (vehicle.Number, vehicle))
    |> Map.ofSeq

let oldVehicles = 
    vehicles
    |> Map.filter (fun number vehicle -> [170 ; 346 ; 350] |> List.contains number)

printfn "vse: %A" vehicles
printfn "stare: %A" oldVehicles

Seq.initInfinite ( fun _x -> ())
|> Seq.fold (fun previous _elm -> 
    System.Threading.Thread.Sleep(System.TimeSpan.FromMilliseconds(30000.0))
    let now = System.DateTime.Now
    let current = loadVehicles () |> Set.ofSeq
    (current - previous)
    |> Seq.iter (fun v -> printfn "%A;%A;%A;%A;%A;%A;%A;%A;%A;%A;%A" now.DayOfWeek now.TimeOfDay v.Number v.LineNumber v.Delay v.Station v.Direction v.Shift v.Driver v.Coordinates.Lat v.Coordinates.Lng)
    current) ( loadVehicles () |> Set.ofSeq)
