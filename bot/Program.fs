// For more information see https://aka.ms/fsharp-console-apps
namespace Discord

open System
open System.Threading
open System.Threading.Tasks

module TaskUtils =
    /// <summary>Queues the specified work to run on the thread pool. Helper for Task.Run</summary>
    let inline runOnThreadpool (cancellationToken: CancellationToken) (func: unit -> Task<'b>) =
        Task.Run<'b>(func, cancellationToken)

    /// <summary>Helper for t.GetAwaiter().GetResult()</summary>
    let inline runSynchounously (t: System.Threading.Tasks.Task<'b>) = t.GetAwaiter().GetResult()

    let inline tryCancel (cts: CancellationTokenSource) =
        try
            cts.Cancel()
        with :? ObjectDisposedException as e ->
            // if CTS is disposed we're probably exiting cleanly
            ()

module DiceCalculator =
    open System.IO
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Logging

    open DSharpPlus
    open DSharpPlus.Commands
    open DSharpPlus.Commands.Processors

    open Discord.DiceCalculatorCommands

    let services = 
        ServiceCollection()
            .AddSingleton<DiscordClient>(fun _provider -> 
                let DISCORD_TOKEN = 
                    try
                        File.ReadAllText("TOKEN")
                    with
                    | :? FileNotFoundException ->
                        let ENV_DISCORD_TOKEN = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
                        if String.IsNullOrWhiteSpace ENV_DISCORD_TOKEN then
                            failwith "Discord bot token not found!"
                        else
                            ENV_DISCORD_TOKEN

                new DiscordClient(new DiscordConfiguration(
                Token=DISCORD_TOKEN,
                TokenType=TokenType.Bot,
                Intents=(TextCommands.TextCommandProcessor.RequiredIntents ||| SlashCommands.SlashCommandProcessor.RequiredIntents ||| DiscordIntents.MessageContents),
                MinimumLogLevel=LogLevel.Debug
            )))
            .AddSingleton<Random>(Random())

    let serviceProvider = services.BuildServiceProvider()

    // type ResolvePrefixAsyncDelegate = delegate of CommandsExtension * Entities.DiscordMessage -> ValueTask<int>

    let bot (ct: CancellationToken) =
        task {
            let dc = serviceProvider.GetRequiredService<DiscordClient>()

            let cmdExt = dc.UseCommands(CommandsConfiguration(
                ServiceProvider=serviceProvider,
                DebugGuildId=149603585301282818uL))

            cmdExt.AddCommands(types=[typeof<DiceCommand>])
            let textCommandProcessor = TextCommands.TextCommandProcessor(
                TextCommands.TextCommandConfiguration(
                    PrefixResolver = TextCommands.Parsing.ResolvePrefixDelegateAsync(
                        fun ce msg -> TextCommands.Parsing.DefaultPrefixResolver("?").ResolvePrefixAsync(ce, msg))
                ))

            let slashCommandProcessor = SlashCommands.SlashCommandProcessor()

            do! cmdExt.AddProcessorsAsync(textCommandProcessor, slashCommandProcessor)

            do! dc.ConnectAsync()
            do! Task.Delay(-1)

            return 1
        }

    [<EntryPoint>]
    let main _args =
        use cts = new CancellationTokenSource()

        TaskUtils.runOnThreadpool cts.Token (fun () -> bot cts.Token)
        |> TaskUtils.runSynchounously