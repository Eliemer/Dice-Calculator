namespace Model

open System

type RollingMethod =
    | Total
    | KeepHigh of int
    | KeepLow of int

    override x.ToString() =
        match x with
        | Total -> String.Empty
        | KeepHigh n -> $"KH{n}"
        | KeepLow n -> $"KL{n}"

type Dice =
    { Count: int
      Method: RollingMethod
      Size: int }

type ValueExpression =
    | Dice of Dice
    | Flat of int

type Operator =
    | Add
    | Subtract

    override x.ToString() =
        match x with
        | Add -> "+"
        | Subtract -> "-"

type Expression =
    {
        First: ValueExpression
        Rest: (Operator * ValueExpression) list
    }
