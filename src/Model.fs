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

type DiceSize =
    | Constant of int
    | Sequence of int list

    member this.Length =
        match this with
        | Constant n -> n
        | Sequence ns -> ns.Length

    override this.ToString() =
        match this with
        | Constant n -> n.ToString()
        | Sequence ns -> sprintf "%A" ns

type Dice =
    { Count: int
      Method: RollingMethod
      Size: DiceSize }

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
    { First: ValueExpression
      Rest: (Operator * ValueExpression) list }
