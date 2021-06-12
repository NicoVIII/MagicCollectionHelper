module MagicCollectionHelper.AvaloniaApp.Components.Collection.View

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

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

let renderText dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    let inventoryableAmount =
        List.sumBy
            (AgedEntryWithInfo.fromEntry infoMap
             >> function
             | Some entry -> entry ^. AgedEntryWithInfoLenses.amount
             | None -> 0u)
            entries

    let cardAmount = List.sumBy (fun (entry: DeckStatsCardEntry) -> entry.amount) dsEntries

    TextBlock.create [
        TextBlock.textWrapping TextWrapping.Wrap
        TextBlock.text (
            match loadInProgress, List.isEmpty entries with
            | true, _ -> "Loading..."
            | false, true -> "Your collection is empty. Import it first."
            | false, false ->
                let percent =
                    (double inventoryableAmount) / (double cardAmount)
                    * 100.

                $"You have %i{cardAmount} cards in your collection.\n\n"
                + $"From those you can use {inventoryableAmount} (%.1f{percent}%%) for inventory."
                + " If you need more, please extend the info in your collection. Especially important are set, collector number and language."
        )
    ]
    :> IView

let content dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    Border.create [
        Border.padding 10.
        Border.child (renderText dsEntries entries infoMap state dispatch)
    ]
    :> IView

let render dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            topBar state dispatch
            content dsEntries entries infoMap state dispatch
        ]
    ]
    :> IView
