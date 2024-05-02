namespace Output

open Evaluator

[<RequireQualifiedAccess>]
module Cli =

    let prettyPrintReport (er : ExpressionReport) : unit =
        printfn "===="
        printfn "Total: %d" er.Total
        printfn "Input Expression: %s" er.Expression
        printfn "Breakdown:"

        for report in er.Result do
            printfn "  '%s':" report.DiceExpression
            printfn "    Total: %d" report.Total

            report.Rolls
            |> List.map _.ToString()
            |> function
                | [] -> ()
                | ls ->
                    ls
                    |> String.concat ", "
                    |> printfn "    All rolls: [%s]"

            match report.Result with
            | Single n -> printfn "    Selected roll: %d" n
            | Multiple ns -> 
                ns
                |> List.map _.ToString()
                |> String.concat ", "
                |> printfn "    Selected rolls: [%s]"
        printfn "===="