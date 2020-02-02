// Learn more about F# at http://fsharp.org

open BibleStats.Bible.Tools
open FSharp.Data

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance)

[<Literal>]
let Calendar2025Url = @"http://www.katolik.cz/kalendar/kal_rok.asp?r=2025"

type Calendar2025 = HtmlProvider<Calendar2025Url, Encoding="windows-1250">

let convert (str:string) =
   let cp1250 = System.Text.Encoding.GetEncoding("iso-8859-1")
   let cp1250Bytes = cp1250.GetBytes(str)
   let utf8Bytes = System.Text.Encoding.Convert(cp1250, System.Text.Encoding.Unicode,cp1250Bytes)
   System.Text.Encoding.Unicode.GetString(utf8Bytes)

let test () =
   System.Text.Encoding.GetEncodings()
   |> Seq.map (fun info -> (info.CodePage, info.Name, info.DisplayName))
   |> Seq.iter (printfn "%A")

[<EntryPoint>]
let main argv =
   
   //test ()
   //System.DateTime.Now
   //|> GetCalendarUrl
   //|> FSharp.Data.Http.RequestString
   //|> convert
   //|> printfn "ěščřžýáíé:%A"
 
   let calendar2025 = Calendar2025.Load(@"http://www.katolik.cz/kalendar/kal_rok.asp?r=2025")
   calendar2025.Tables.``Liturgický kalendáø na rok 2025``.Rows
   //|> Seq.last
   //|> (fun table -> table.Rows)
   |> Seq.iter (printfn "%A")

   0 // Return an integer exit code