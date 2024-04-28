module DiceCalculator

open System

[<EntryPoint>]
let main args =
    let rnd = Random()

    printfn "%A" args

    args
    |> function
        | [||] -> Error "Please provide an expression to evaluate"
        | expr -> Ok <| String.concat String.Empty expr
    |> Result.bind Parser.Expression.evaluate
    |> Result.map _.Eval(rnd)
    |> function
        | Ok res -> printfn "%A" res; 0
        | Error msg -> printfn "%s" msg; 1