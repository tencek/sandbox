open Ncr.Devices.PointOfService.CustomerDisplayCli
open System

[<EntryPoint>]
let main argv =
   let a = 7
   let b = 9
   printfn "It is %O that %d > %d" (CDGreater(a,b)) a b
   printfn "It is %O that %d > %d" (CDGreater(b,a)) b a

   let command = CreateEmptyCommand CommandType.DisplaySelfTest
   printfn "Sending command : %O..." command
   SendGenericCommand(command) |> printfn "Result: %O"
   
   let printStatusCallBack = new StatusChangedCallBack (printfn "New status of Customer Display is: %O")
   printfn "Setting callback..."
   SetStatusNotificationByCallback(printStatusCallBack) |> printfn "Result %O"
    
   0 // return an integer exit code