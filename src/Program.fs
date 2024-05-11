module DiceCalculator

open System
open Output

[<EntryPoint>]
let main args =
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
        // |> Result.iter Cli.prettyPrintReport
        |> Result.map (Output.Json.jsonify)
        |> function
            | Error msg -> printfn "Error: %s" msg
            | Ok res -> printfn "Result:\n%s" res

    1