namespace Dobble

type Symbol = Name of string

type Card = Symbols of Set<Symbol>

type Game = Cards of Set<Card>
