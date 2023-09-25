namespace MagicCollectionHelper.AvaloniaApp

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent

open MagicCollectionHelper.Core

module AppRoot =
    type State = {
        cardInfo: IWritable<CardInfoMap>
        dsEntries: IWritable<DeckStatsCardEntry list>
        entries: IWritable<AgedEntry list>
        prefs: IWritable<Prefs>
        setData: IWritable<SetDataMap>
    }

    module Effect =
        let recalculateEntries cardInfo dsEntries (entries: IWritable<AgedEntry list>) = async {
            let! newEntries = DeckStatsCardEntry.listToEntriesAsync cardInfo dsEntries

            let oldEntries = entries.Current |> List.map (fun agedEntry -> agedEntry.data)

            AgedEntry.determineCardAge oldEntries newEntries |> entries.Set
        }

    let setupEffects (ctx: IComponentContext) state =
        // Recalculate entries and save collection to file, when something changes
        ctx.useEffect (
            fun () ->
                [
                    Effect.recalculateEntries state.cardInfo.Current state.dsEntries.Current state.entries
                    Persistence.DeckStatsCardEntry.save state.dsEntries.Current
                ]
                |> Async.Parallel
                |> Async.Ignore
                |> Async.Start
            , [ EffectTrigger.AfterChange state.dsEntries ]
        )

        // Save preferences to file, if they change
        ctx.useEffect (
            fun () -> Persistence.Prefs.save state.prefs.Current |> Async.Start
            , [ EffectTrigger.AfterChange state.prefs ]
        )

    let create () =
        Component(fun ctx ->
            let initialState = ctx.useState<Loading.InitialAppState option> None

            // We render loading screen, if not everything is ready
            match initialState.Current with
            | Some initialState ->
                let state: State = {
                    cardInfo = ctx.useState initialState.cardInfo
                    dsEntries = ctx.useState initialState.dsEntries
                    entries = ctx.useState initialState.entries
                    prefs = ctx.useState initialState.prefs
                    setData = ctx.useState initialState.setData
                }

                setupEffects ctx state

                App.create
                    {
                        cardInfo = state.cardInfo
                        dsEntries = state.dsEntries
                        entries = state.entries
                        prefs = state.prefs
                        setData = state.setData
                    }
            | None -> Loading.create initialState)

type MainWindow() as this =
    inherit HostWindow()

    do
#if DEBUG
        this.AttachDevTools()
#endif

        base.Title <- "MagicCollectionHelper"
        base.Width <- 800.0
        base.Height <- 600.0

        this.Content <- AppRoot.create ()

type BaseApp() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())
        this.Styles.Load "avares://MagicCollectionHelper.AvaloniaApp/Styles.xaml"
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder
            .Configure<BaseApp>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
