namespace Discord

open System
open System.Threading.Tasks

open DSharpPlus
open DSharpPlus.Commands
open DSharpPlus.Commands.ArgumentModifiers

module DiceCalculatorCommands = 

    type DiceCommand (rnd : Random)=

        [<Command "ping">]
        static member PingAsync(ctx: CommandContext) : Task =
            task {
                let! _ = ctx.RespondAsync $"Pong! Latency is {ctx.Client.Ping}ms."
                return ()
            }

        [<Command "roll">]
        member _.RollDice(ctx : CommandContext, [<RemainingText>] expression: String) : Task =
            task {
                return
                    match Parser.Expression.evaluate expression with
                    | Error _msg -> task {do! ctx.RespondAsync("Could not parse input expression")}
                    | Ok model -> task {
                        let result = 
                            Evaluator.eval rnd model
                            |> Output.Discord.embed
                        do! ctx.RespondAsync result
                    }
            }
