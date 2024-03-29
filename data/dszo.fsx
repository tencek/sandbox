﻿// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"..\packages\FSharp.Data.3.0.0\lib\net45\FSharp.Data.dll"

open FSharp.Data

type Coordinates = { Lat:float ; Lng:float }

type Orientation = Orientation of int

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
        Orientation : Orientation
    }

type Vehicles = JsonProvider<"http://www.dszo.cz/online/tabs2.php", Encoding="utf-8">

type Snapshot = { TimeStamp:System.DateTime ; Vehicles:seq<Vehicle> }

let (|Regex|_|) pattern input =
    let m = System.Text.RegularExpressions.Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None

let createSnapShot () =
    let loadCoordinates () = 
        let (timeStamp, coordinates, orientations) = 
            Http.RequestString("http://www.dszo.cz/online/pokus.php", responseEncodingOverride="utf-8").Split('\n')
            |> Seq.fold (fun (timestamp, coordinates, orientations) line -> 
                match line.Trim() with
                | Regex @"window\.epoint([0-9]+)=new google\.maps\.LatLng\((.+),(.+)\);" [vehicleNum ; lat ; lng ] -> 
                    (timestamp, ((int vehicleNum, {Lat=float lat ; Lng = float lng})::coordinates), orientations)
                | Regex @"image([0-9]+) = \{[^\}]*rotation: ([0-9]+),[^\}]}*" [vehicleNum ; orientation] ->
                    (timestamp, coordinates, (int vehicleNum, Orientation (int orientation))::orientations)
                | Regex @"Data aktualizována: ([0-9:\. ]+)&nbsp;" [dateTimeStr] -> 
                    try
                        (System.DateTime.Parse(dateTimeStr) |> Some, coordinates, orientations)
                    with
                        _exn -> 
                            printfn "Failed to parse %A as date time!" dateTimeStr
                            (timestamp, coordinates, orientations)
                | _ -> 
                    (timestamp, coordinates, orientations)
            ) (None, List.empty, List.Empty)
        match timeStamp with
        | Some timeStamp -> 
            (timeStamp, Map.ofList coordinates, Map.ofList orientations)
        | None -> 
            printfn "Timestamp not loaded! Using current time..."
            (System.DateTime.Now, Map.ofList coordinates, Map.ofList orientations)

    let (timeStamp, coordinates, orientations) = loadCoordinates ()
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
                    Orientation = orientations.Item (int item.Strings.[0])
                } |> Some
            with
                exn -> 
                    printfn "Data error: %A" exn
                    printfn "Item not parsed: %A" item
                    None
        )
        |> Seq.choose id
        |> Seq.sortBy (fun vehicle -> vehicle.Number)
    { TimeStamp=timeStamp ; Vehicles=vehicles }

//let vehicles = 
//    loadVehicles ()
//    |> snd
//    |> Seq.map (fun vehicle -> (vehicle.Number, vehicle))
//    |> Map.ofSeq

//let oldVehicles = 
//    vehicles
//    |> Map.filter (fun number vehicle -> [170 ; 346 ; 350] |> List.contains number)

//printfn "vse: %A" vehicles
//printfn "stare: %A" oldVehicles

let saveSnapshot outFilePath snapshot = 
    if not <| System.IO.File.Exists(outFilePath) then
        let line = sprintf "%A;%A;%A;%A;%A;%A;%A;%A;%A;%A;%A;%A;%A" "DayOfWeek" "Date" "Time" "Number" "LineNumber" "Delay" "Station" "Direction" "Shift" "Driver" "Latitude" "Longitude" "Orientation"
        System.IO.File.AppendAllLines (outFilePath, Seq.singleton line) 

    let linesOut = 
        snapshot.Vehicles
        |> Seq.map (fun v -> 
            let (Orientation orientation) = v.Orientation
            sprintf "%A;%s;%s;%d;%d;%A;%A;%A;%A;%d;%f;%f;%d"
                snapshot.TimeStamp.DayOfWeek 
                (snapshot.TimeStamp.ToShortDateString()) 
                (snapshot.TimeStamp.ToLongTimeString()) 
                v.Number 
                v.LineNumber 
                v.Delay 
                v.Station 
                v.Direction 
                v.Shift 
                v.Driver 
                v.Coordinates.Lat 
                v.Coordinates.Lng 
                orientation)
    System.IO.File.AppendAllLines(outFilePath, linesOut)

let saveSnapshot' = saveSnapshot @"C:\temp\vehicles2.csv"

Seq.initInfinite ( fun _x -> ())
|> Seq.fold (fun lastTimeStamp _elm -> 
    try
        let snapshot = createSnapShot ()
        if snapshot.TimeStamp <> lastTimeStamp then
            saveSnapshot' snapshot
        System.Threading.Thread.Sleep(System.TimeSpan.FromMilliseconds(30000.0))
        snapshot.TimeStamp
    with 
        exn -> 
            printfn "%A: Some error occured: %A" System.DateTime.Now exn.Message
            System.Threading.Thread.Sleep(System.TimeSpan.FromMilliseconds(10000.0))
            lastTimeStamp) ( System.DateTime.MinValue )
