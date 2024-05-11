namespace Parser

open FParsec

open Model

module RollingMethod =
    let keepHigh =
        pstringCI "H" <|> pstringCI "KH" >>. opt pint32
        |>> function
            | Some n -> KeepHigh n
            | None -> KeepHigh 1

    let keepLow =
        pstringCI "L" <|> pstringCI "KL" >>. opt pint32
        |>> function
            | Some n -> KeepLow n
            | None -> KeepLow 1

    let total = optional (pstringCI "T") |>> fun () -> Total
    let RollingMethodParser = (attempt keepHigh) <|> (attempt keepLow) <|> total

module Dice =
    let count = pint32
    let size = pint32
    let method = RollingMethod.RollingMethodParser
    let DiceParser =
        count .>> (pchar 'd' <|> pchar 'D') .>>. size .>>. method
        |>> fun ((count, size), method) ->
            { Count = count
              Size = size
              Method = method }
            // |> ValueExpressions.Dice

    let FlatParser = pint32 
        // |>> ValueExpressions.Flat

module Expression =
    let add = skipChar '+' |>> fun () -> Add
    let subtract = skipChar '-' |>> fun () -> Subtract

    let operator = (attempt add) <|> subtract

    let valueExpression = 
        attempt (Dice.DiceParser |>> ValueExpression.Dice)
        <|> (Dice.FlatParser |>> ValueExpression.Flat) 

    let ExpressionParser =
        spaces >>. valueExpression
        .>>. many (spaces >>. operator .>> spaces .>>. valueExpression)
        |>> fun (first, rest) -> {First = first; Rest= rest}

    let evaluate str =
        match run ExpressionParser str with
        | Success(res, _, _) -> Result.Ok res
        | Failure(msg, _, _) -> Result.Error msg

