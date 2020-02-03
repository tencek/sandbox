// Learn more about F# at http://fsharp.org

open BibleStats.Bible.Tools
open FSharp.Data
open System.Text

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance)

let [<Literal>] private CalendarSample = __SOURCE_DIRECTORY__ + @"\calendar2020.html"

[<Literal>]
let Calendar2020Url = @"http://www.katolik.cz/kalendar/kal_rok.asp?r=2020"

type Calendar2020 = HtmlProvider<CalendarSample, Encoding="utf-8">

let convert (str:string) =
   let cp1250 = Encoding.GetEncoding("windows-1250")
   let cp1250Bytes = cp1250.GetBytes(str)
   let utf8Bytes = Encoding.Convert(cp1250, Encoding.UTF8,cp1250Bytes)
   // Convert the new byte[] into a char[] and then into a string.
   let mutable unicodeChars = Array.create (Encoding.UTF8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)) ' '
   Encoding.UTF8.GetChars(utf8Bytes, 0, utf8Bytes.Length, unicodeChars, 0) |> ignore
   new string(unicodeChars)

let replace (str:string) =
   System.Text.RegularExpressions.Regex.Replace(str, @"\<h2\>[^<]*\<\/h2\>", "<h2>Liturgický kalendář na rok</h2>")

let test () =
   let unicodeString = "This string contains the unicode character Pi (\u03a0)"

   // Create two different encodings.
   let ascii = Encoding.ASCII
   let unicode = Encoding.Unicode

   // Convert the string into a byte array.
   let unicodeBytes = unicode.GetBytes(unicodeString)

   // Perform the conversion from one encoding to the other.
   let asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes)
         
   // Convert the new byte[] into a char[] and then into a string.
   let mutable asciiChars = Array.create (ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)) ' '
   ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0) |> ignore
   let asciiString = new string(asciiChars)

   // Display the strings created before and after the conversion.
   printfn "Original string: %A" unicodeString
   printfn "Ascii converted string: %A" asciiString

[<EntryPoint>]
let main argv =
   
   //test ()
   //System.DateTime.Now
   //|> GetCalendarUrl
   //|> FSharp.Data.Http.RequestString
   //|> convert
   //|> printfn "ěščřžýáíé:%A"
 

   let calendar2020 = Calendar2020.GetSample ()
   calendar2020.Tables.``Liturgický kalendář na rok``.Rows
   |> Seq.last
   |> Seq.singleton
   |> Seq.iter (fun row -> printfn "%A" (row.Column1, row.Column4))

   let y2021 = 
      Http.RequestString "http://www.katolik.cz/kalendar/kal_rok.asp?r=2021"
      |> convert
      |> replace
      |> (fun str -> new System.IO.StringReader(str))

   let calendar2021 = Calendar2020.Load(y2021)
   calendar2021.Tables.``Liturgický kalendář na rok``.Rows
   |> Seq.last
   |> Seq.singleton
   |> Seq.iter (fun row -> printfn "%A" (row.Column1, row.Column4))

   0 // Return an integer exit code