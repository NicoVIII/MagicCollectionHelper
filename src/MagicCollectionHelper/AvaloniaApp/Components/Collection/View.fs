module MagicCollectionHelper.AvaloniaApp.Components.Collection.View

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Collection
open MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated
open MagicCollectionHelper.AvaloniaApp.Elements
open Avalonia.Media

let topBar (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    ActionButtonBar.create [
        ActionButton.create
            { text = "Import collection"
              isEnabled = not loadInProgress
              action = (fun _ -> ImportCollection |> dispatch)
              subPatch = Never }
    ]

let renderText prefs dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    let inventoryableAmount =
        List.sumBy
            (AgedEntryWithInfo.fromEntry infoMap
             >> function
             | Some entry -> entry ^. AgedEntryWithInfoLenses.amount
             | None -> 0u)
            entries

    TextBlock.create [
        TextBlock.textWrapping TextWrapping.Wrap
        TextBlock.text (
            match loadInProgress, List.isEmpty entries with
            | true, _ -> "Loading..."
            | false, true -> "Your collection is empty. Import it first."
            | false, false ->
                let cardAmount =
                    List.sumBy (fun (entry: DeckStatsCardEntry) -> entry.amount) dsEntries

                let percent =
                    (double inventoryableAmount) / (double cardAmount)
                    |> Numbers.percent prefs.numBase

                let inline pN p = Numbers.print prefs.numBase p

                $"You have %s{pN 0 cardAmount} cards in your collection.\n\n"
                + $"From those you can use {pN 0 inventoryableAmount} ({pN 1 percent}%%) for inventory."
                + " If you need more, please extend the info in your collection. Especially important are set, collector number and language."
        )
    ]
    :> IView

let content prefs dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    Border.create [
        Border.padding 10.
        Border.child (renderText prefs dsEntries entries infoMap state dispatch)
    ]
    :> IView

let render prefs dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            topBar state dispatch
            content prefs dsEntries entries infoMap state dispatch
        ]
    ]
    :> IView
