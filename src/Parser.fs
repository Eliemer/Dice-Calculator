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

module DiceSize =
    let constant = pint32 |>> Constant
    let enumeration = spaces >>. sepBy pint32 (spaces .>> skipChar ',' .>> spaces)
    let range = 
        spaces >>. pint32 
        .>> spaces .>> skipStringCI ".." 
        .>> spaces .>>. pint32
        |>> fun (a, b) -> [a..b]

    let sequence = skipChar '[' >>. ((attempt range) <|> enumeration) .>> spaces .>> skipChar ']' |>> Sequence
    let DiceSizeParser = (attempt constant) <|> sequence

module Dice =
    let count = pint32
    let size = DiceSize.DiceSizeParser
    let method = RollingMethod.RollingMethodParser

    let DiceParser =
        count .>> (pchar 'd' <|> pchar 'D') .>>. size .>>. method
        |>> fun ((count, size), method) ->
            { Count = count
              Size = size
              Method = method }

    let FlatParser = pint32

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
        |>> fun (first, rest) -> { First = first; Rest = rest }

    let evaluate str =
        match run ExpressionParser str with
        | Success(res, _, _) -> Result.Ok res
        | Failure(msg, _, _) -> Result.Error msg
