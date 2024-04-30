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

    let valueExpression = 
        attempt (Dice.DiceParser |>> ValueExpressions.Dice)
        <|> (Dice.FlatParser |>> ValueExpressions.Flat) 

    let terminating = valueExpression |>> Terminating

    let ExpressionParser, private expressionRef = createParserForwardedToRef ()

    let operator =
        valueExpression .>> spaces .>>. (add <|> subtract) .>> spaces
        .>>. ExpressionParser
        |>> fun ((d, op), e) -> { left = d; right = e; operator = op }

    let continuing = operator |>> Continuing

    do expressionRef := ((attempt continuing) <|> terminating)

    let evaluate str =
        match run ExpressionParser str with
        | Success(res, _, _) -> Result.Ok res
        | Failure(msg, _, _) -> Result.Error msg

