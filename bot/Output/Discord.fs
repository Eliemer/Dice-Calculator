namespace Output

open System.Text
open DSharpPlus.Entities
open Evaluator

module Discord =
    let embed (expr: ExpressionReport) : DiscordEmbed =

        let deb = DiscordEmbedBuilder()

        deb
            .AddField("Total", expr.Total.ToString())
            .AddField("Input Expression", expr.Expression)
        |> ignore

        for report in expr.Result do
            let sb = StringBuilder()
            sb.AppendLine($"Total: {report.Total}") |> ignore

            match report.Result with
            | [] -> ()
            | [ n ] -> sb.AppendLine($"Selected Roll: {n}") |> ignore
            | rs when rs.Length < report.Rolls.Length -> sb.Append("Selected Rolls: ").AppendJoin(", ", rs).AppendLine() |> ignore
            | _ -> ()

            match report.Rolls with
            | [] -> ()
            | rs -> sb.Append("All Rolls: ").AppendJoin(", ", rs).AppendLine() |> ignore

            deb.AddField(report.DiceExpression, sb.ToString()) |> ignore


        deb.Build()
