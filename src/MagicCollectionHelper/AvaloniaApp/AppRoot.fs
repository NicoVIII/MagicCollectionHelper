namespace MagicCollectionHelper.AvaloniaApp.Main

open Avalonia.FuncUI
open Avalonia.FuncUI.Types
open MagicCollectionHelper.Core.CardTypes
open MagicCollectionHelper.Core.DomainTypes
open MagicCollectionHelper.Core

type Processable<'a> =
    | Wait
    | Process
    | Processed of 'a

type Downloadable<'a> =
    | Prepare
    | Download
    | Import
    | Ready of 'a

type LoadingState = {
    cardInfo: IWritable<Downloadable<CardInfoMap>>
    dsEntries: IWritable<Processable<DeckStatsCardEntry list>>
    entries: IWritable<Processable<AgedEntry list>>
    prefs: IWritable<Processable<Prefs>>
    setData: IWritable<Downloadable<SetDataMap>>
}

type State = {
    cardInfo: IReadable<CardInfoMap>
    dsEntries: IWritable<DeckStatsCardEntry list>
    entries: IWritable<AgedEntry list>
    prefs: IWritable<Prefs>
    setData: IReadable<SetDataMap>
}

module Effect =
    open MagicCollectionHelper.Core.Import

    let loadCardInfo (cardInfo: IWritable<Downloadable<CardInfoMap>>) = async {
        let! filePath =
            CardData.prepareImportFile ()
            |> function
                | FileExists path -> async { return path }
                | DownloadFile job ->
                    cardInfo.Set Download
                    job

        cardInfo.Set Import
        let! importedCardInfo = CardData.importFile filePath
        cardInfo.Set(Ready importedCardInfo)
    }

    let loadPrefs (prefs: IWritable<Processable<Prefs>>) = async {
        prefs.Set Process
        let! importedPrefs = Persistence.Prefs.load ()

        importedPrefs
        |> Option.defaultValue (Prefs.create 20u 40u Decimal Config.missingPercentDefault false)
        |> Processed
        |> prefs.Set
    }

    let loadSetData (setData: IWritable<Downloadable<SetDataMap>>) = async {
        let! filePath =
            SetData.prepareImportFile ()
            |> function
                | FileExists path -> async { return path }
                | DownloadFile job ->
                    setData.Set Download
                    job

        setData.Set Import
        let! importedSetData = SetData.importFile filePath
        setData.Set(Ready importedSetData)
    }

    let loadCollection (dsEntries: IWritable<Processable<DeckStatsCardEntry list>>) = async {
        dsEntries.Set Process
        let! importedCollection = Persistence.DeckStatsCardEntry.load ()
        importedCollection |> Option.defaultValue [] |> Processed |> dsEntries.Set
    }

    let calculateEntries cardInfo dsEntries (entries: IWritable<Processable<AgedEntry list>>) = async {
        entries.Set Process
        let! newEntries = DeckStatsCardEntry.listToEntriesAsync cardInfo dsEntries
        // If we init entries, every entry is old
        newEntries
        |> List.map (fun entry -> {
            amountOld = entry.amount
            data = entry
        })
        |> Processed
        |> entries.Set
    }

    let recalculateEntries cardInfo dsEntries (entries: IWritable<AgedEntry list>) = async {
        let! newEntries = DeckStatsCardEntry.listToEntriesAsync cardInfo dsEntries

        let oldEntries = entries.Current |> List.map (fun agedEntry -> agedEntry.data)

        AgedEntry.determineCardAge oldEntries newEntries |> entries.Set
    }

