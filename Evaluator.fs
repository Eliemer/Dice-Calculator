module Evaluator

open System
open Model

let roll (rnd: Random) (vexpr: ValueExpressions) =
    match vexpr with
    | Flat n ->
        {
            DiceExpression = n.ToString()
            Result = Single n
            Rolls = []
        }
    | Dice d ->
        let rolls =
            [ for _ in 1 .. d.Count do
                rnd.Next(0, d.Size) + 1 ]

        let result =
            match d.Method with
            | Total -> List.sum rolls |> Single
            | KeepHigh n -> List.sortDescending rolls |> List.take n |> Multiple
            | KeepLow n -> List.sort rolls |> List.take n |> Multiple

        { DiceExpression = $"{d.Count}D{d.Size}{d.Method.ToString()}"
          Result = result
          Rolls = rolls }

let eval (rnd: Random) (expr: Expression) =
    let rec eval_inner
        (rnd: Random)
        (accumulator: int -> int)
        (accumulatingResult: ExpressionReport)
        (expr: Expression)
        =
        match expr with
        | Terminating d ->
            let report = roll rnd d

            { Result = report :: accumulatingResult.Result |> List.rev
              Total = List.sum report.Results |> accumulator
              Expression =
                $"{accumulatingResult.Expression} {report.DiceExpression}"
                    .Trim()
                    .ToLowerInvariant() }

        | Continuing({ left = d; right = e; operator = op }) ->
            let report = roll rnd d

            let total = accumulator report.Total

            let opFun =
                match op with
                | Add -> (+) total
                | Subtract -> (-) total

            let res =
                { Result = report :: accumulatingResult.Result
                  Total = total
                  Expression = $"{accumulatingResult.Expression} {report.DiceExpression} {op}" }

            eval_inner rnd opFun res e

    eval_inner
        rnd
        id
        { Result = []
          Total = 0
          Expression = String.Empty }
        expr
