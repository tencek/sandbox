// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
//#I @"bin\Release"
#r @"C:\git\sandbox\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

open FSharp.Data

type Drevnice = 
  HtmlProvider<"http://hydro.chmi.cz/hpps/hpps_prfdata.php?seq=307366", Encoding="utf-8">


Drevnice().Tables.Table8.Rows.[0].``Stav [cm]`` |> printfn "%A"

