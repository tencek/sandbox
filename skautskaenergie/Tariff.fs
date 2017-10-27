module skautskaenergie.Tariff

[<Measure>] type CZK

type Tariff = {
    Name:TariffName
    Rating:Rating
    Medium:Medium
}

and Rating =
    | SingleRate of SingleRate
    | DoubleRate of DoubleRate

and Medium = 
    | Gas
    | Electricity

and SingleRate = {
    Price: Price
}

and DoubleRate = {
    PriceHigh: Price
    PriceLow: Price
}

and TariffName = string
and Price = decimal<CZK>

let _x = {Name = "abc"; Rating = SingleRate {Price = 123m<CZK>} ; Medium = Gas}
