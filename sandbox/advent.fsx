open System.IO

let toFullPath fileName = 
    __SOURCE_DIRECTORY__ + @"\" + fileName

let readLines (filePath:string) = seq {
    use streamReader = new StreamReader(filePath)
    while not streamReader.EndOfStream do
        yield streamReader.ReadLine ()
    }

let trace label x =
    printfn "| %s: %A" label x
    x
