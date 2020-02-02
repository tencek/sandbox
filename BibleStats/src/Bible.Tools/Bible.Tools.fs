module BibleStats.Bible.Tools

[<Literal>]
let CalenarBaseUrl = "http://katolik.cz/kalendar/kalendar.asp"

let GetCalendarUrl (date:System.DateTime) =
   sprintf "%s?d=%d&m=%d&r=%d" CalenarBaseUrl date.Day date.Month date.Year

let hello name =
   printfn "Hello %s" name
