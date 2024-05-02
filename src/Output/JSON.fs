namespace Output

open System
open System.Text.Json
open System.Text.Json.Serialization

open Evaluator

module private Dtos =

    type RollReportDto =
        { Total: int
          Rolls: int list
          Selected: int list
          Expression: string }

    type ExpressionReportDto =
        { Total: int
          Expression: string
          Breakdown: RollReportDto list }

[<RequireQualifiedAccess>]
module Json =
    open Dtos

    let private opt = JsonFSharpOptions.Default().ToJsonSerializerOptions()

    let jsonify (er: ExpressionReport) : string =
        let dto =
            { Total = er.Total
              Expression = er.Expression
              Breakdown =
                [ for res in er.Result do
                      { RollReportDto.Total = res.Total
                        Rolls = res.Rolls
                        Selected = res.Results
                        Expression = res.DiceExpression } ] }

        JsonSerializer.Serialize(dto, opt)
