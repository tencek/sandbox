open System.IO
open System.Text.RegularExpressions

type RegisterName = string
type Value = int
type IncOrDec = Inc | Dec
type Operator = Gt | Lt | Eq | Geq | Leq | Neq
type Condition = { registerName:RegisterName ; operator:Operator ; value:Value }
type Instruction = { registerName:RegisterName ; incOrDec:IncOrDec ; value:Value ; condition:Condition }
type Registers = { registers:Map<RegisterName,Value> ; runningMaximum:Value }

let readLines (filePath:string) = seq {
    use streamReader = new StreamReader(filePath)
    while not streamReader.EndOfStream do
        yield streamReader.ReadLine ()
    }

let (|RegisterNamePattern|_|) str = 
    let m = Regex.Match(str, @"([a-z]+)")
    if (m.Success) then 
        Some m.Groups.[1].Value 
    else
        None 

let (|ValuePattern|_|) str =
   match System.Int32.TryParse(str) with
   | (true,intValue) -> Some intValue
   | _ -> None

let (|IncOrDec|_|) str =
   match str with
   | "inc" -> Some(Inc)
   | "dec" -> Some(Dec)
   | _ -> None

let (|Operator|_|) str =
    match str with
    | ">" -> Some Gt
    | "<" -> Some Lt
    | "==" -> Some Eq
    | "!=" -> Some Neq
    | "<=" -> Some Leq
    | ">=" -> Some Geq
    | _ -> None

let (|Condition|_|) tokens = 
    match tokens with
    | RegisterNamePattern register::Operator operator::ValuePattern value::_ ->  Some {registerName=register ; operator=operator ; value=value}
    | _ -> None

let (|Instruction|_|) tokens = 
    match tokens with
    | RegisterNamePattern register::IncOrDec incOrDec::ValuePattern value::"if"::rest ->
        match rest with
        | Condition condition -> Some {registerName=register ; incOrDec=incOrDec ; value=value ; condition=condition}
        | _ -> None
    | _ -> None

let parseLine (line:string) = 
    line.Split(' ')
    |> Array.toList
    |> function
        | Instruction instruction -> Some instruction
        | _ -> failwithf "%s is not an instruction!" line

let toFullPath fileName = 
    __SOURCE_DIRECTORY__ + @"\" + fileName

let trace label x =
    printfn "| %s: %A" label x
    x

let getRegisterValue registers registerName = 
    let registers = registers.registers
    if registers |> Map.containsKey registerName then
        registers.Item registerName
    else
        0

let checkCondition getRegValueFunc condition = 
    match condition.operator with
    | Gt -> getRegValueFunc condition.registerName > condition.value
    | Lt -> getRegValueFunc condition.registerName < condition.value
    | Eq -> getRegValueFunc condition.registerName = condition.value
    | Geq -> getRegValueFunc condition.registerName >= condition.value
    | Leq -> getRegValueFunc condition.registerName <= condition.value
    | Neq -> getRegValueFunc condition.registerName <> condition.value

let processInstruction registers instruction =
    let getRegValueFunc = getRegisterValue registers
    if checkCondition getRegValueFunc instruction.condition then
        let registerName = instruction.registerName
        let newRegisterValue = 
            match instruction.incOrDec with
            | Inc -> getRegValueFunc registerName + instruction.value
            | Dec -> getRegValueFunc registerName - instruction.value
        let newRegisters = 
            registers.registers
            |> Map.remove registerName
            |> Map.add registerName newRegisterValue
        let newRunningMaximum = max registers.runningMaximum newRegisterValue
        {registers=newRegisters ; runningMaximum = newRunningMaximum}
    else
        registers

let processed = 
    "instructions.txt"
    |> toFullPath
    |> readLines
    |> Seq.map parseLine
    |> Seq.choose id
    |> Seq.fold (fun registers instruction -> processInstruction registers instruction) {registers=Map.empty ; runningMaximum=0}

let largestRegister = 
    processed.registers
    |> Map.toSeq
    |> Seq.maxBy ( fun (registerName, value) -> value )

printfn "Max register value: %s=%d, running maximum: %d" (largestRegister |> fst) (snd largestRegister) processed.runningMaximum