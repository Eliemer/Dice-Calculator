module Evaluator

open System
open Model

type RollReport =
    { DiceExpression: string
      Result: int list
      Rolls: int list }

    member report.Total = List.sum report.Result

type ExpressionReport =
    { Result: RollReport list
      Total: int
      Expression: string }

let roll (rnd: Random) (vexpr: ValueExpression) =
    match vexpr with
    | Flat n ->
        { DiceExpression = n.ToString()
          Result = [ n ]
          Rolls = [] }
    | Dice d ->
        let rolls =
            [ for _ in 1 .. d.Count do
                  rnd.Next(0, d.Size) + 1 ]

        let result =
            match d.Method with
            | Total -> rolls
            | KeepLow n -> List.sort rolls |> List.take n
            | KeepHigh n -> List.sortDescending rolls |> List.take n

        { DiceExpression = $"{d.Count}D{d.Size}{d.Method.ToString()}"
          Result = result
          Rolls = rolls }

// let eval (rnd: Random) (expr: Expression) =
//     let rec eval_inner
//         (rnd: Random)
//         (accumulator: int -> int)
//         (accumulatingResult: ExpressionReport)
//         (expr: Expression)
//         =
//         match expr with
//         | Terminating d ->
//             let report = roll rnd d

//             { Result = report :: accumulatingResult.Result |> List.rev
//               Total = List.sum report.Result |> accumulator
//               Expression =
//                 $"{accumulatingResult.Expression} {report.DiceExpression}"
//                     .Trim()
//                     .ToLowerInvariant() }

//         | Continuing({ left = d; right = e; operator = op }) ->
//             let report = roll rnd d

//             let total = accumulator report.Total

//             let opFun =
//                 match op with
//                 | Add -> (+) total
//                 | Subtract -> (-) total

//             let res =
//                 { Result = report :: accumulatingResult.Result
//                   Total = total
//                   Expression = $"{accumulatingResult.Expression} {report.DiceExpression} {op.ToString()}" }

//             eval_inner rnd opFun res e

//     eval_inner
//         rnd
//         id
//         { Result = []
//           Total = 0
//           Expression = String.Empty }
//         expr


let eval (rnd: Random) (expr: Expression) : ExpressionReport =

    let init =
        let report = roll rnd expr.First

        { Result = [ report ]
          Total = List.sum report.Result
          Expression = $"{report.DiceExpression}".Trim().ToLowerInvariant() }

    let folder (acc: ExpressionReport) ((op, vexpr): Operator * ValueExpression) =
        let report = roll rnd vexpr
        let res =
            {   Result = report :: acc.Result
                Total = 
                    match op with
                    | Add -> acc.Total + report.Total
                    | Subtract -> acc.Total - report.Total
                Expression = $"{acc.Expression} {op.ToString()} {report.DiceExpression}" }

        res

    List.fold folder init expr.Rest
    |> fun res -> { res with Result = List.rev res.Result }
