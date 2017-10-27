module skautskaenergie.Tariff

[<Measure>] type CZK

type Tariff = 
    | SingleRateTariff of SingleRateTariff
    | DoubleRateTariff of DoubleRateTariff

and SingleRateTariff = {
    Name: TariffName
    Price: Price
}

and DoubleRateTariff = {
    Name: TariffName
    PriceHigh: Price
    PriceLow: Price
}

and TariffName = string
and Price = decimal<CZK>

let _gas = SingleRateTariff { Name = "Plyn" ; Price = 497m<CZK> }
let _D25d = DoubleRateTariff { Name = "D25d" ; PriceHigh = 1090m<CZK> ; PriceLow = 765m<CZK> }

let tariffs = [| _gas ; _D25d |]