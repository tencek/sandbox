namespace Dobble

type Symbol = Symbol of string

type Card = Symbols of Symbol list

type Game = Cards of Card list
