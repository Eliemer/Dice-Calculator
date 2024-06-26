namespace Parser

open FParsec

open Model

module private Testing =
    let (<!>) (p: Parser<_, _>) label : Parser<_, _> =
        fun stream ->
            printfn "%A: Entering %s" stream.Position label
            let reply = p stream
            printfn "%A: Leaving %s (%A)" stream.Position label reply.Status
            reply

module RollingMethod =
    let keepHigh =
        pstringCI "H" <|> pstringCI "KH" >>. opt puint32
        |>> function
            | Some n -> KeepHigh n
            | None -> KeepHigh 1u

    let keepLow =
        pstringCI "L" <|> pstringCI "KL" >>. opt puint32
        |>> function
            | Some n -> KeepLow n
            | None -> KeepLow 1u

    let total = optional (pstringCI "T") |>> fun () -> Total
    let RollingMethodParser = (attempt keepHigh) <|> (attempt keepLow) <|> total

module DiceSize =
    let constant = puint32 |>> Constant

    let enumeration =
        (sepBy pint32 (spaces .>>? skipChar ',' .>> spaces)) |>> Enumeration

    let range =
        pint32 .>> spaces .>>? skipStringCI ".." .>> spaces .>>. pint32
        >>= fun (a, b) ->
            if a > b then
                fail "The right number of the range must be equal or larger than the left number"
            else
                preturn <| Range(a, b)

    let sequence =
        skipChar '[' >>. spaces >>. (range <|> enumeration) .>> spaces .>> skipChar ']'

    let DiceSizeParser = constant <|> sequence

module Dice =
    let count = puint32
    let size = DiceSize.DiceSizeParser
    let method = RollingMethod.RollingMethodParser

    let DiceParser =
        count .>> (pchar 'd' <|> pchar 'D') .>>. size .>>. method
        |>> fun ((count, size), method) ->
            { Count = count
              Size = size
              Method = method }

    let FlatParser =
        pint32 .>>? notFollowedByStringCI "d"
        <?> "integer number (32-bit, signed) not followed by 'd' (case-insensitive)"

module Expression =
    let add = skipChar '+' |>> fun () -> Add
    let subtract = skipChar '-' |>> fun () -> Subtract

    let operator = (attempt add) <|> subtract

    let valueExpression =
        (Dice.FlatParser |>> ValueExpression.Flat)
        <|> (Dice.DiceParser |>> ValueExpression.Dice)

    let ExpressionParser =
        spaces >>. valueExpression
        .>>. many (spaces >>. operator .>> spaces .>>. valueExpression)
        |>> fun (first, rest) -> { First = first; Rest = rest }

    let evaluate str =
        match run ExpressionParser str with
        | Success(res, _, _) -> Result.Ok res
        | Failure(msg, _, _) -> Result.Error msg
