module DiceCalculator

open System
open Output

[<EntryPoint>]
let main _args =
    let rnd = Random()

    while true do
        printfn "Input dice expression: "

        Console.ReadLine()
        |> fun input ->
            if String.IsNullOrWhiteSpace input then
                Error "Invalid expression"
            else
                Ok input
        |> Result.bind Parser.Expression.evaluate
        |> Result.map (Evaluator.eval rnd)
        |> function
            | Ok rep -> Cli.prettyPrintReport rep
            | Error msg -> printfn "----\n%s----" msg

    1
