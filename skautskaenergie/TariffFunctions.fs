module skautskaenergie.TariffFunctions

open skautskaenergie.Tariff

let LoadTariffs = 
    let gas = { Name = "Plyn" ; Rating = SingleRate {Price = 497m<CZK>} ; Medium = Gas }
    let D25d = { Name = "D25d" ; Rating = DoubleRate {PriceHigh = 1090m<CZK> ; PriceLow = 765m<CZK>} ; Medium = Electricity }
    [ gas ; D25d ]

let GetNames tariffs = 
    tariffs
    |> Seq.map ( fun tariff -> tariff.Name )

let FindByName tariffs (name:string) = 
    tariffs
    |> Seq.tryFind(fun tariff -> tariff.Name.ToUpper() = name.ToUpper())