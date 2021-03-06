﻿#r "netstandard"
#r "system.net.http"
//#r "FSharp.Data.TypeProviders.dll"
//open FSharp.Data.TypeProviders

open System.Net.Http
open System
open System.Collections.Generic
//open FSharp.Data.TypeProviders

//type AbacusPresence = HtmlProvider<"">

let names = seq {
    use sr = new System.IO.StreamReader("assets\skaut-dikydobrovolnikum-jmena.txt")
    while not sr.EndOfStream do
    done
    let c = sr.Read()
    yield ""
}


let createClient () = 
    new HttpClient()

let login login password (client:HttpClient) = async {
    let formData = seq { 
        yield new KeyValuePair<string,string>("Login", login) 
        yield new KeyValuePair<string, string>("Pwd", password)
    }
    let httpContent = new FormUrlEncodedContent(formData)
    let postUri = new Uri("https://jobabacus.in.edhouse.cz/login.php?Act=Validate")
    let! result = client.PostAsync(postUri, httpContent) |> Async.AwaitTask
    return! result.Content.ReadAsStringAsync() |> Async.AwaitTask
}

let client = createClient ()
let result = 
    client
    |> login "login" "password"
    |> Async.RunSynchronously

let presence = client.GetStringAsync("https://jobabacus.in.edhouse.cz/index.php?Page=Pres") |> Async.AwaitTask |> Async.RunSynchronously

printfn "|||%s|||" presence


