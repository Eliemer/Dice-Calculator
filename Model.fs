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

type ValueExpressions =
    | Dice of Dice
    | Flat of int

type Operator =
    | Add
    | Subtract

    override x.ToString() =
        match x with
        | Add -> "+"
        | Subtract -> "-"

type OperatorRecord =
    { operator: Operator
      left: ValueExpressions
      right: Expression }

and Expression =
    | Terminating of ValueExpressions
    | Continuing of OperatorRecord
