namespace Model

open System

type RollResult =
    | Single of int
    | Multiple of int list

type RollReport =
    { DiceExpression: string
      Result: RollResult
      Rolls: int list }

    member report.Results =
        match report.Result with
        | Single r -> [ r ]
        | Multiple rs -> rs

    member report.Total = List.sum report.Results

type ExpressionReport =
    { Result: RollReport list
      Total: int
      Expression: string }

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
