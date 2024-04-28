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

    member x.Roll(rnd: Random) =
        let rolls =
            [ for _ in 1 .. x.Count do
                  rnd.Next(0, x.Size) + 1 ]

        let result =
            match x.Method with
            | Total -> List.sum rolls |> Single
            | KeepHigh n -> List.sortDescending rolls |> List.take n |> Multiple
            | KeepLow n -> List.sort rolls |> List.take n |> Multiple

        { DiceExpression = $"{x.Count}D{x.Size}{x.Method.ToString()}"
          Result = result
          Rolls = rolls }

type Operator =
    | Add
    | Subtract

    override x.ToString() =
        match x with
        | Add -> "+"
        | Subtract -> "-"

type OperatorRecord =
    { operator: Operator
      left: Dice
      right: Expression }

and Expression =
    | Terminating of Dice
    | Continuing of OperatorRecord

    member x.Eval(rnd: Random) =
        x.Eval(
            rnd,
            id,
            { Result = []
              Total = 0
              Expression = String.Empty }
        )

    member private x.Eval(rnd: Random, accumulator: int -> int, accumulatingResult: ExpressionReport) =
        match x with
        | Terminating d ->
            let report = d.Roll(rnd)

            { Result = report :: accumulatingResult.Result |> List.rev
              Total = List.sum report.Results |> accumulator
              Expression = $"{accumulatingResult.Expression} {report.DiceExpression}".Trim().ToLowerInvariant() }

        | Continuing({ left = d; right = e; operator = op }) ->
            let report = d.Roll(rnd)

            let total = accumulator report.Total

            let opFun =
                match op with
                | Add -> (+) total
                | Subtract -> (-) total

            let res =
                { Result = report :: accumulatingResult.Result
                  Total = total
                  Expression = $"{accumulatingResult.Expression} {report.DiceExpression} {op}" }

            e.Eval(rnd, opFun, res)
