module DiceCalculator

open System

[<EntryPoint>]
let main args =
    let rnd = Random()

    printfn "%A" args


    // args
    // |> function
    //     | [||] -> Error "Please provide an expression to evaluate"
    //     | expr -> Ok <| String.concat String.Empty expr
    while true do
        printfn "Input dice expression: "

        Console.ReadLine()
        |> fun input ->
            if String.IsNullOrWhiteSpace input then
                Error "Invalid expression"
            else
                Ok input
        |> Result.bind Parser.Expression.evaluate
        |> function
            | Error err -> Error err
            | Ok model ->
                printfn "%A" model
                Ok model
        |> Result.map (Evaluator.eval rnd)
        |> printfn "%A"

    // |> function
    //     | Ok res ->
    //         printfn "%A" res
    //         0
    //     | Error msg ->
    //         printfn "%s" msg
    //         1
    1