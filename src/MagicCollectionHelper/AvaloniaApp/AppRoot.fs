namespace MagicCollectionHelper.AvaloniaApp.Main

open Avalonia.FuncUI
open Avalonia.FuncUI.Types
open MagicCollectionHelper.Core.CardTypes
open MagicCollectionHelper.Core.DomainTypes
open MagicCollectionHelper.Core

type Loadable<'a> =
    | Prepare
    | Download
    | Import
    | Ready of 'a

type State = {
    cardInfo: IWritable<Loadable<CardInfoMap>>
    dsEntries: IWritable<Loadable<DeckStatsCardEntry list>>
    entries: IWritable<Loadable<Entry list>>
    setData: IWritable<Loadable<SetDataMap>>
}

module Effect =
    open MagicCollectionHelper.Core.Import

    let loadCardInfo (cardInfo: IWritable<Loadable<CardInfoMap>>) = async {
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

    let loadSetData (setData: IWritable<Loadable<SetDataMap>>) = async {
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

    let loadCollection (dsEntries: IWritable<Loadable<DeckStatsCardEntry list>>) = async {
        dsEntries.Set Import
        let! importedCollection = Persistence.DeckStatsCardEntry.load ()
        importedCollection |> Option.defaultValue [] |> Ready |> dsEntries.Set
    }

    let calculateEntries cardInfo dsEntries (entries: IWritable<Loadable<Entry list>>) = async {
        let! newEntries = DeckStatsCardEntry.listToEntriesAsync cardInfo dsEntries
        entries.Set(Ready newEntries)
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

    let loadSetData loadable =
        match loadable with
        | Prepare -> Icons.sync, "Preparing set data import..."
        | Download -> Icons.sync, "Downloading set data..."
        | Import -> Icons.sync, "Importing set data..."
        | Ready _ -> Icons.check, "Set data ready!"
        ||> loadingLine

    let loadCardInfo loadable =
        match loadable with
        | Prepare -> Icons.sync, "Preparing card info import..."
        | Download -> Icons.sync, "Downloading card info..."
        | Import -> Icons.sync, "Importing card info..."
        | Ready _ -> Icons.check, "Card info ready!"
        ||> loadingLine

    let loadCollection loadable =
        match loadable with
        | Prepare -> Icons.sync, "Preparing collection import..."
        | Import -> Icons.sync, "Importing collection..."
        | Ready _ -> Icons.check, "Collection imported!"
        | Download -> failwith "Invalid state"
        ||> loadingLine

    let processEntries loadable =
        match loadable with
        | Prepare -> Icons.reloadAlert, "Waiting..."
        | Import -> Icons.reload, "Process collection..."
        | Ready _ -> Icons.check, "Collection ready!"
        | Download -> failwith "Invalid state"
        ||> loadingLine

    let render (state: State) : IView =
        StackPanel.create [
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.minWidth 280.
            StackPanel.spacing 25.
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.children [
                loadSetData state.setData.Current
                loadCollection state.dsEntries.Current
                loadCardInfo state.cardInfo.Current
                processEntries state.entries.Current
            ]
        ]
        :> IView

[<AutoOpen>]
module Main =
    let view =
        Component(fun ctx ->
            let loadState = {
                cardInfo = ctx.useState Prepare
                dsEntries = ctx.useState Prepare
                entries = ctx.useState Prepare
                setData = ctx.useState Prepare
            }

            // Start loading
            ctx.useEffect (
                fun () ->
                    [
                        Effect.loadCardInfo loadState.cardInfo
                        Effect.loadCollection loadState.dsEntries
                        Effect.loadSetData loadState.setData
                    ]
                    |> Async.Parallel
                    |> Async.Ignore
                    |> Async.Start
                , [ EffectTrigger.AfterInit ]
            )

            // Calculate entries, when necessary data is ready
            ctx.useEffect (
                fun () ->
                    match loadState.cardInfo.Current, loadState.dsEntries.Current with
                    | Ready cardInfo, Ready dsEntries ->
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
                loadState.setData.Current
            with
            | Ready cardInfo, Ready dsEntires, Ready entries, Ready setData ->
                Ready.View.view
                    {
                        cardInfo = ctx.useState cardInfo
                        dsEntries = ctx.useState dsEntires
                        entries = ctx.useState entries
                        setData = ctx.useState setData
                    }
            | _ -> LoadingView.render loadState)