module LoadingView =
    open Avalonia.Controls
    open Avalonia.Layout
    open Avalonia.FuncUI.DSL
    open MagicCollectionHelper.AvaloniaApp

    let loadingLine (icon: IView<Canvas>) text =
        StackPanel.create [
            StackPanel.orientation Orientation.Horizontal
            StackPanel.spacing 5.
            StackPanel.children [ icon; TextBlock.create [ TextBlock.fontSize 24.; TextBlock.text text ] ]
        ]

    let prefLine prefs =
        match prefs with
        | Wait -> Icons.sync, "Wait for preferences..."
        | Process -> Icons.sync, "Process preferences..."
        | Processed _ -> Icons.check, "Preferences ready!"
        ||> loadingLine

    let loadSetData setData =
        match setData with
        | Prepare -> Icons.sync, "Preparing set data import..."
        | Download -> Icons.sync, "Downloading set data..."
        | Import -> Icons.sync, "Importing set data..."
        | Ready _ -> Icons.check, "Set data ready!"
        ||> loadingLine

    let loadCollection collection =
        match collection with
        | Wait -> Icons.sync, "Wait for collection import..."
        | Process -> Icons.sync, "Process collection..."
        | Processed _ -> Icons.check, "Collection ready!"
        ||> loadingLine

    let loadCardInfo cardInfo =
        match cardInfo with
        | Prepare -> Icons.sync, "Preparing card info import..."
        | Download -> Icons.sync, "Downloading card info..."
        | Import -> Icons.sync, "Importing card info..."
        | Ready _ -> Icons.check, "Card info ready!"
        ||> loadingLine

    let processEntries entries =
        match entries with
        | Wait -> Icons.reloadAlert, "Waiting..."
        | Process -> Icons.reload, "Process collection..."
        | Processed _ -> Icons.check, "Collection ready!"
        ||> loadingLine

    let render (state: LoadingState) : IView =
        StackPanel.create [
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.minWidth 280.
            StackPanel.spacing 25.
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.children [
                prefLine state.prefs.Current
                loadSetData state.setData.Current
                loadCollection state.dsEntries.Current
                loadCardInfo state.cardInfo.Current
                processEntries state.entries.Current
            ]
        ]
        :> IView

module ReadyView =
    let render (ctx: IComponentContext) (state: State) =
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

        Ready.View.view
            {
                cardInfo = state.cardInfo
                dsEntries = state.dsEntries
                entries = state.entries
                prefs = state.prefs
                setData = state.setData
            }

[<AutoOpen>]
module Main =
    let view =
        Component(fun ctx ->
            let loadState: LoadingState = {
                cardInfo = ctx.useState Prepare
                dsEntries = ctx.useState Wait
                entries = ctx.useState Wait
                prefs = ctx.useState Wait
                setData = ctx.useState Prepare
            }

            // Start loading
            ctx.useEffect (
                fun () ->
                    [
                        Effect.loadCardInfo loadState.cardInfo
                        Effect.loadCollection loadState.dsEntries
                        Effect.loadPrefs loadState.prefs
                        Effect.loadSetData loadState.setData
                    ]
                    |> Async.Parallel
                    |> Async.Ignore
                    |> Async.Start
                , [ EffectTrigger.AfterInit ]
            )

            // Calculate entries initially, when necessary data is ready
            ctx.useEffect (
                fun () ->
                    match loadState.cardInfo.Current, loadState.dsEntries.Current with
                    | Ready cardInfo, Processed dsEntries ->
                        Effect.calculateEntries cardInfo dsEntries loadState.entries |> Async.Start
                    | _ -> ()
                , [
                    EffectTrigger.AfterChange loadState.cardInfo
                    EffectTrigger.AfterChange loadState.dsEntries
                ]
            )

            // We render loading screen, if not everything is ready
            match
                loadState.cardInfo.Current,
                loadState.dsEntries.Current,
                loadState.entries.Current,
                loadState.prefs.Current,
                loadState.setData.Current
            with
            | Ready cardInfo, Processed dsEntires, Processed entries, Processed prefs, Ready setData ->
                let input: State = {
                    cardInfo = ctx.useState cardInfo
                    dsEntries = ctx.useState dsEntires
                    entries = ctx.useState entries
                    prefs = ctx.useState prefs
                    setData = ctx.useState setData
                }

                ReadyView.render ctx input
            | _ -> LoadingView.render loadState)
