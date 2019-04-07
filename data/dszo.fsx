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
    let loadCoordinates () = 
        let (timestamp, coordinates) = 
            Http.RequestString("http://www.dszo.cz/online/pokus.php", responseEncodingOverride="utf-8").Split('\n')
            |> Seq.fold (fun (timestamp, coords) line -> 
                match line.Trim() with
                | Regex @"window\.epoint([0-9]+)=new google\.maps\.LatLng\((.+),(.+)\);" [vehicleNum ; lat ; lng ] -> 
                    (timestamp, ((int vehicleNum, {Lat=float lat ; Lng = float lng})::coords))
                | Regex @"Data aktualizována: ([0-9:\. ]+)&nbsp;" [dateTimeStr] -> 
                    try
                        (System.DateTime.Parse(dateTimeStr) |> Some, coords)
                    with
                        _exn -> 
                            printfn "Failed to parse %A as date time!" dateTimeStr
                            (timestamp, coords)
                | _ -> 
                    (timestamp, coords)
            ) (None, List.empty)
        match timestamp with
        | Some timestamp -> 
            (timestamp, Map.ofList coordinates)
        | None -> 
            printfn "Timestamp not loaded! Using current time..."
            (System.DateTime.Now, Map.ofList coordinates)

    let (timestamp, coordinates) = loadCoordinates ()
    let vehicles = 
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
    (timestamp, vehicles)

let vehicles = 
    loadVehicles ()
    |> snd
    |> Seq.map (fun vehicle -> (vehicle.Number, vehicle))
    |> Map.ofSeq

let oldVehicles = 
    vehicles
    |> Map.filter (fun number vehicle -> [170 ; 346 ; 350] |> List.contains number)

printfn "vse: %A" vehicles
printfn "stare: %A" oldVehicles

let outFilePath = @"C:\temp\vehicles.cvs"
if not <| System.IO.File.Exists(outFilePath) then
    let line = sprintf "%A;%A;%A;%A;%A;%A;%A;%A;%A;%A;%A;%A" "DayOfWeek" "Date" "Time" "Number" "LineNumber" "Delay" "Station" "Direction" "Shift" "Driver" "Latitude" "Longitude"
    System.IO.File.AppendAllLines (outFilePath, Seq.singleton line) 

Seq.initInfinite ( fun _x -> ())
|> Seq.fold (fun previous _elm -> 
    System.Threading.Thread.Sleep(System.TimeSpan.FromMilliseconds(30000.0))
    let (timestamp, vehicles) = loadVehicles ()
    let current = Set.ofSeq vehicles
    let changes = 
        (current - previous)
        |> Seq.map (fun v -> sprintf "%A;%A;%A;%A;%A;%A;%A;%A;%A;%A;%A;%A" timestamp.DayOfWeek timestamp.Date timestamp.TimeOfDay v.Number v.LineNumber v.Delay v.Station v.Direction v.Shift v.Driver v.Coordinates.Lat v.Coordinates.Lng)
    System.IO.File.AppendAllLines(outFilePath, changes)
    changes |> Seq.iter (printfn "%s")
    current) ( loadVehicles () |> snd |> Set.ofSeq)
