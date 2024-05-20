namespace Model

open System

type RollingMethod =
    | Total
    | KeepHigh of uint
    | KeepLow of uint

    override x.ToString() =
        match x with
        | Total -> String.Empty
        | KeepHigh n -> $"KH{n}"
        | KeepLow n -> $"KL{n}"

type DiceSize =
    | Constant of uint
    | Enumeration of int list
    | Range of int * int

    member this.Length =
        match this with
        | Constant n -> int n
        | Range(a, b) -> (b - a) + 1
        | Enumeration ns -> ns.Length

    override this.ToString() =
        match this with
        | Constant n -> n.ToString()
        | Range(a, b) -> $"[{a}..{b}]"
        | Enumeration ns ->
            let sb = System.Text.StringBuilder(ns.Length)
            sb.Append('[').AppendJoin(',', ns |> List.toArray).Append(']').ToString()

type Dice =
    { Count: uint
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
